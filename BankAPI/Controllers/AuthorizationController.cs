using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Shared.DBObjects.AccountStatus;
using SharedClass;
using SharedClass.ClientObjects;
using SharedClass.DBObjects.Logs;
using System.Security.Claims;
using System.Text.Json;

namespace BankAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthorizationController : Controller
	{
		private readonly static int LongDelay = 5000; //5 seconds
		private readonly static int ShortDelay = 2000;
		private readonly string nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		private readonly IAuthService _authService;
		private readonly IStatusService _statusService;
		private readonly ILogService _logService;
		private readonly PasswordChecker _passwordChecker;
		public AuthorizationController(IAuthService authService, IStatusService statusService, ILogService logService, PasswordChecker passwordChecker)
		{
			this._authService = authService;
			this._statusService = statusService;
			this._logService = logService;
			this._passwordChecker = passwordChecker;
		}

		[HttpPost("template")]
		public async Task<ActionResult<ServiceResponse<string>>> GetTemplate(UserNameQuestionary UserName)
		{
			Thread.Sleep(LongDelay);

			var adr = Request.Headers.Origin;

			var temp = await _authService.GetTemplate(UserName.value, adr);
			if (!temp.Success && string.IsNullOrEmpty(temp.Message))
			{
				return BadRequest
					(
					new ServiceResponse<string>
					{
						Success = temp.Success,
						Message = "Ups, something go wrong!"
					});
			}
			//Succes
			await _logService.AddLog($"GetTemplate:{UserName.value}/{adr}", false, temp.Message == null ? "Ok" : temp.Message);
			return Ok
				(
				new ServiceResponse<string>
				{
					Data = temp.Data,
					Success = temp.Success,
					Message = temp.Message == null ? "Ok" : temp.Message,
				});
		}

		[HttpPost("login")]
		public async Task<ActionResult<ServiceResponse<string>>> Login(LoginQuestionary loginInfo)
		{
			Thread.Sleep(LongDelay);

			var adr = Request.Headers.Origin;

			var stat = await _statusService.GetStatus(loginInfo.userName);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";

				return Ok
					 (
					 new ServiceResponse<string>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
					 });
			}

			var temp = await _authService.GetToken(loginInfo.template, loginInfo.userName, loginInfo.password, adr);
			//Hidden error
			if (!temp.Success && string.IsNullOrEmpty(temp.Message))
			{
				return BadRequest
					(
					new ServiceResponse<string>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}

			//Reduce trials
			if (!temp.Success && temp.Message == "Wrong password")
			{
				await _logService.AddLog($"Login:{loginInfo.userName}/{adr}", false, "User wrote wrong password");
				await _statusService.ReduceTrials(loginInfo.userName);
			}
			//Reset trials
			else if (temp.Success)
			{
				await _statusService.RestartTrials(loginInfo.userName);
				await _logService.AddLog($"Login:{loginInfo.userName}/{adr}", false, "Ok");
			}
			//Succes
			return Ok
				(
				new ServiceResponse<string>
				{
					Data = temp.Data,
					Success = temp.Success,
					Message = temp.Message == null ? "Ok" : temp.Message,
				});
		}

		[HttpPost("forgot")]
		public async Task<ActionResult<ServiceResponse<bool>>> forgotPassword(UserNameQuestionary UserName)
		{
			Thread.Sleep(LongDelay);

			var adr = Request.Headers.Origin;

			var stat = await _statusService.GetStatus(UserName.value);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";

				return Ok
					 (
					 new ServiceResponse<bool>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
					 });
			}

			var temp = await _authService.GetTemplate(UserName.value, adr);
			if (!temp.Success)
			{
				return BadRequest
						(
						new ServiceResponse<bool>
						{
							Success = false,
							Message = "Ups, something go wrong!"
						});
			}
			await _logService.AddLog($"ChangingPassword:{UserName.value}/{adr}", false, "User request to restore his password. Control word \"Okoń\"");
			return Ok
				(
				new ServiceResponse<bool>
				{
					Data = true,
					Success = true,
					Message = "Your request for password restoring was accepted. Go to the closest IBank location and give secret word: \"Okoń\".",
				});

		}

		[Authorize(Roles = "admin")]
		[HttpGet("logs")]
		public async Task<ActionResult<ServiceResponse<List<LogData>>>> Logs()
		{
			Thread.Sleep(ShortDelay);
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			//Illegal token?
			if (username == null)
			{
				await _logService.AddLog($"Logs:{username}", true, "User sent request with username that don'texist!");
				return BadRequest
					(
					new ServiceResponse<List<LogData>>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}
			var stat = await _statusService.GetStatus(username);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";
				await _logService.AddLog($"Logs:{username}", true, msg);
				return Ok
					 (
					 new ServiceResponse<List<LogData>>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
					 });
			}
			try
			{
				var logs = await _logService.GetLogs();
				return Ok(new ServiceResponse<List<LogData>>()
				{
					Data = logs,
					Success = true,
					Message = "Ok"
				});
			}
			catch (Exception ex)
			{
				return Ok(new ServiceResponse<List<LogData>>()
				{
					Success = false,
					Message = ex.Message
				});
			}
		}

		[HttpPost("change-password")]
		public async Task<ActionResult<ServiceResponse<bool>>> PasswordChange(PasswordChangeForm form)
		{
			Thread.Sleep(LongDelay);
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			//Illegal token?
			if (username == null)
			{
				await _logService.AddLog($"ChangePassword:{username}", true, "User sent request with username that don'texist!");
				return BadRequest
					(
					new ServiceResponse<bool>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}
			var stat = await _statusService.GetStatus(username);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";
				await _logService.AddLog($"ChangePassword:{username}", true, msg);

				return Ok
					 (
					 new ServiceResponse<bool>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
					 });
			}

			var passCheck = _passwordChecker.FullPasswordCheck(form.newPassword);


			if (!passCheck.Success)
			{
				return BadRequest
					(
					new ServiceResponse<bool>
					{
						Success = false,
						Message = "Password is illegal!"
					});
			}

			var res = await _authService.ChangePassword(form, username);

			if (!res.Success && string.IsNullOrEmpty(res.Message))
			{
				return BadRequest
						(
						new ServiceResponse<bool>
						{
							Success = false,
							Message = "Ups, something go wrong!"
						});
			}
			await _logService.AddLog($"ChangePassword:{username}", false, res.Message == null ? "Ok" : res.Message);
			return Ok
			(
			new ServiceResponse<bool>
			{
				Success = res.Success,
				Message = res.Message == null ? "Ok" : res.Message
			});
		}

		private string GetUsername(StringValues auth)
		{
			var token = auth.ToString().Replace("Bearer ", "");

			var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

			var claimsList = identity.Claims;
			string username = null;
			foreach (var claim in claimsList)
			{
				if (claim.Type == nameType)
				{
					username = claim.Value;
					break;
				}
			}
			return username;
		}

		private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
		{
			var payload = jwt.Split('.')[1];
			var jsonBytes = ParseBase64WithoutPadding(payload);
			var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
			var claims = keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
			return claims;
		}

		private byte[] ParseBase64WithoutPadding(string base64)
		{
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
			}
			return Convert.FromBase64String(base64);
		}
	}
}
