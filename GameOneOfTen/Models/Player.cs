namespace GameOneOfTen.Models
{
    /// <summary>
    /// Class describing the player
    /// </summary>
    public class Player
    {
        public Player()
        {
            this.BetHistory = new HashSet<BetHistory>();
        }

        public int Id { get; set; }

        public required string Login { get; set; }

        public required string Password { get; set; }

        /// <summary>
        /// Current player balance
        /// </summary>
        public required int Balance { get; set; }

        /// <summary>
        /// Player betting history
        /// </summary>
        public virtual ICollection<BetHistory> BetHistory { get; set; }
    }
}
