using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SharedClass;
using SharedClass.ClientObjects;
using System.Security.Claims;
using System.Text.Json;

namespace BankAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BankController : Controller
	{
		private readonly static int SlowedDown = 3000;
		private readonly string nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		private readonly IBankService _bankService;
		private readonly ILogService _logService;

		public BankController(IBankService bankService, ILogService logService)
		{
			this._bankService = bankService;
			this._logService = logService;
		}

		[Authorize]
		[HttpGet("getTransfers")]
		public async Task<ActionResult<ServiceResponse<List<TransferInfo>>>> GetTransfers()
		{
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			//Illegal token?
			if (username == null)
			{
				await _logService.AddLog($"GetTransfers:{username}", true, "User sent request with username that don'texist!");
				return BadRequest
					(
					new ServiceResponse<string>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}
			var res = await _bankService.GetTransfers(username);
			if (res.Success)
			{
				await _logService.AddLog($"GetTransfers:{username}", false, "Ok");
			}

			return Ok
				(
				new ServiceResponse<List<TransferInfo>>
				{
					Data = res.Data,
					Success = res.Success,
					Message = res.Success ? res.Message : "Ups, something go wrong!"
				});
		}

		[Authorize]
		[HttpPost("makeTransfer")]
		public async Task<ActionResult<ServiceResponse<bool>>> MakeTransfer(TransferForm form)
		{
			Thread.Sleep(SlowedDown);
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			//Illegal token?
			if (username == null)
			{
				await _logService.AddLog($"MakeTransfer:{username}", true, "User sent request with username that don'texist!");
				return BadRequest
					(
					new ServiceResponse<string>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}

			var result = await _bankService.MakeTransfer(form, username);

			if (!string.IsNullOrEmpty(result.Message))
			{
				await _logService.AddLog($"MakeTransfer:{username}", false, result.Message);
			}

			return Ok
				(
				new ServiceResponse<bool>
				{
					Success = result.Success,
					Message = result.Success ? "Ok" : "Ups, something go wrong!"
				});
		}

		[Authorize]
		[HttpGet("seeDetails")]
		public async Task<ActionResult<ServiceResponse<AccountInfo>>> SeeDetails()
		{
			Thread.Sleep(SlowedDown);
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			//Illegal token?
			if (username == null)
			{
				await _logService.AddLog($"SeeDetails:{username}", true, "User sent request with username that don'texist!");
				return BadRequest
					(
					new ServiceResponse<string>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}
			var res = await _bankService.GetDetails(username);

			if (res.Success)
			{
				await _logService.AddLog($"SeeDetails:{username}", false, "Ok");
			}

			return Ok
				(
				new ServiceResponse<AccountInfo>
				{
					Data = res.Data,
					Success = res.Success,
					Message = res.Success ? res.Message : "Ups, something go wrong!"
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
