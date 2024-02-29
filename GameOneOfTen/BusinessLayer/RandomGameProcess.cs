using GameOneOfTen.Controllers;
using GameOneOfTen.DbLayer;
using GameOneOfTen.Models;

namespace GameOneOfTen.BusinessLayer
{
    /// <summary>
    /// Class responsible for managing the game process, handling functions such as making bets and 
    /// retrieving player balances, as defined by the provided interface.
    /// </summary>
    public class RandomGameProcess : IGameProcess
    {
        private IDataService _dataService;
        private readonly ILogger<RandomGameProcess> _logger;

        public RandomGameProcess(ILogger<RandomGameProcess> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        /// <summary>
        /// Checks if a player with the provided username and password exists in the system.
        /// </summary>
        /// <param name="userName">The username of the player.</param>
        /// <param name="password">The password of the player.</param>
        /// <returns>The player's Id if a matching player is found, otherwise null.</returns>
        public int? IsPlayerExists(string userName, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return null;
                }
                return _dataService.IsPlayerExists(userName, password);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while checking if player with userName {userName} exists: {ex.Message}");
                throw; 
            }
        }

        /// <summary>
        /// Creates a new player account with the provided username and password.
        /// </summary>
        /// <param name="userName">The username of the new player.</param>
        /// <param name="password">The password of the new player.</param>
        /// <returns>The newly created player object.</returns>
        public Player CreatePlayer(string userName, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) ||
                _dataService.IsPlayerNameExists(userName))
                {
                    return null;
                }
                return _dataService.CreatePlayer(userName, password, Constants.DefaultStartBalance);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"An error occurred while creating player with userName {userName}: {ex.Message}");
                throw; 
            }
        }

        /// <summary>
        /// Implements the logic for making a bet.
        /// </summary>
        /// <param name="bet">The bet to be made.</param>
        /// <returns>The bet history entity if the bet was made successfully; otherwise, null.</returns>
        public BetHistory? MakeBet(Bet bet)
        {
            try
            {
                if (bet == null || bet.Number < Constants.StartRangeOfNumbers || bet.Number > Constants.EndRangeOfNumbers)
                {
                    return null;
                }
                var balance = _dataService.GetBalance(bet.PlayerId);
                if (balance >= bet.Value)
                {
                    var currentNumber = GenerateNumber(Constants.StartRangeOfNumbers, Constants.EndRangeOfNumbers);
                    var betHistoryRecord = new BetHistory()
                    {
                        Number = bet.Number,
                        Value = currentNumber == bet.Number ? bet.Value * Constants.WinTimesCount : -bet.Value,
                        Result = currentNumber == bet.Number ? BetStatus.Won : BetStatus.Lost,
                        PlayerId = bet.PlayerId,
                        Date = DateTime.Now
                    };
                    return _dataService.CreateBet(betHistoryRecord);
                }
                return null;
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
        /// Generates a random number within the specified range of numbers.
        /// </summary>
        /// <param name="fromRange">The starting value of the range.</param>
        /// <param name="toRange">The ending value of the range.</param>
        /// <returns>The generated random number within the specified range.</returns>
        private int GenerateNumber(int fromRange, int toRange)
        {
            // Create a new instance of the Random class
            Random rnd = new Random();

            // Generate a random number within the specified range
            int num = rnd.Next(fromRange, toRange + 1);

            // Return the generated random number
            return num;
        }

        #endregion
    }
}
