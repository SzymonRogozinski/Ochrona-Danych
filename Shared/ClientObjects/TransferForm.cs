using System.ComponentModel.DataAnnotations;

namespace SharedClass.ClientObjects
{
	public class TransferForm
	{
		//len 12
		[Required, RegularExpression("^[1234567890]+$", ErrorMessage = "Account number is not correct!"), StringLength(12, MinimumLength = 12, ErrorMessage = "Account number is too short or too long")]
		public string AdressAccountNum { get; set; }
		//Private info
		[Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ]+$", ErrorMessage = "First name contains illegal char!"), StringLength(50, MinimumLength = 3, ErrorMessage = "First name is too short or too long")]
		public string AdressFirstName { get; set; }
		[Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ ]+$", ErrorMessage = "LastName contains illegal char!"), StringLength(50, MinimumLength = 3, ErrorMessage = "Last name is too short or too long")]
		public string AdressLastName { get; set; }
		[Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890 .,]+$", ErrorMessage = "Title contains illegal char!")]
		public string Title { get; set; }
		[Required, Range(0, double.MaxValue, ErrorMessage = "Price must be positive!")]
		public double Price { get; set; }
		[Required]
		public DateTime TimeStamp { get; set; }
	}
}
