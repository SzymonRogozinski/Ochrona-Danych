using System.Text.RegularExpressions;

namespace SharedClass
{
	public class PasswordChecker
	{
		private static readonly int CharRange = 112;
		private static double Const = Math.Log2(CharRange);

		//Check if password contains require chars
		private string CheckPasswordChars(string password)
		{
			if (password.Length < 8)
				return "Password is too short!";
			if (!Regex.Match(password, @"\d+", RegexOptions.ECMAScript).Success)
				return "Password do not contain numbers!";
			if (!Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success ||
			  !Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
				return "Password do not contain small or big characters!";
			if (!Regex.Match(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
				return "Password do not contain special symbol!";
			return "Ok";
		}

		private double CheckPasswordStrength(string password)
		{
			int len = password.Length;
			double PasswordPotential = Const * len;
			double entropy = 0.0;
			while (password.Length > 0)
			{
				char find = password[0];
				int charOccurence = password.ToCharArray().Count(c => c == find);
				password = password.Replace(find + "", "");
				double propability = (double)charOccurence / len;
				entropy += propability * Math.Log2(propability);
			}
			//Entropy modyfied by proportion of password entropy and Max. password entropy
			double proportion = -entropy / Math.Log2(len);
			double result = PasswordPotential * proportion;
			return result;
		}

		private string PasswordStrengthValue(double entropy)
		{
			if (entropy < 40)
			{
				return PasswordStrength.VERY_WEAK;
			}
			else if (entropy < 60)
			{
				return PasswordStrength.WEAK;
			}
			else if (entropy < 90)
			{
				return PasswordStrength.MEDIUM;
			}
			else if (entropy < 120)
			{
				return PasswordStrength.STRONG;
			}
			else
			{
				return PasswordStrength.VERY_STRONG;
			}
		}

		public ServiceResponse<string> FullPasswordCheck(string password)
		{
			string CharContainTest = CheckPasswordChars(password);
			if (CharContainTest != "Ok")
			{
				return new ServiceResponse<string>()
				{
					Success = false,
					Message = CharContainTest
				};
			}

			double PassStr = CheckPasswordStrength(password);
			string PassStrVal = PasswordStrengthValue(PassStr);
			if (PassStrVal == PasswordStrength.VERY_WEAK)
			{
				return new ServiceResponse<string>()
				{
					Success = false,
					Message = "Password is too weak!"
				};
			}
			else
			{
				return new ServiceResponse<string>()
				{
					Success = true,
					Message = "Ok",
					Data = PassStrVal
				};
			}
		}
	}

	public static class PasswordStrength
	{
		//Below 40
		public static readonly string VERY_WEAK = "Very weak";
		//Below 60
		public static readonly string WEAK = "Weak";
		//Below 90
		public static readonly string MEDIUM = "Medium";
		//Below 120
		public static readonly string STRONG = "Strong";
		//120 and more
		public static readonly string VERY_STRONG = "Very strong";
	}
}
