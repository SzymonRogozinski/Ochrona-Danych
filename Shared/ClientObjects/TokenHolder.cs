namespace SharedClass.ClientObjects
{
	public class TokenHolder
	{
		public string? token { get; set; }
		public string? role { get; set; }
		public DateTime? expires { get; set; }

		public void ClearToken()
		{
			token = null;
			role = null;
			expires = null;
		}
	}
}
