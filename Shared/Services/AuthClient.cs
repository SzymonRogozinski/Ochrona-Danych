using Microsoft.Extensions.Options;
using SharedClass.ClientObjects;
using SharedClass.DBObjects.Logs;
using System.Net.Http.Json;

namespace SharedClass.Services
{

	public interface IAuthClient
	{
		Task<ServiceResponse<string>> GetTemplate(UserNameQuestionary UserName);
		Task<ServiceResponse<bool>> ForgetPassword(UserNameQuestionary UserName);
		Task<ServiceResponse<string>> Login(LoginQuestionary loginInfo);
		Task<ServiceResponse<List<LogData>>> GetLogs();
		Task<ServiceResponse<bool>> ChangePassword(PasswordChangeForm form);
	}

	public class AuthClient : IAuthClient
	{
		private readonly HttpClient _httpClient;
		private readonly AppSettings _appSettings;
		private readonly TokenHolder _tokenHolder;

		public AuthClient(HttpClient httpClient, IOptions<AppSettings> appSettings, TokenHolder tokenHolder)
		{
			_httpClient = httpClient;
			_appSettings = appSettings.Value;
			_tokenHolder = tokenHolder;
			if (!string.IsNullOrEmpty(_tokenHolder.token))
				_httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _tokenHolder.token));
		}

		private void isTokenChanged()
		{
			if (_tokenHolder.token != null && !_httpClient.DefaultRequestHeaders.Contains(_tokenHolder.token))
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
				_httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", _tokenHolder.token));
			}
			else if (_httpClient.DefaultRequestHeaders.Contains("Authorization") && _tokenHolder.token == null)
			{
				_httpClient.DefaultRequestHeaders.Remove("Authorization");
			}
		}

		public async Task<ServiceResponse<string>> GetTemplate(UserNameQuestionary UserName)
		{
			isTokenChanged();
			try
			{
				var response = await _httpClient.PostAsJsonAsync(_appSettings.AuthEndpoint.TemplateEndpoint, UserName);

				if (!response.IsSuccessStatusCode)
				{
					return new ServiceResponse<string>()
					{
						Success = false,
						Message = "Something goes wrong!"
					};
				}
				var data = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
				return data;
			}
			catch (Exception e)
			{
				return new ServiceResponse<string>()
				{
					Success = false,
					Message = "Something goes wrong!"
				};
			}
		}

		public async Task<ServiceResponse<bool>> ForgetPassword(UserNameQuestionary UserName)
		{
			isTokenChanged();
			try
			{
				var response = await _httpClient.PostAsJsonAsync(_appSettings.AuthEndpoint.ForgetPassword, UserName);

				if (!response.IsSuccessStatusCode)
				{
					return new ServiceResponse<bool>()
					{
						Success = false,
						Message = "Something goes wrong!"
					};
				}
				var data = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
				return data;
			}
			catch (Exception e)
			{
				return new ServiceResponse<bool>()
				{
					Success = false,
					Message = "Something goes wrong!"
				};
			}
		}

		public async Task<ServiceResponse<string>> Login(LoginQuestionary loginInfo)
		{
			isTokenChanged();
			try
			{
				var response = await _httpClient.PostAsJsonAsync(_appSettings.AuthEndpoint.LoginEndpoint, loginInfo);

				if (!response.IsSuccessStatusCode)
				{
					return new ServiceResponse<string>()
					{
						Success = false,
						Message = "Something goes wrong!"
					};
				}
				var data = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
				return data;
			}
			catch (Exception e)
			{
				return new ServiceResponse<string>()
				{
					Success = false,
					Message = "Something goes wrong!"
				};
			}
		}

		public async Task<ServiceResponse<List<LogData>>> GetLogs()
		{
			isTokenChanged();
			try
			{
				var response = await _httpClient.GetAsync(_appSettings.AuthEndpoint.GetLogsEndpoint);
				if (!response.IsSuccessStatusCode)
				{
					return new ServiceResponse<List<LogData>>()
					{
						Success = false,
						Message = "Something goes wrong!"
					};
				}
				var data = await response.Content.ReadFromJsonAsync<ServiceResponse<List<LogData>>>();
				return data;
			}
			catch (Exception e)
			{
				return new ServiceResponse<List<LogData>>()
				{
					Success = false,
					Message = "Something goes wrong!"
				};
			}
		}

		public async Task<ServiceResponse<bool>> ChangePassword(PasswordChangeForm form)
		{
			isTokenChanged();
			try
			{
				var response = await _httpClient.PostAsJsonAsync(_appSettings.AuthEndpoint.ChangePasswordEndpoint, form);

				if (!response.IsSuccessStatusCode)
				{
					return new ServiceResponse<bool>()
					{
						Success = false,
						Message = "Something goes wrong!"
					};
				}
				var data = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
				return data;
			}
			catch (Exception e)
			{
				return new ServiceResponse<bool>()
				{
					Success = false,
					Message = "Something goes wrong!"
				};
			}




			throw new NotImplementedException();
		}
	}
}
