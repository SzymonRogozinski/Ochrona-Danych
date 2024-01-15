using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.AccountStatus
{
    public class CryptedAccountStatus
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string CryptedInfo { get; set; }

    }
}
