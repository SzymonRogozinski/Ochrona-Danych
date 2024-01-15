using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.TransferData
{
    public class Transfer
    {
        //Do kogo
        [Required]
        public int Address { get; set; }
        [Required]
        //Od kogo
        public int Sender { get; set; }
        //private
        [Required, RegularExpression("^[a-zA-ZęóąśłżźćńĘÓĄŚŁŻŹĆŃ1234567890 .,]+$", ErrorMessage = "Title contains illegal char!")]
        public string Title { get; set; }
        [Required, Range(0, double.MaxValue, ErrorMessage = "Price must be positive!")]
        public double Price { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }

    }
}
