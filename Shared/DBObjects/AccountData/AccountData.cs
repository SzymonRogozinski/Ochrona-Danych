using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.AccountData
{
    public class AccountData
    {
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890@.]+$", ErrorMessage = "UserName contains illegal char!"), StringLength(30, MinimumLength = 3, ErrorMessage = "Username is too short or too long")]
        public string UserName { get; set; }
        //len 12
        [Required, RegularExpression("^[1234567890]+$", ErrorMessage = "Account number is not correct!"), StringLength(12, MinimumLength = 12, ErrorMessage = "Account number is too short or too long")]
        public string AccountNumber { get; set; }
        //Private info
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ]+$", ErrorMessage = "First name contains illegal char!"), StringLength(50, MinimumLength = 3, ErrorMessage = "First name is too short or too long")]
        public string FirstName { get; set; }
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ ]+$", ErrorMessage = "LastName contains illegal char!"), StringLength(50, MinimumLength = 3, ErrorMessage = "Last name is too short or too long")]
        public string LastName { get; set; }
        [Required]
        public double Balance { get; set; }
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890@.]+$", ErrorMessage = "Email contains illegal char!")]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }
        [Required, RegularExpression("^[1234567890]+$", ErrorMessage = "Pesel is not correct!"), StringLength(11, MinimumLength = 11, ErrorMessage = "Pesel is too short or too long")]
        public string Pesel { get; set; }
        //Card info
        [RegularExpression("^[1234567890]+$", ErrorMessage = "Credit Card Number is not correct!"), StringLength(16, MinimumLength = 16, ErrorMessage = "Credit Card Number is too short or too long")]
        public string CardNumber { get; set; } //length=16
        public string CardExp { get; set; } //MM/YY
        [Range(111, 9999, ErrorMessage = "Card CSC is not correct!")]
        public int CardCSC { get; set; } //length=3 or 4
        //Password
        [Required]
        public Password[] Passwords { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        public AccountData() { }

        public AccountData(string userName, string accountNum, PrivateData pd)
        {
            UserName = userName;
            AccountNumber = accountNum;
            FirstName = pd.FirstName;
            LastName = pd.LastName;
            Balance = pd.Balance;
            Email = pd.Email;
            Role = pd.Role;
            Pesel = pd.Pesel;
            CardNumber = pd.CardNumber;
            CardExp = pd.CardExp;
            CardCSC = pd.CardCSC;
            Passwords = pd.Passwords;
            PasswordSalt = pd.PasswordSalt;
        }

    }

    public static class Roles
    {
        public static readonly string ADMIN = "admin";
        public static readonly string CUSTOMER = "customer";
    }
}
