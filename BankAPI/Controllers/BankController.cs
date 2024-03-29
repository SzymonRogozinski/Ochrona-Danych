﻿using BankAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Shared.DBObjects.AccountStatus;
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
		private readonly static int Delay = 3000;
		private readonly string nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		private readonly IBankService _bankService;
		private readonly ILogService _logService;
		private readonly IStatusService _statusService;

		public BankController(IBankService bankService, ILogService logService, IStatusService statusService)
		{
			this._bankService = bankService;
			this._logService = logService;
			this._statusService = statusService;
		}

		[Authorize]
		[HttpGet("getTransfers")]
		public async Task<ActionResult<ServiceResponse<List<TransferInfo>>>> GetTransfers()
		{
			Thread.Sleep(Delay);
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
			var stat = await _statusService.GetStatus(username);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";
				await _logService.AddLog($"GetTransfers:{username}", true, msg);

				return Ok
					 (
					 new ServiceResponse<List<TransferInfo>>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
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
			Thread.Sleep(Delay);
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
			var stat = await _statusService.GetStatus(username);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";
				await _logService.AddLog($"MakeTransfer:{username}", true, msg);

				return Ok
					 (
					 new ServiceResponse<bool>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
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
					Message = result.Message == null ? "Ok" : result.Message
				});
		}

		[Authorize]
		[HttpGet("seeDetails")]
		public async Task<ActionResult<ServiceResponse<AccountInfo>>> SeeDetails()
		{
			Thread.Sleep(Delay);
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			//Illegal token?
			if (username == null)
			{
				await _logService.AddLog($"SeeDetails:{username}", true, "User sent request with username that don'texist!");
				return BadRequest
					(
					new ServiceResponse<AccountInfo>
					{
						Success = false,
						Message = "Ups, something go wrong!"
					});
			}
			var stat = await _statusService.GetStatus(username);
			if (stat != Statuses.OK)
			{
				string msg = stat == Statuses.PASSWORDS_TRIALS_OUT ? "You run out of password trials!" : "You have been blocked!";
				await _logService.AddLog($"SeeDetails:{username}", true, msg);

				return Ok
					 (
					 new ServiceResponse<AccountInfo>
					 {
						 Success = false,
						 Message = "You run out of password trials!"
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
