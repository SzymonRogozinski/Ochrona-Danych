using Shared.DBObjects.AccountData;
using System.ComponentModel.DataAnnotations;

namespace SharedClass.ClientObjects
{
	public class AccountInfo
	{
		[Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890@.]+$", ErrorMessage = "UserName contains illegal char!"), StringLength(30, MinimumLength = 3, ErrorMessage = "Username is too short or too long")]
		public string UserName { get; set; }
		//len 12
		[Required, RegularExpression("^[1234567890]+$", ErrorMessage = "Account number is not correct!"), StringLength(12, MinimumLength = 12, ErrorMessage = "Account number is too short or too long")]
		public string AccountNumber { get; set; }
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
		public string Pesel { get; set; }
		//Card info
		[RegularExpression("^[1234567890]+$", ErrorMessage = "Credit Card Number is not correct!"), StringLength(16, MinimumLength = 16, ErrorMessage = "Credit Card Number is too short or too long")]
		public string CardNumber { get; set; } //length=16
		public string CardExp { get; set; } //MM/YY
		[Range(111, 9999, ErrorMessage = "Card CSC is not correct!")]
		public int CardCSC { get; set; } //length=3 or 4

		public AccountInfo(AccountData data)
		{
			this.UserName = data.UserName;
			this.AccountNumber = data.AccountNumber;
			this.FirstName = data.FirstName;
			this.LastName = data.LastName;
			this.Balance = data.Balance;
			this.Email = data.Email;
			this.Pesel = data.Pesel;
			this.CardNumber = data.CardNumber;
			this.CardExp = data.CardExp;
			this.CardCSC = data.CardCSC;
		}
	}
}
