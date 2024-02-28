using Moq;
using GameOneOfTen.BusinessLayer;
using GameOneOfTen.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace GameOneOfTen.Tests
{
    public class GameControllerTests
    {
        private Mock<IGameProcess> _mockGameProcess;
        private Mock<ILogger<GameController>> _mockLogger;
        private GameController controller;

        [SetUp]
        public void Setup()
        {
            _mockGameProcess = new Mock<IGameProcess>();
            _mockLogger = new Mock<ILogger<GameController>>();
            controller = new GameController(_mockLogger.Object, _mockGameProcess.Object);
        }

        [Test]
        public void GetCurrentBalance_WithInvalidPlayerId_ReturnsBadRequest()
        {
            {
                // Arrange
                int invalidPlayerId = 0;

                // Act
                var result = controller.GetCurrentBalance(invalidPlayerId);

                // Assert
                Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            }
        }
    }
}