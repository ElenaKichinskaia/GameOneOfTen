using GameOneOfTen.Models;

namespace GameOneOfTen.BusinessLayer
{
    /// <summary>
    /// Interface for managing the game process, including making bets and getting player balances.
    /// </summary>
    public interface IGameProcess
    {
        /// <summary>
        /// Checks if a player with the provided username and password exists in the system.
        /// </summary>
        /// <param name="userName">The username of the player.</param>
        /// <param name="password">The password of the player.</param>
        /// <returns>The player's Id if a matching player is found, otherwise null.</returns>
        public int? IsPlayerExists(string userName, string password);

        /// <summary>
        /// Creates a new player account with the provided username and password.
        /// </summary>
        /// <param name="userName">The username of the new player.</param>
        /// <param name="password">The password of the new player.</param>
        /// <returns>The newly created player object.</returns>
        public Player CreatePlayer(string userName, string password);

        /// <summary>
        /// Makes a bet and returns the bet history entity, or null if the bet cannot be made.
        /// </summary>
        /// <param name="bet">The bet to be made.</param>
        /// <returns>The bet history entity if the bet was made successfully; otherwise, null.</returns>
        public BetHistory? MakeBet(Bet bet);

    }
}
