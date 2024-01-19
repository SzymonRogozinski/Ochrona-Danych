using Microsoft.Extensions.Options;
using SharedClass.ClientObjects;
using System.Net.Http.Json;

namespace SharedClass.Services
{
    public interface IBankClient
    {
        Task<ServiceResponse<List<TransferInfo>>> GetTransfers();
        Task<ServiceResponse<bool>> MakeTransfers(TransferForm form);
        Task<ServiceResponse<AccountInfo>> GetDetails();

    }

    public class BankClient : IBankClient
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly TokenHolder _tokenHolder;

        public BankClient(HttpClient httpClient, IOptions<AppSettings> appSettings, TokenHolder tokenHolder)
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

        public async Task<ServiceResponse<List<TransferInfo>>> GetTransfers()
        {
            isTokenChanged();
            try
            {
                var response = await _httpClient.GetAsync(_appSettings.BankEndpoint.GetTransfersEndpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<List<TransferInfo>>()
                    {
                        Success = false,
                        Message = "Something goes wrong!"
                    };
                }
                var data = await response.Content.ReadFromJsonAsync<ServiceResponse<List<TransferInfo>>>();
                return data;
            }
            catch (Exception e)
            {
                return new ServiceResponse<List<TransferInfo>>()
                {
                    Success = false,
                    Message = "Something goes wrong!"
                };
            }
        }

        public async Task<ServiceResponse<bool>> MakeTransfers(TransferForm form)
        {
            isTokenChanged();
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_appSettings.BankEndpoint.MakeTransferEndpoint, form);

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

        public async Task<ServiceResponse<AccountInfo>> GetDetails()
        {
            isTokenChanged();
            try
            {
                var response = await _httpClient.GetAsync(_appSettings.BankEndpoint.GetDetailsEndpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<AccountInfo>()
                    {
                        Success = false,
                        Message = "Something goes wrong!"
                    };
                }
                var data = await response.Content.ReadFromJsonAsync<ServiceResponse<AccountInfo>>();
                return data;
            }
            catch (Exception e)
            {
                return new ServiceResponse<AccountInfo>()
                {
                    Success = false,
                    Message = "Something goes wrong!"
                };
            }
        }
    }
}
