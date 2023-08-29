using ChatRoomServer.Models;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ChatRoomServer.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IHttpContextAccessor _context;

        public ChatController(ILogger<ChatController> logger, IHttpContextAccessor context)
        {
            _logger = logger;
            _context = context;
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

        // DEBUG
        public IActionResult SendMessage([FromBody]string message)
        {
            _logger.LogInformation(message);
            if (message == null)
                return StatusCode(406, "Null Input");
            return Ok();
        }

        // DEBUG
        public IActionResult GetMessages()
        {
            return Ok();
        }

        // DEBUG
        public IActionResult TestAction(int num1, int num2)
        {
            try
            {
                string s = $"Test 2 Successful for {_context.HttpContext?.User.Identity?.Name}: {num1 + num2}";
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
