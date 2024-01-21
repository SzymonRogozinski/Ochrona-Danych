using BankAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Shared.DBObjects.AccountStatus;
using SharedClass;
using System.Security.Claims;
using System.Text.Json;

namespace BankAPI.Controllers
{
	[Route("")]
	[ApiController]
	public class HoneyPot : Controller
	{
		private readonly static int ShortDelay = 2000;
		private readonly string nameType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
		private readonly string trapLink = "https://c.tenor.com/vH75Q3ONk1sAAAAC/tenor.gif";
		private readonly IAuthService _authService;
		private readonly IStatusService _statusService;
		private readonly ILogService _logService;
		public HoneyPot(IAuthService authService, IStatusService statusService, ILogService logService, PasswordChecker passwordChecker)
		{
			this._authService = authService;
			this._statusService = statusService;
			this._logService = logService;
		}

		[HttpGet("static")]
		[HttpGet("config")]
		[HttpGet("main")]
		[HttpGet("passwords")]
		[HttpGet("logs")]
		[HttpGet("admin")]
		[HttpGet("index.php")]
		[HttpGet(".htaccess")]
		[HttpGet("login")]
		[HttpGet("wp-admin")]
		[HttpGet("wp-login.php")]
		public async Task<ActionResult> trap()
		{
			Thread.Sleep(ShortDelay);
			var adr = Request.Headers.Origin;
			var auth = Request.Headers.Authorization;
			string username = GetUsername(auth);
			await _logService.AddLog($"HoneyPot:{adr}", false, "Somebody try something nasty!");
			//Illegal token?
			if (username != null)
			{
				await _logService.AddLog($"HoneyPot:{username}/{adr}", false, "User have been banned");
				await _statusService.SetStatus(username, Statuses.BANNED);
			}

			return new RedirectResult(trapLink);
		}

		private string GetUsername(StringValues auth)
		{
			try
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
			catch
			{
				return null;
			}
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
