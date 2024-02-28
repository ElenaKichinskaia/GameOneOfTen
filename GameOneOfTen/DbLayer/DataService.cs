using GameOneOfTen.Models;

namespace GameOneOfTen.DbLayer
{
    public class DataService : IDataService
    {
        private readonly ApplicationContext _context;

        public DataService(ApplicationContext ctx)
        {
            _context = ctx;
        }

        public Player CreatePlayer(Player player) {

            var newPlayer = _context.Players.Add(player);
            _context.SaveChanges();

            return newPlayer.Entity;
        }

        public bool DeletePlayer(int playerId)
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

        public int GetBalance(int playerId)
        {
            var player = _context.Players.Find(playerId);
            return player != null ? player.Balance : -1;
        }

        public ICollection<BetHistory> GetBetHistories(int playerId)
        {
            return _context.BetHistories.Where(x => x.PlayerId == playerId).ToList();
        }

        public BetHistory? CreateBet(BetHistory bet)
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
    }
}
