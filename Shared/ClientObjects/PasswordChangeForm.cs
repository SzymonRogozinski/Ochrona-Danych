using System.ComponentModel.DataAnnotations;

namespace SharedClass.ClientObjects
{
	public class PasswordChangeForm
	{
		//112 posible chars!
		[Required, RegularExpression("^[ -~ęóąśłżźćńĘÓĄŚŁŻŹĆŃ]{8,30}$", ErrorMessage = "Password contains illegal char!")]
		public string oldPassword { get; set; }

		//112 posible chars!
		[Required, RegularExpression("^[ -~ęóąśłżźćńĘÓĄŚŁŻŹĆŃ]{8,30}$", ErrorMessage = "Password contains illegal char!")]
		public string newPassword { get; set; }
		[Compare("newPassword", ErrorMessage = "The passwords do not match")]
		public string confirmPassword { get; set; }
	}
}
