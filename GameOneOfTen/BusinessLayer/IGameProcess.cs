using GameOneOfTen.Models;

namespace GameOneOfTen.BusinessLayer
{
    /// <summary>
    /// Interface for managing the game process, including making bets and getting player balances.
    /// </summary>
    public interface IGameProcess
    {
        public Player CreatePlayer(Player player);

        /// <summary>
        /// Makes a bet and returns the bet history entity, or null if the bet cannot be made.
        /// </summary>
        /// <param name="bet">The bet to be made.</param>
        /// <returns>The bet history entity if the bet was made successfully; otherwise, null.</returns>
        public BetHistory? MakeBet(Bet bet);

        /// <summary>
        /// Gets the balance of the player.
        /// </summary>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>The balance of the player.</returns>
        public int GetBalance(int playerId);

    }
}
