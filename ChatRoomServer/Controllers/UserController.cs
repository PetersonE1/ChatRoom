using ChatRoomServer.Models;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatRoomServer.Controllers
{
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

        [AllowAnonymous]
        public IActionResult Register(string username, string password)
        {
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
