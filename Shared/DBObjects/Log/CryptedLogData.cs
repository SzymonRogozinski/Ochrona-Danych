using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.Logs
{
    public class CryptedLogData
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string CryptedInfo { get; set; }
    }
}
