using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.TransferData
{
    public class CryptedTransfer
    {
        public int Id { get; set; }
        //Do kogo
        [Required]
        public int Address { get; set; }
        [Required]
        //Od kogo
        public int Sender { get; set; }
        [Required]
        public string CryptedInfo { get; set; }
    }
}
