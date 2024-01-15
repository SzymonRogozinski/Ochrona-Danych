using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DBObjects.AccountStatus;
using Shared.DBObjects.Logs;
using SharedClass;
using SharedClass.ClientObjects;

namespace BankAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthorizationController : Controller
	{
		private readonly static int SlowedDown = 5000; //5 seconds
		private readonly IAuthService _authService;
		private readonly IStatusService _statusService;
		private readonly ILogService _logService;
		public AuthorizationController(IAuthService authService, IStatusService statusService, ILogService logService)
		{
			this._authService = authService;
			this._statusService = statusService;
			this._logService = logService;
		}

		[HttpPost("template")]
		public async Task<ActionResult<ServiceResponse<string>>> GetTemplate(UserNameQuestionary UserName)
		{
			Thread.Sleep(SlowedDown);

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
			await _logService.AddLog($"GetTemplate:{UserName.value}/{adr}", false, "Ok");
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
		public async Task<ActionResult<ServiceResponse<bool>>> Login(LoginQuestionary loginInfo)
		{
			Thread.Sleep(SlowedDown);

			var adr = Request.Headers.Origin;

			var stat = await _statusService.GetStatus(loginInfo.userName);
			if (stat != Statuses.OK)
			{
				return Ok
					 (
					 new ServiceResponse<string>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
					 });
			}

			var temp = await _authService.GetToken($"Login:{loginInfo.userName}/{adr}", loginInfo.userName, loginInfo.password, adr);
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
			}
			//Succes
			await _logService.AddLog($"Login:{loginInfo.userName}/{adr}", false, "Ok");
			return Ok
				(
				new ServiceResponse<string>
				{
					Data = temp.Data,
					Success = temp.Success,
					Message = temp.Message == null ? "Ok" : temp.Message,
				});
		}

		[Authorize(Roles = "admin")]
		[HttpGet("logs")]
		public async Task<ActionResult<ServiceResponse<List<LogData>>>> Logs()
		{
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

		[Authorize(Roles = "admin")]
		[HttpPost("register")]
		public async Task<ActionResult<ServiceResponse<bool>>> Register()
		{
			//Legal chars for passwords a-zA-z + polish + numbers 
			return Ok
				(
				new ServiceResponse<bool>
				{
					Success = true,
					Message = "There will be something"
				});
		}

		[HttpPost("change-password")]
		public async Task<ActionResult<ServiceResponse<bool>>> PasswordChange()
		{
			return Ok
				(
				new ServiceResponse<bool>
				{
					Success = true,
					Message = "There will be something"
				});
		}
	}
}
