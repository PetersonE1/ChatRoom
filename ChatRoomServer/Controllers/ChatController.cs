using ChatRoomServer.Models;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ChatRoomServer.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
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
