using Microsoft.EntityFrameworkCore;
using ChatRoomServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using ChatRoomServer.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ChatRoomServer.Formatters;

bool runDemo = false;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthorization();
if (builder.Environment.IsDevelopment() && runDemo)
{
    Console.WriteLine("Using development authentication");
    builder.Services.AddAuthentication("Bearer").AddJwtBearer();
}
else
{
    builder.Services.AddAuthentication("Bearer").AddJwtBearer(o =>
    {
        var Key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };
    });
}

builder.Services.AddSingleton<IJWTManagerRepository, JWTManagerRepository>();

builder.Services.AddControllers();
builder.Services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("UserList"));
builder.Services.AddDbContext<MessageContext>(opt => opt.UseInMemoryDatabase("MessageList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc(o => o.InputFormatters.Insert(0, new PlainTextBodyInputFormatter()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseWebSockets();

app.UseRouting();
app.MapControllerRoute(
    name: default,
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAuthorization();
// DEBUG
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}.").RequireAuthorization();
// DEBUG
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            string s = context.User.Identity?.Name ?? "Anonymous";
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await EchoWebSocketManager.Echo(webSocket, s);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next(context);
    }

});

app.MapControllers();

app.Run();