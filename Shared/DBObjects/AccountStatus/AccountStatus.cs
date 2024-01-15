using System.ComponentModel.DataAnnotations;

namespace Shared.DBObjects.AccountStatus
{
    public class AccountStatus
    {
        public static readonly int MaxTrials = 3;
        public static readonly TimeSpan BlockTime = new TimeSpan(0, 5, 0);

        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public int HowManyTrials { get; set; }
        [Required]
        public string Status { get; set; }
    }

    public static class Statuses
    {
        public static readonly string OK = "ok";
        public static readonly string PASSWORDS_TRIALS_OUT = "out of trials";
        public static readonly string PASSWORD_CHANGED = "pass changed";
        public static readonly string BANNED = "ban";

    }
}
