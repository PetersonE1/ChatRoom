using ChatRoomServer.Models;
using ChatRoomServer.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChatRoomServer.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IHttpContextAccessor _context;
        private readonly MessageContext _messageContext;
        private readonly IConfiguration _configuration;

        public ChatController(ILogger<ChatController> logger, IHttpContextAccessor context, MessageContext messageContext, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _messageContext = messageContext;
            _configuration = configuration;
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

        public async Task EstablishConnection()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await ChatWebSocketManager.ProcessRequest(webSocket, HttpContext, _messageContext, _configuration);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        internal static string GenerateMessageID(int string_length)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                string date = DateTime.UtcNow.ToShortDateString();
                var bit_count = (string_length * 6);
                var byte_count = ((bit_count + 7) / 8); // rounded up
                var bytes = new byte[byte_count];
                rng.GetBytes(bytes);
                return date + "-" + Convert.ToBase64String(bytes);
            }
        }
    }
}
