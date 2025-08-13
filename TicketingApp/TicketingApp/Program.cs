using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingApp.API.Extensions;
using TicketingApp.Application.Tickets.Commands;
using TicketingApp.Domain.Interfaces;
using TicketingApp.Infrastructure.Persistence;
using TicketingApp.Infrastructure.Repositories;
using TicketingApp.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Logging.AddConsole();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => new
            {
                Field = e.Key,
                Errors = e.Value?.Errors.Select(er => er.ErrorMessage)
            });

        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(errors));

        return new BadRequestObjectResult(context.ModelState);
    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.RegisterAuthentication();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddMediatR(typeof(CreateTicketCommand).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithAuth();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAngular");

app.MapControllers();

app.Run();