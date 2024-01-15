using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.AccountData
{
    public class PrivateData
    {
        //Private info
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ]+$", ErrorMessage = "FirstName contains illegal char!")]
        public string FirstName { get; set; }
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ ]+$", ErrorMessage = "LastName contains illegal char!")]
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


        public PrivateData() { }
        public PrivateData(AccountData ad)
        {
            FirstName = ad.FirstName;
            LastName = ad.LastName;
            Balance = ad.Balance;
            Email = ad.Email;
            Role = ad.Role;
            Pesel = ad.Pesel;
            CardNumber = ad.CardNumber;
            CardExp = ad.CardExp;
            CardCSC = ad.CardCSC;
            Passwords = ad.Passwords;
            PasswordSalt = ad.PasswordSalt;
        }
    }
}
