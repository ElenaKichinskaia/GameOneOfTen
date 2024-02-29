using GameOneOfTen.BusinessLayer;
using GameOneOfTen.DbLayer;
using GameOneOfTen.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Cryptography;
using System.Text;

namespace GameOneOfTen.Tests
{
    [TestFixture]
    public class GameProcessTests
    {
        private Mock<RandomGameProcess> _gameProcess;
        private MockDataService _dataService;
        private Mock<ILogger<RandomGameProcess>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _dataService = new MockDataService();
            _mockLogger = new Mock<ILogger<RandomGameProcess>>();
            _gameProcess = new Mock<RandomGameProcess>(_mockLogger.Object, _dataService);            
        }

        #region IsPlayerExists Tests

        [Test]
        public void IsPlayerExists_ReturnId_ExistingPlayer()
        {
            // Preparation of data
            string username = "existingUser";
            string password = "correctPassword";
            var player = _gameProcess.Object.CreatePlayer(username, password);

            // Execution of the test
            int? playerId = _gameProcess.Object.IsPlayerExists(username, password);

            // Verification of the result
            Assert.AreEqual(playerId, player.Id, "The method should return the player's Id for an existing player.");
        }

        [Test]
        public void IsPlayerExists_ReturnNull_NonExistingPlayer()
        {
            // Preparation of data
            string username = "nonExistingUser";
            string password = "password";

            // Execution of the test
            int? playerId = _gameProcess.Object.IsPlayerExists(username, password);

            // Verification of the result
            Assert.IsNull(playerId, "The method should return null for a non-existing player.");
        }

        [Test]
        public void IsPlayerExists_ReturnNull_EmptyInput()
        {
            // Preparation of data
            string username = "";
            string password = "";

            // Execution of the test
            int? playerId = _gameProcess.Object.IsPlayerExists(username, password);

            // Verification of the result
            Assert.IsNull(playerId, "The method should return null for empty input.");
        }

        [Test]
        public void IsPlayerExists_ReturnNull_IncorrectPassword()
        {
            // Preparation of data
            string username = "existingUser";
            string correctPassword = "password";
            string incorrectPassword = "incorrectPassword";

            var player = _gameProcess.Object.CreatePlayer(username, correctPassword);

            // Execution of the test
            int? playerId = _gameProcess.Object.IsPlayerExists(username, incorrectPassword);

            // Verification of the result
            Assert.IsNull(playerId, "The method should return null for an incorrect password.");
        }

        [Test]
        public void IsPlayerExists_ReturnNull_IncorrectUsername()
        {
            // Preparation of data
            string correctUsername = "existingUser";
            string incorrectUsername = "nonExistingUser";
            string password = "password";

            var player = _gameProcess.Object.CreatePlayer(correctUsername, password);

            // Execution of the test
            int? playerId = _gameProcess.Object.IsPlayerExists(incorrectUsername, password);

            // Verification of the result
            Assert.IsNull(playerId, "The method should return null for an incorrect username.");
        }

        [Test]
        public void IsPlayerExists_ReturnNull_ExtremeCasesLong()
        {
            // Preparation of data
            string longUsername = new string('a', 1000); // Very long username
            string longPassword = new string('b', 1000); // Very long password

            // Execution of the tests
            int? playerIdLong = _gameProcess.Object.IsPlayerExists(longUsername, longPassword);

            // Verification of the results
            Assert.IsNull(playerIdLong, "The method should handle an extreme case of very long input.");
        }

        [Test]
        public void IsPlayerExists_ReturnNull_ExtremeCasesShort()
        {
            // Preparation of data
            string shortUsername = ""; // Very short username
            string shortPassword = ""; // Very short password

            // Execution of the tests
            int? playerIdShort = _gameProcess.Object.IsPlayerExists(shortUsername, shortPassword);

            // Verification of the results
            Assert.IsNull(playerIdShort, "The method should handle an extreme case of very short input.");
        }

        /*
        [Test]
        public void IsPlayerExists_ReturnNull_SqlInjectionProtection()
        {
            // Preparation of malicious input with SQL injection
            string maliciousUsername = "'; DROP TABLE Players; --";
            string password = "password123";

            // Execution of the test
            int? playerId = _gameProcess.IsPlayerExists(maliciousUsername, password);

            // Verification that the method returns null because no player exists with such username
            Assert.IsNull(playerId, "The IsPlayerExists method should return null for malicious input with SQL injection.");
        }
        */

        #endregion

        #region CreatePlayer Tests

        [Test]
        public void TestCreatePlayer_Successful()
        {
            // Preparation of data
            string username = "newUser";
            string password = "password";
            Player expectedPlayer = new Player { Login = username, Password = HashPassword(password), Balance = Constants.DefaultStartBalance };

            _dataService.DeleteAllPlayers();

            // Execution of the test
            Player createdPlayer = _gameProcess.Object.CreatePlayer(username, password);

            // Verification of the result
            Assert.IsNotNull(createdPlayer, "The method should return the newly created player.");
            Assert.AreEqual(expectedPlayer.Login, createdPlayer.Login, "The created player's username should match the input.");
            Assert.AreEqual(expectedPlayer.Password, createdPlayer.Password, "The created player's password should match the input.");
            Assert.AreEqual(Constants.DefaultStartBalance, createdPlayer.Balance, "The created player's balance should match the default start balance.");
        }

        [Test]
        public void TestCreatePlayer_PlayerAlreadyExists()
        {
            // Preparation of data
            string username = "newUser";
            string password = "password";
            Player createdPlayer = _gameProcess.Object.CreatePlayer(username, password);

            // Execution of the test
            Player newPlayer = _gameProcess.Object.CreatePlayer(username, password);

            // Verification of the result
            Assert.IsNull(newPlayer, "The method should return null when the username already exists.");
        }

        [Test]
        public void TestCreatePlayer_EmptyInput()
        {
            // Preparation of data
            string username = "";
            string password = "";

            // Execution of the test
            Player createdPlayer = _gameProcess.Object.CreatePlayer(username, password);

            // Verification of the result
            Assert.IsNull(createdPlayer, "The method should return null for empty input.");
        }

        #endregion

        #region MakeBet Tests

        [Test]
        public void TestMakeBet_ReturnNull_NullInput()
        {
            // Execution of the test
            BetHistory? betHistory = _gameProcess.Object.MakeBet(null);

            // Verification of the result
            Assert.IsNull(betHistory, "The method should return null when input value is null.");
        }

        // Test for Number within the range [0, 9]
        [TestCase(0)]
        [TestCase(5)]
        [TestCase(9)]
        public void TestMakeBet_NumberInRange(int number)
        {
            // Preparation of data
            string username = "newUser";
            string password = "password";
            _dataService.DeleteAllPlayers();
            var player = _gameProcess.Object.CreatePlayer(username, password);

            var bet = new Bet { PlayerId = player.Id, Value = 50, Number = number };

            // Execution of the test
            BetHistory? betHistory = _gameProcess.Object.MakeBet(bet);

            // Verification of the result
            Assert.IsNotNull(betHistory, $"The method should accept the number {number} within the range [0, 9].");
        }

        // Test for Number less than 0
        [Test]
        public void TestMakeBet_NumberLessThanZero()
        {
            // Preparation of data
            string username = "newUser";
            string password = "password";
            _dataService.DeleteAllPlayers();
            var player = _gameProcess.Object.CreatePlayer(username, password);

            var bet = new Bet { PlayerId = player.Id, Value = 50, Number = -1 };

            // Execution of the test
            BetHistory? betHistory = _gameProcess.Object.MakeBet(bet);

            // Verification of the result
            Assert.IsNull(betHistory, "The method should reject the number less than 0.");
        }

        // Test for Number greater than 9
        [Test]
        public void TestMakeBet_NumberGreaterThanNine()
        {
            // Preparation of data
            string username = "newUser";
            string password = "password";
            _dataService.DeleteAllPlayers();
            var player = _gameProcess.Object.CreatePlayer(username, password);

            var bet = new Bet { PlayerId = player.Id, Value = 50, Number = 10 };

            // Execution of the test
            BetHistory? betHistory = _gameProcess.Object.MakeBet(bet);

            // Verification of the result
            Assert.IsNull(betHistory, "The method should reject the number greater than 9.");
        }

        [Test]
        public void TestMakeBet_IncreasedBalance_WonBet()
        {
            // Preparation of data
            _dataService.DeleteAllPlayers();
            string username = "newUser";
            string password = "password";
            Player player = _gameProcess.Object.CreatePlayer(username, password);
            Bet bet = new Bet
            {
                PlayerId = player.Id,
                Number = 5,
                Value = 1
            };
            var betResult = BetStatus.Lost;
            var expectedWinning = bet.Value * Constants.WinTimesCount;

            // Execution of the test
            while (betResult != BetStatus.Won)
            {
                player.Balance = Constants.DefaultStartBalance;
                var historyRecord = _gameProcess.Object.MakeBet(bet);
                if (historyRecord != null)
                {
                    betResult = historyRecord.Result;
                }
            }

            // Verification of the result
            Assert.AreEqual(player.Balance, (Constants.DefaultStartBalance + expectedWinning), "The balance should be bigger on the bet.");
        }

        [Test]
        public void TestMakeBet_DecreasedBalance_LostBet()
        {
            // Preparation of data
            _dataService.DeleteAllPlayers();
            string username = "newUser";
            string password = "password";
            Player player = _gameProcess.Object.CreatePlayer(username, password);
            Bet bet = new Bet
            {
                PlayerId = player.Id,
                Number = 5,
                Value = 1
            };
            var betResult = BetStatus.Won;
            var expectedLoss = bet.Value;

            // Execution of the test
            while (betResult != BetStatus.Lost)
            {
                player.Balance = Constants.DefaultStartBalance;
                var historyRecord = _gameProcess.Object.MakeBet(bet);
                if (historyRecord != null)
                {
                    betResult = historyRecord.Result;
                }
            }

            // Verification of the result
            Assert.AreEqual(player.Balance, (Constants.DefaultStartBalance - expectedLoss), "The balance should be bigger on the bet.");
        }

        #endregion

        #region Private methods

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