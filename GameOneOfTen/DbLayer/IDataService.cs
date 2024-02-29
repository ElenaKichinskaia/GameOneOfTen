using GameOneOfTen.Models;
using System.Security.Cryptography;
using System.Text;

namespace GameOneOfTen.DbLayer
{
    public interface IDataService
    {
        /// <summary>
        /// Checks if a player with the provided username and password exists in the database.
        /// </summary>
        /// <param name="userName">The username of the player.</param>
        /// <param name="password">The password of the player.</param>
        /// <returns>The player's Id if a matching player is found, otherwise null.</returns>
        public int? IsPlayerExists(string userName, string password);

        /// <summary>
        /// Checks if a player with the specified username exists in the database.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if a player with the given username exists, otherwise false.</returns>
        public bool IsPlayerNameExists(string userName);

        /// <summary>
        /// Creates a new player account with the provided username, password, and initial balance.
        /// </summary>
        /// <param name="userName">The username of the new player.</param>
        /// <param name="password">The password of the new player.</param>
        /// <param name="balance">The initial balance of the new player.</param>
        /// <returns>The newly created player object.</returns>
        public Player CreatePlayer(string userName, string password, int balance);

        /// <summary>
        /// Deletes a player account with the specified ID from the database.
        /// </summary>
        /// <param name="playerId">The Id of the player account to delete.</param>
        /// <returns>True if the player account was successfully deleted, otherwise false.</returns>
        public bool DeletePlayer(int playerId);

        /// <summary>
        /// Creates a new bet history record.
        /// </summary>
        /// <param name="bet">The bet history object containing the details of the bet.</param>
        /// <returns>The created bet history record, or null if creation fails.</returns>
        public BetHistory? CreateBet(BetHistory bet);

        /// <summary>
        /// Retrieves the collection of bet histories for the player with the specified ID.
        /// </summary>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>The collection of bet histories for the player.</returns>
        public ICollection<BetHistory> GetBetHistories(int playerId);

        /// <summary>
        /// Retrieves the balance of the player with the specified ID.
        /// </summary>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>The balance of the player.</returns>
        public int GetBalance(int playerId);
    }

    /// <summary>
    /// Implementation of the data service using mock data. This implementation is intended for testing purposes.
    /// </summary>
    public class MockDataService : IDataService
    {
        private List<Player> players = new List<Player>();

        public BetHistory? CreateBet(BetHistory bet)
        {
            var player = players.Find(p => p.Id == bet.PlayerId);
            if (player != null)
            {
                player.Balance += bet.Value;
                return bet;
            }
            else
            {
                return null;
            }
        }

        public bool IsPlayerNameExists(string userName) 
        {
            var player = players.FirstOrDefault(x => string.Equals(x.Login, userName));
            return player != null;
        }

        public Player CreatePlayer(string userName, string password, int balance)
        {
            var hashPassword = HashPassword(password);

            var newPlayer = new Player
            {
                Id = players.Count,
                Login = userName,
                Password = hashPassword,
                Balance = balance
            };
            players.Add(newPlayer);

            return newPlayer;
        }

        public bool DeletePlayer(int playerId)
        {
            var player = players.Find(p => p.Id == playerId);
            if (player != null)
            {
                players.Remove(player);
                return true;
            }
            return false;
        }

        public int GetBalance(int playerId)
        {
            var player = players.Find(p => p.Id == playerId);
            return player != null ? player.Balance : -1;
        }

        public ICollection<BetHistory> GetBetHistories(int playerId)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllPlayers() {
            players.Clear();
        }

        public int? IsPlayerExists(string userName, string password)
        {
            var hashPassword = HashPassword(password);

            var player = players.FirstOrDefault(x => string.Equals(x.Login, userName) && string.Equals(x.Password, hashPassword));
            return player != null ? player.Id : null;
        }

        /// <summary>
        /// Hashes the provided password using a cryptographic hashing algorithm.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hashed password.</returns>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
