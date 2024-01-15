namespace SharedClass.ClientObjects
{
	public class AppSettings
	{
		public string DefaultLanguage { get; set; }
		public string BaseAPIUrl { get; set; }
		public BankEndpoint BankEndpoint { get; set; }
		public AuthEndpoint AuthEndpoint { get; set; }
	}

	public class AuthEndpoint
	{
		public string Base_url { get; set; }
		public string TemplateEndpoint { get; set; }
		public string LoginEndpoint { get; set; }
		public string RegisterEndpoint { get; set; }
		public string ChangePasswordEndpoint { get; set; }
		public string GetLogsEndpoint { get; set; }
	}

	public class BankEndpoint
	{
		public string Base_url { get; set; }
		public string GetTransfersEndpoint { get; set; }
		public string MakeTransferEndpoint { get; set; }
		public string GetDetailsEndpoint { get; set; }
	}

}
