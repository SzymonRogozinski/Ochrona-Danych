using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.TransferData
{
    public class PrivateTransferData
    {
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890 .,]+$", ErrorMessage = "Title contains illegal char!")]
        public string Title { get; set; }
        [Required, Range(0, double.MaxValue, ErrorMessage = "Price must be positive!")]
        public double Price { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
    }
}
