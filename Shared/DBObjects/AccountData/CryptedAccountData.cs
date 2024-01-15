using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.AccountData
{
    public class CryptedAccountData
    {
        public int Id { get; set; }
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890.]+$", ErrorMessage = "UserName contains illegal char!"), StringLength(30, MinimumLength = 3, ErrorMessage = "Username is too short or too long")]
        public string UserName { get; set; }
        //len 12
        [Required, RegularExpression("^[1234567890]+$", ErrorMessage = "Account number is illegall"), StringLength(12, MinimumLength = 12, ErrorMessage = "Account number is too short or too long")]
        public string AccountNumber { get; set; }
        [Required]
        public string CryptedInfo { get; set; }
    }
}
