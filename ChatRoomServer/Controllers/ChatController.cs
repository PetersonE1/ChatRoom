﻿using ChatRoomServer.Models;
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
        private readonly UserContext _context;
        private readonly IJWTManagerRepository _jwtManager;

        public ChatController(ILogger<ChatController> logger, UserContext context, IJWTManagerRepository jwtManager)
        {
            _logger = logger;
            _context = context;
            _jwtManager = jwtManager;
            
            //DEBUG
            /*User user = new User() { Id = 0, Username = "test", PasswordHash = "test".GetHashCode() };
            _context.Users.Add(user);
            _context.SaveChanges();*/
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
            return StatusCode(401, "Username and/or password is incorrect");
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
