namespace SharedClass.ClientObjects
{
	public class TokenHolder
	{
		public string? token { get; set; }
		public string? role { get; set; }
		public DateTime? expires { get; set; }

		private AuthStateProvider _authStateProvider;

		public void setAuthProvider(AuthStateProvider authStateProvider)
		{
			if (_authStateProvider == null)
				_authStateProvider = authStateProvider;
		}

		public async Task ClearToken()
		{
			token = null;
			role = null;
			expires = null;
			await _authStateProvider.GetAuthenticationStateAsync();
		}

		public async Task DelayClear()
		{
			await Task.Run(async () =>
			{
				Thread.Sleep(5000);
			});
			ClearToken();
		}
	}
}
