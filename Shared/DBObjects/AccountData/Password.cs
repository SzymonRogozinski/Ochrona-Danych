using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DBObjects.AccountData
{
    public class Password
    {
        [Required]
        public byte[] PasswordValue { get; set; }
        // "*"->blocked " "->write char
        [Required]
        public string PasswordTempalte { get; set; }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(PasswordValue) + "?" + PasswordTempalte;
        }
    }
}
