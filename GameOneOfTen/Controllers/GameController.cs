using GameOneOfTen.BusinessLayer;
using GameOneOfTen.Models;
using GameOneOfTen.Models.APIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

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
        /// Creates a new player with the provided username and password.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /api/players
        ///     {
        ///         "userName": "example",
        ///         "password": "example password"
        ///     }
        /// </remarks>
        /// <param name="userName">The username of the new player.</param>
        /// <param name="password">The password of the new player.</param>
        /// <returns>A response containing a success message if the account was created successfully.</returns>
        /// <response code="201">Returns the newly created player.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="500">If there was an internal server error.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("players", Name = "CreatePlayer")]
        public ActionResult CreatePlayer(string userName, string password)
        {
            try
            {
                // Here, validate the loginModel against your user store. This example assumes validation is successful.

                var player = _gameProcess.CreatePlayer(userName, password);
                if (player == null)
                {
                    // Log any exceptions
                    _logger.LogError($"Attempted to create a new user with a username that is already taken.");

                    // Return internal server error response
                    return StatusCode(400, "Sorry, the username you selected is already in use. Please choose a different username.");
                }

                return StatusCode(201, new
                {
                    message = "Your account has been successfully created. Please log in to start playing the game."
                });
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"Error occurred while creating of new user with credentials: Login: {userName}: {ex.Message}");

                // Return internal server error response
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Logs in an existing player with the provided credentials.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     POST /api/login
        ///     {
        ///         "userName": "example",
        ///         "password": "example password"
        ///     }
        /// </remarks>
        /// <param name="userName">The username of the player.</param>
        /// <param name="password">The password of the player.</param>
        /// <returns>A response containing an authentication token if login is successful.</returns>
        /// <response code="200">Returns an authentication token if login is successful.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="401">If login fails due to incorrect credentials.</response>
        /// <response code="500">If there was an internal server error.</response>
        [HttpPost("login", Name = "Login")]
        public ActionResult Login(string userName, string password)
        {
            try
            {
                // Here, validate the loginModel against your user store. This example assumes validation is successful.

                var playerId = _gameProcess.IsPlayerExists(userName, password);
                if (playerId == null)
                {
                    // Log any exceptions
                    _logger.LogError($"Attempted login by client with unrecognized credentials: {userName}");

                    // Return internal server error response
                    return StatusCode(401, "Your login attempt was unsuccessful. Please check your credentials and try again.");
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.NameId, playerId.Value.ToString()),
                new Claim("admin", "true"),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyVeryLongAndSecretJwtKeyIWouldntKeepHereInRealApp"));

                var token = new JwtSecurityToken(
                    // issuer: config["Jwt:Issuer"],
                    // audience: config["Jwt:Audience"],
                    issuer: "Jwt:Issuer",
                    audience: "Jwt:Audience",
                    expires: DateTime.UtcNow.AddHours(2),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return StatusCode(200, new
                {
                    message = "Enjoy playing!",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"Error occurred while login of user with credentials: Login: {userName}: {ex.Message}");

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
        ///         "number": 10,
        ///         "points": 50
        ///     }
        /// </remarks>
        /// <param name="betRequest">The bet request containing chosen number, and points to bet.</param>
        /// <returns>
        /// Returns a BetResponse indicating the status of the bet and the points won or lost.
        /// </returns>
        [Authorize]
        [HttpPost("bets", Name = "MakeBet")]
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

                int playerId = GetPlayerIdFromToken(Request.Headers["Authorization"]);

                if (playerId == 0)
                {
                    // Log invalid input
                    _logger.LogWarning($"Invalid credentials: Player's id from the header.");

                    // Return bad request response
                    return BadRequest("Invalid credentials.");
                }

                // Create a new Bet instance from the bet request
                var bet = new Bet()
                {
                    PlayerId = playerId,
                    Number = betRequest.Number,
                    Value = betRequest.Points
                };
                var betRecord = _gameProcess.MakeBet(bet);

                // Create a BetResponse from the bet result
                var betResponse = new BetResponse
                {
                    Account = betRecord.Player.Balance.ToString(),
                    Status = betRecord.Result.GetDisplayName(),
                    Points = betRecord.Value.ToString()
                };

                return new ActionResult<BetResponse>(betResponse);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                _logger.LogError($"Error occurred while making bet: {ex.Message}");

                // Return internal server error response
                return StatusCode(500, "An error occurred while processing your request");
            }
        }


        /******************************** private methods ************************************/

        /// <summary>
        ///  Retrieves the player's Id from the authorization token provided in the request header.        
        ///  This method parses the authorization header to extract the player's Id.
        /// </summary>
        /// <param name="authorizationHeader">Authorization header containing the token</param>
        /// <returns>The player's Id extracted from the token, or 0 if extraction fails or Id is not found</returns>
        private int GetPlayerIdFromToken(string authorizationHeader)
        {
            var handler = new JwtSecurityTokenHandler();
            var authHeader = authorizationHeader.Replace("Bearer ", "");
            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == "nameid").Value;

            return id != null ? int.Parse(id) : 0;
        }
    }
}
