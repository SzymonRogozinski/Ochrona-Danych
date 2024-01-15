using BankAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.DBObjects.AccountData;
using SharedClass;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankAPI.Services
{
	public interface IAuthService
	{
		public Task<ServiceResponse<string>> GetTemplate(string userName, string adress);
		public Task<ServiceResponse<string>> GetToken(string template, string userName, string password, string adress);
	}

	public class AuthService : IAuthService
	{
		private readonly DataContext _context;
		private readonly IConfiguration _config;
		private readonly ILogService _logService;

		public AuthService(DataContext context, IConfiguration config, ILogService logService)
		{
			this._context = context;
			this._config = config;
			this._logService = logService;
		}

		public async Task<ServiceResponse<string>> GetTemplate(string username, string adress)
		{
			try
			{
				var userExist = await UserExist(username);
				if (!userExist)
				{
					await _logService.AddLog($"GetTemplate:{username}/{adress}", false, "Account don't exist!");
					return new ServiceResponse<string>
					{
						Message = "Account don't exist!",
						Success = false,
					};
				}
				var rand = new Random();

				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);
				var uncryptAcc = Cryptographer.Decrypt(acc);
				var template = uncryptAcc.Passwords[rand.Next(uncryptAcc.Passwords.Length)].PasswordTempalte;
				return new ServiceResponse<string>
				{
					Data = template,
					Success = true,
				};
			}
			catch (Exception e)
			{
				await _logService.AddLog($"GetTemplate:{username}/{adress}", true, e.Message);
				return new ServiceResponse<string>
				{
					Success = false,
				};
			}
		}

		public async Task<ServiceResponse<string>> GetToken(string template, string username, string password, string adress)
		{
			try
			{
				var userExist = await UserExist(username);
				if (!userExist)
				{
					await _logService.AddLog($"GetToken:{username}/{adress}", false, "Account don't exist!");
					return new ServiceResponse<string>
					{
						Message = "Account don't exist!",
						Success = false,
					};
				}
				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);
				var unAcc = Cryptographer.Decrypt(acc);
				//hash password
				int i = 0;
				byte[] pass = null;
				while (i < unAcc.Passwords.Length)
				{
					if (unAcc.Passwords[i].PasswordTempalte == template)
					{
						pass = unAcc.Passwords[i].PasswordValue;
						break;
					}
					i++;
				}
				//Not find template
				if (pass == null)
				{
					//Illegal state, propably attack
					return new ServiceResponse<string>
					{
						Success = false,
					};
				}
				//Wrong password
				if (!VerifyPasswordHash(password, pass, unAcc.PasswordSalt))
				{
					return new ServiceResponse<string>
					{
						Message = "Wrong password",
						Success = false,
					};
				}
				return new ServiceResponse<string>
				{
					Data = CreateToken(unAcc, acc.Id),
					Success = true,
				};
			}
			catch (Exception e)
			{
				await _logService.AddLog($"GetToken{username}/{adress}", true, e.Message);
				return new ServiceResponse<string>
				{
					Success = false,
				};
			}
		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computedHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateToken(AccountData user, int id)
		{
			List<Claim> claims = new List<Claim>()
			 {
				 new Claim(ClaimTypes.NameIdentifier, id.ToString()),
				 new Claim(ClaimTypes.Name, user.UserName),
				 new Claim(ClaimTypes.Role, user.Role),
			 };

			SymmetricSecurityKey key =
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

			SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
							   claims: claims,
							   expires: DateTime.Now.AddMinutes(30),
							   signingCredentials: creds
				  );
			var xd = new JwtSecurityTokenHandler();

			var tokenHandler = xd.WriteToken(token);
			return tokenHandler;
		}


		private async Task<bool> UserExist(string userName)
		{
			var u = await _context.Accounts.AnyAsync(user => user.UserName == userName);
			if (u)
			{
				return true;
			}
			return false;
		}
	}
}
