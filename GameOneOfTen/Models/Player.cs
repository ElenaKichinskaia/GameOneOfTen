namespace GameOneOfTen.Models
{
    /// <summary>
    /// Class describing the player
    /// </summary>
    public class Player
    {
        public int Id { get; set; }

        public required string Login { get; set; }

        public required string Password { get; set; }

        /// <summary>
        /// Current player balance
        /// </summary>
        public required int Balance { get; set; }
    }
}
