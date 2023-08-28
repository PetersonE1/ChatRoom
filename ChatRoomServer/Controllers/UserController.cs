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
        public IActionResult Authenticate(string username, string password)
        {
            if (username == null || password == null)
                return StatusCode(406, "Invalid input");
            int hash = password.GetHashCode();
            if (_context.Users.Where(n => n.Username == username && n.PasswordHash == hash).Count() > 0)
            {
                Tokens token = _jwtManager.GenerateToken(username);
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

        // DEBUG
        [AllowAnonymous]
        public IActionResult Register2([FromHeader(Name = "user")] string user)
        {
            try
            {
                byte[] base64Encoded = Convert.FromBase64String(user);
                string decoded = Encoding.UTF8.GetString(base64Encoded);
                string[] users = decoded.Split(':');
                return Ok($"Username: {users[0]}, Password: {users[1]}");
            }
            catch
            {
                return StatusCode(500, "Error processing input");
            }
        }

        [AllowAnonymous]
        public IActionResult Register([FromHeader(Name = "username")] string username, [FromHeader(Name = "password")] string password)
        {
            if (username == null || password == null)
                return StatusCode(406, "Invalid input");
            int hash = password.GetHashCode();
            User user = new User() { Username = username, PasswordHash = hash, Id = (_context.Users.LastOrDefault()?.Id ?? -1) + 1 };
            if (_context.Users.Where(n => n.Username == username).Count() > 0)
                return StatusCode(406, "User already in system");
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok("Registered account!");
        }
    }
}
