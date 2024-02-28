using GameOneOfTen.Models;
using System.Net.WebSockets;

namespace GameOneOfTen.DbLayer
{
    public interface IDataService
    {
        public Player CreatePlayer(Player player);

        public bool DeletePlayer(int playerId);

        public BetHistory? CreateBet(BetHistory bet);

        public ICollection<BetHistory> GetBetHistories(int playerId);

        public int GetBalance(int playerId);
    }

    public class MockDataService : IDataService
    {
        private Dictionary<String, int> playerPoints = new Dictionary<String, int>();

        public BetHistory? CreateBet(BetHistory bet)
        {
            throw new NotImplementedException();
        }

        public Player CreatePlayer(Player player)
        {
            var login = player.Login
            if (playerPoints.ContainsKey(login))
            {
                throw new ArgumentException("Player already exists";
            }
            playerPoints[login] = 10000;
            return new Player ;
        }

        public bool DeletePlayer(int playerId)
        {
            return playerPoints.Remove
        }

        public int GetBalance(int playerId)
        {
            throw new NotImplementedException();
        }

        public ICollection<BetHistory> GetBetHistories(int playerId)
        {
            throw new NotImplementedException();
        }
    }
}
