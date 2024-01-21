using BankAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.DBObjects.AccountData;
using SharedClass;
using SharedClass.ClientObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankAPI.Services
{
	public interface IAuthService
	{
		public Task<ServiceResponse<string>> GetTemplate(string userName, string adress);
		public Task<ServiceResponse<string>> GetToken(string template, string userName, string password, string adress);
		public Task<ServiceResponse<bool>> ChangePassword(PasswordChangeForm form, string username);
	}

	public class AuthService : IAuthService
	{
		private readonly DataContext _context;
		private readonly IConfiguration _config;
		private readonly ILogService _logService;
		private readonly Cryptographer _cryptographer;
		private byte[] salt;
		private readonly RNGCryptoServiceProvider rng;

		public AuthService(DataContext context, IConfiguration config, ILogService logService, Cryptographer cryptographer)
		{
			this._context = context;
			this._config = config;
			this._logService = logService;
			this._cryptographer = cryptographer;
			this.rng = new RNGCryptoServiceProvider();
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
				var uncryptAcc = _cryptographer.Decrypt(acc);
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
				var unAcc = _cryptographer.Decrypt(acc);
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
					await _logService.AddLog($"GetToken", false, "Attack Alert!");
					await _logService.AddLog($"GetToken:{username}/{adress}", true, "Cannot found password template!");
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

		public async Task<ServiceResponse<bool>> ChangePassword(PasswordChangeForm form, string username)
		{
			try
			{
				var userExist = await UserExist(username);
				if (!userExist)
				{
					await _logService.AddLog($"ChangePassword:{username}", true, "Account don't exist!");
					return new ServiceResponse<bool>
					{
						Success = false,
					};
				}
				var acc = await _context.Accounts.FirstAsync(user => user.UserName == username);
				var unAcc = _cryptographer.Decrypt(acc);

				//Check old password
				string password = CheckOldPassword(form.oldPassword, unAcc.Passwords[0].PasswordTempalte);

				//Wrong password?
				if (!VerifyPasswordHash(password.ToString(), unAcc.Passwords[0].PasswordValue, unAcc.PasswordSalt))
				{
					return new ServiceResponse<bool>
					{
						Message = "Old password is wrong!",
						Success = false,
					};
				}

				//Create new password template
				var passwords = CreateNewPasswordTemplate(form.newPassword);
				//Update passwords
				_context.ChangeTracker.Clear();

				unAcc.Passwords = passwords.ToArray();
				unAcc.PasswordSalt = salt;

				var copyId = acc.Id;
				//Copy
				var newAccountData = _cryptographer.Encrypt(unAcc);
				newAccountData.Id = copyId;

				var updatedAccountData = new CryptedAccountData() { Id = copyId };
				_context.Accounts.Attach(updatedAccountData);

				updatedAccountData.UserName = newAccountData.UserName;
				updatedAccountData.CryptedInfo = newAccountData.CryptedInfo;
				updatedAccountData.AccountNumber = newAccountData.AccountNumber;

				await _context.SaveChangesAsync();

				return new ServiceResponse<bool>
				{
					Success = true
				};

			}
			catch (Exception e)
			{
				await _logService.AddLog($"ChangePassword:{username}", true, e.Message);
				return new ServiceResponse<bool>
				{
					Success = false,
				};
			}
		}

		private string CheckOldPassword(string oldPassword, string template)
		{
			char[] passChars = oldPassword.ToCharArray();
			for (int i = 0; i < template.Length; i++)
			{
				if (template[i] == '*')
				{
					passChars[i] = '*';
				}
			}
			return new string(passChars);
		}

		private List<Password> CreateNewPasswordTemplate(string password)
		{
			var result = new List<Password>();
			int len = password.Length;
			Random random = new Random();

			this.salt = new byte[8];
			rng.GetBytes(salt);
			var hmac = new HMACSHA512(salt);

			//How many new passwords
			for (int iter = 0; iter < 5; iter++)
			{
				char[] passwordSliced = password.ToCharArray();
				bool[] passwordHiddenChars = new bool[len];
				int count = 0;
				//Creating new password
				while (count < len / 2)
				{
					int position = random.Next(len);
					if (!passwordHiddenChars[position])
					{
						passwordHiddenChars[position] = true;
						passwordSliced[position] = '*';
						count++;
					}
				}
				//Hash new password
				string hidePassword = new string(passwordSliced);
				byte[] passwordValue = hmac.ComputeHash(Encoding.ASCII.GetBytes(hidePassword));
				//Get template
				string template = "";
				foreach (bool b in passwordHiddenChars)
				{
					template += b ? '*' : ' ';
				}
				result.Add(new Password()
				{
					PasswordValue = passwordValue,
					PasswordTempalte = template
				});
			}
			return result;

		}

		private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
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
