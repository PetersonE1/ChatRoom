using Microsoft.EntityFrameworkCore;
using ChatRoomServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

builder.Services.AddControllers();
builder.Services.AddDbContext<UserContext>(opt => opt.UseInMemoryDatabase("UserList"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllerRoute(
    name: default,
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseAuthorization();
app.MapGet("/secret", (ClaimsPrincipal user) => $"Hello {user.Identity?.Name}.").RequireAuthorization();

app.MapControllers();

app.Run();
