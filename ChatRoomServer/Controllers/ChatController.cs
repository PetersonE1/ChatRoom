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

        public ChatController(ILogger<ChatController> logger, IHttpContextAccessor context, MessageContext messageContext)
        {
            _logger = logger;
            _context = context;
            _messageContext = messageContext;
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
            Message new_message = new Message()
            {
                Body = message,
                Sender = Request.HttpContext.User.Identity?.Name ?? "Anonymous",
                TimeSent = DateTime.UtcNow,
                Id = GenerateMessageID(32)
            };
            _messageContext.Messages.Add(new_message);
            _messageContext.SaveChanges();
            return Ok();
        }

        // DEBUG
        public IActionResult GetMessages([FromHeader]bool displayID)
        {
            string s = string.Empty;
            foreach (Message message in _messageContext.Messages)
            {
                s += $"[{message.Sender} {message.TimeSent.ToLocalTime().ToShortTimeString()}{(displayID ? $" ({message.Id})" : string.Empty)}] " + message.Body + "\r\n";
            }
            return Ok(s);
        }

        public async Task EstablishConnection()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await ChatWebSocketManager.ProcessRequest(webSocket, HttpContext, _messageContext);
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
