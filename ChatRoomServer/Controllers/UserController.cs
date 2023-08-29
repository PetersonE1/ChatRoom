using ChatRoomServer.Models;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ChatRoomServer.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly UserContext _context;
        private readonly IJWTManagerRepository _jwtManager;

        public UserController(ILogger<ChatController> logger, UserContext context, IJWTManagerRepository jwtManager)
        {
            _logger = logger;
            _context = context;
            _jwtManager = jwtManager;

            //DEBUG
            /*User user = new User() { Id = 0, Username = "test", PasswordHash = "test".GetHashCode() };
            _context.Users.Add(user);
            _context.SaveChanges();*/
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return Ok("Connection successful");
        }

        [AllowAnonymous]
        public IActionResult Authenticate([FromHeader(Name = "Credentials")] string credentials)
        {
            if (credentials == null)
                return StatusCode(406, "Null Input");
            byte[] base64Encoded = Convert.FromBase64String(credentials);
            string decoded = Encoding.UTF8.GetString(base64Encoded);

            if (!decoded.Contains(':'))
                return StatusCode(406, "Invalid Input");
            string[] credentialsArray = decoded.Split(':');

            if (credentialsArray[0].Length == 0 || credentialsArray[1].Length == 0)
                return StatusCode(406, "Invalid Input");
            int hash = credentialsArray[1].GetHashCode();
            if (_context.Users.Where(n => n.Username == credentialsArray[0] && n.PasswordHash == hash).Count() > 0)
            {
                Tokens token = _jwtManager.GenerateToken(credentialsArray[0]);
                return Ok(token);
            }
            return StatusCode(406, "Username and/or password is incorrect");
        }

        // DEBUG
        [AllowAnonymous]
        public IActionResult Encode(string username, string password)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes($"{username}:{password}");
            return Ok(Convert.ToBase64String(plainTextBytes));
        }

        [AllowAnonymous]
        public IActionResult Register([FromHeader(Name = "Credentials")] string credentials)
        {
            if (credentials == null)
                return StatusCode(406, "Null Input");
            byte[] base64Encoded = Convert.FromBase64String(credentials);
            string decoded = Encoding.UTF8.GetString(base64Encoded);

            if (!decoded.Contains(':'))
                return StatusCode(406, "Invalid Input");
            string[] credentialsArray = decoded.Split(':');

            if (credentialsArray[0].Length == 0 || credentialsArray[1].Length == 0)
                return StatusCode(406, "Invalid Input");
            int hash = credentialsArray[1].GetHashCode();
            User user = new User() { Username = credentialsArray[0], PasswordHash = hash, Id = (_context.Users.LastOrDefault()?.Id ?? -1) + 1 };
            if (_context.Users.Where(n => n.Username == credentialsArray[0]).Count() > 0)
                return StatusCode(406, "User already in system");
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Registered account!");
        }
    }
}
