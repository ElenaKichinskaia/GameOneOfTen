using GameOneOfTen.BusinessLayer;
using GameOneOfTen.Models;
using GameOneOfTen.Models.APIModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace GameOneOfTen.Controllers
{
    /// <summary>
    /// Controller for managing the game of chance, where players predict a random number and wager points.
    /// </summary>
    /// <remarks>
    /// Game Rules:
    /// - Players predict a random number between 0 and 9.
    /// - Each player starts with 10,000 points in their account.
    /// - Players can wager any number of points on their prediction.
    /// - If the prediction is correct, players win 9 times their wager.
    /// - If the prediction is incorrect, players lose their wagered points.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        private readonly IGameProcess _gameProcess;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging.</param>
        /// <param name="gameProcess">The game process service for managing game operations.</param>
        public GameController(ILogger<GameController> logger, IGameProcess gameProcess)
        {
            _logger = logger;
            _gameProcess = gameProcess;
        }

        /// <summary>
        /// Retrieves the current balance of a player.
        /// </summary>
        /// <remarks>
        /// Sample request: GET /balance/1
        /// </remarks>
        /// <param name="playerId">The Id of the player.</param>
        /// <returns>
        /// Returns the current balance of the player if successful.
        /// If the player ID is invalid, returns a BadRequest response with an error message.
        /// If an error occurs during processing, returns a StatusCode 500 response with an error message.
        /// </returns>
        [HttpGet("balance/{playerId}", Name = "GetCurrentBalance")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public ActionResult<int> GetCurrentBalance(int playerId)
        {
            try
            {
                // Validate input parameters
                if (playerId <= 0)
                {
                    // Log invalid input
                    _logger.LogWarning($"Invalid player ID: {playerId}");

                    // Return bad request response
                    return BadRequest("Invalid player ID");
                }
                return _gameProcess.GetBalance(playerId);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"Error occurred while retrieving balance for player with Id {playerId}: {ex.Message}");

                // Return internal server error response
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Makes a bet for the specified player.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /bet
        ///     {
        ///         "playerId": 1,
        ///         "number": 10,
        ///         "points": 50
        ///     }
        /// </remarks>
        /// <param name="betRequest">The bet request containing player Id, chosen number, and points to bet.</param>
        /// <returns>
        /// Returns a BetResponse indicating the status of the bet and the points won or lost.
        /// </returns>
        [HttpPost("bet", Name = "MakeBet")]
        [ProducesResponseType(typeof(BetResponse), 200)]
        public ActionResult<BetResponse> MakeBet(BetRequest betRequest)
        {
            try
            {
                // Validate input parameters

                if (betRequest == null)
                {
                    // Log invalid input
                    _logger.LogWarning("Null bet request received");

                    // Return bad request response
                    return BadRequest("Bet request cannot be null.");
                }
                
                if (betRequest.PlayerId <= 0)
                {
                    // Log invalid input
                    _logger.LogWarning($"Invalid player Id: {betRequest.PlayerId}");

                    // Return bad request response
                    return BadRequest("Player Id must be a positive integer.");
                }

                if (betRequest.Number < 0 || betRequest.Number > 9)
                {
                    // Log invalid input
                    _logger.LogWarning($"Invalid bet number: {betRequest.Number}");

                    // Return bad request response
                    return BadRequest("Bet number must be an integer between 0 and 9 (inclusive).");
                }

                if (betRequest.Points <= 0)
                {
                    // Log invalid input
                    _logger.LogWarning($"Invalid bet points: {betRequest.Points}");

                    // Return bad request response
                    return BadRequest("Points must be a positive integer.");
                }

                // Create a new Bet instance from the bet request
                var bet = new Bet()
                {
                    PlayerId = betRequest.PlayerId,
                    Number = betRequest.Number,
                    Value = betRequest.Points
                };
                var betRecord = _gameProcess.MakeBet(bet);

                // Create a BetResponse from the bet result
                var betResponse = new BetResponse
                {
                    PlayerId = betRequest.PlayerId,
                    Status = betRecord.Result.GetDisplayName(),
                    Points = betRecord.Value
                };

                return new ActionResult<BetResponse>(betResponse);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"Error occurred while making bet for player with Id {betRequest.PlayerId}: {ex.Message}");

                // Return internal server error response
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
