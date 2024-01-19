using System.ComponentModel.DataAnnotations;

namespace SharedClass.DBObjects.Logs
{
	public class CryptedLogData
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public string CryptedInfo { get; set; }
	}
}
