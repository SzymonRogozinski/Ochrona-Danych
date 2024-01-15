using System.ComponentModel.DataAnnotations;

namespace SharedClass.ClientObjects
{
	public class UserNameQuestionary
	{
		[Required]
		[RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890@.]{3,60}$", ErrorMessage = "UserName contains illegal char!")]
		public string value { get; set; }

	}
}
