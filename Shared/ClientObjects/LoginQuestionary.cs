using System.ComponentModel.DataAnnotations;

namespace SharedClass.ClientObjects
{
	public class LoginQuestionary
	{
		[Required, RegularExpression("^[ *]+$", ErrorMessage = "UserName contains illegal char!"), StringLength(60, MinimumLength = 8, ErrorMessage = "Template is too short or too long")]

		public string template { get; set; }
		[Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890*@.]+$", ErrorMessage = "Password contains illegal char!"), StringLength(60, MinimumLength = 8, ErrorMessage = "Password is too short or too long")]

		public string password { get; set; } //Legal chars for passwords a-zA-z + polish + numbers
		[Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890@.]+$", ErrorMessage = "UserName contains illegal char!"), StringLength(30, MinimumLength = 3, ErrorMessage = "Username is too short or too long")]
		public string userName { get; set; }

	}
}
