using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.AccountStatus
{
    public class PrivateAccountStatus
    {
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public int HowManyTrials { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
