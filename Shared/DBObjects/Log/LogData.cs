namespace SharedClass.DBObjects.Logs
{
	public class LogData
	{
		public string Who { get; set; }
		public DateTime When { get; set; }
		public bool IsError { get; set; }
		public string Message { get; set; }
	}
}
