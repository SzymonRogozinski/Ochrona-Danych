using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace SharedClass.ClientObjects
{
	public class AuthStateProvider : AuthenticationStateProvider
	{
		private readonly TokenHolder _tokenHolder;
		private readonly static string roleTypeName = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

		public AuthStateProvider(TokenHolder tokenHolder)
		{
			_tokenHolder = tokenHolder;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var identity = new ClaimsIdentity();

			//Time validation
			if (!string.IsNullOrEmpty(_tokenHolder.token) && !validateTime(_tokenHolder.token))
			{
				_tokenHolder.ClearToken();
			}
			else if (!string.IsNullOrEmpty(_tokenHolder.token))
			{
				try
				{
					identity = new ClaimsIdentity(ParseClaimsFromJwt(_tokenHolder.token), "jwt");
					_tokenHolder.token = _tokenHolder.token.Replace("\"", "");
					var claimsList = new ClaimsIdentity(ParseClaimsFromJwt(_tokenHolder.token), "jwt").Claims;
					foreach (var claim in claimsList)
					{
						if (claim.Type == roleTypeName)
						{
							_tokenHolder.role = claim.Value;
							break;
						}
					}
				}
				catch (Exception)
				{
					_tokenHolder.ClearToken();
					identity = new ClaimsIdentity();
				}
			}

			var user = new ClaimsPrincipal(identity);
			var state = new AuthenticationState(user);
			NotifyAuthenticationStateChanged(Task.FromResult(state));
			return state;
		}

		private bool validateTime(string token)
		{
			token = token.Replace("\"", "");
			var handler = new JwtSecurityTokenHandler();
			var jwtSecurityToken = handler.ReadJwtToken(token);
			var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
			var ticks = long.Parse(tokenExp);

			var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
			var now = DateTime.Now.ToUniversalTime();

			_tokenHolder.expires = tokenDate;

			return tokenDate >= now;
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
