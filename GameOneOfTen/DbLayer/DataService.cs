using GameOneOfTen.Models;
using System.Security.Cryptography;
using System.Text;

namespace GameOneOfTen.DbLayer
{
    public class DataService : IDataService
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<DataService> _logger;

        public DataService(ILogger<DataService> logger, ApplicationContext ctx)
        {
            _logger = logger;
            _context = ctx;
        }

        /// <summary>
        /// Checks if a player with the provided username and password exists in the database.
        /// </summary>
        /// <param name="userName">The username of the player.</param>
        /// <param name="password">The password of the player.</param>
        /// <returns>The player's Id if a matching player is found, otherwise null.</returns>
        public int? IsPlayerExists(string userName, string password)
        {
            try
            {
                var hashPassword = HashPassword(password);

                var player = _context.Players.FirstOrDefault(x => string.Equals(x.Login, userName) && string.Equals(x.Password, hashPassword));
                return player != null ? player.Id : null;
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while checking if player with userName {userName} exists: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Checks if a player with the specified username exists in the database.
        /// </summary>
        /// <param name="userName">The username to check.</param>
        /// <returns>True if a player with the given username exists, otherwise false.</returns>
        public bool IsPlayerNameExists(string userName)
        {
            var player = _context.Players.FirstOrDefault(x => string.Equals(x.Login, userName));
            return player != null;
        }

        /// <summary>
        /// Creates a new player account with the provided username, password, and initial balance.
        /// </summary>
        /// <param name="userName">The username of the new player.</param>
        /// <param name="password">The password of the new player.</param>
        /// <param name="balance">The initial balance of the new player.</param>
        /// <returns>The newly created player object.</returns>
        public Player CreatePlayer(string userName, string password, int balance)
        {
            try
            {
                var hashPassword = HashPassword(password);

                var newPlayer = _context.Players.Add(new Player
                {
                    Login = userName,
                    Password = hashPassword,
                    Balance = balance
                });

                _context.SaveChanges();

                return newPlayer.Entity;
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while creating player with userName {userName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a player account with the specified ID from the database.
        /// </summary>
        /// <param name="playerId">The Id of the player account to delete.</param>
        /// <returns>True if the player account was successfully deleted, otherwise false.</returns>
        public bool DeletePlayer(int playerId)
        {
            try
            {
                var player = _context.Players.Find(playerId);
                if (player != null)
                {
                    _context.BetHistories.RemoveRange(GetBetHistories(playerId));
                    _context.Players.Remove(player);
                    _context.SaveChanges();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while deleting the player with Id {playerId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the balance of the player with the specified ID.
        /// </summary>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>The balance of the player.</returns>
        public int GetBalance(int playerId)
        {
            try
            {
                var player = _context.Players.Find(playerId);
                return player != null ? player.Balance : -1;
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while getting a balance for player with Id {playerId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the collection of bet histories for the player with the specified ID.
        /// </summary>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>The collection of bet histories for the player.</returns>
        public ICollection<BetHistory> GetBetHistories(int playerId)
        {
            try
            {
                return _context.BetHistories.Where(x => x.PlayerId == playerId).ToList();
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while getting a bet history for player with Id {playerId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a new bet history record.
        /// </summary>
        /// <param name="bet">The bet history object containing the details of the bet.</param>
        /// <returns>The created bet history record, or null if creation fails.</returns>
        public BetHistory? CreateBet(BetHistory bet)
        {
            try
            {
                var player = _context.Players.Find(bet.PlayerId);
                if (player != null)
                {
                    player.Balance += bet.Value;
                    _context.Players.Update(player);
                    _context.BetHistories.Add(bet);

                    _context.SaveChanges();

                    return bet;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while making a bet for player with Id {bet.PlayerId}: {ex.Message}");
                throw;
            }
        }


        #region Private methods

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

        #endregion
    }
}
