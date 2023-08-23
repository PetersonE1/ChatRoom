using ChatRoomServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatRoomServer.Controllers
{
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly UserContext _context;

        public ChatController(ILogger<ChatController> logger, UserContext context)
        {
            _logger = logger;
            _context = context;
            
            //DEBUG
            User user = new User() { Id = 0, Username = "test", PasswordHash = "test".GetHashCode() };
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public IActionResult Index()
        {
            try
            {
                string s = "Test Successful";
                _logger.LogInformation("Get success");
                return Ok(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside Index action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public IActionResult Authenticate(string username, string password)
        {
            int hash = password.GetHashCode();
            if (_context.Users.Where(n => n.Username == username && n.PasswordHash == hash).Count() > 0)
            {
                return Ok("Successfully logged in!");
            }
            return StatusCode(406, "User not found");
        }

        public IActionResult TestAction(int num1, int num2)
        {
            try
            {
                string s = $"Test 2 Successful: {num1 + num2}";
                _logger.LogInformation("Get success 2");
                return Ok(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside TestAction: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
