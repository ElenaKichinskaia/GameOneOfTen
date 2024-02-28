using GameOneOfTen.DbLayer;
using GameOneOfTen.Models;

namespace GameOneOfTen.BusinessLayer
{
    public class RandomGameProcess : IGameProcess
    {
        private IDataService _dataService;
        public RandomGameProcess(IDataService dataService)
        {
            _dataService = dataService;
        }

        public Player CreatePlayer(Player player) {
            return _dataService.CreatePlayer(player);
        }

        /// <summary>
        /// Implements the logic for making a bet.
        /// </summary>
        /// <param name="bet">The bet to be made.</param>
        /// <returns>The bet history entity if the bet was made successfully; otherwise, null.</returns>
        public BetHistory? MakeBet(Bet bet)
        {
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

        /// <summary>
        /// Implements the logic for retrieving the balance of a player.
        /// </summary>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>The balance of the specified player.</returns>
        public int GetBalance(int playerId)
        {
            return _dataService.GetBalance(playerId);
        }


        /********* Private methods ***********/

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
    }
}
