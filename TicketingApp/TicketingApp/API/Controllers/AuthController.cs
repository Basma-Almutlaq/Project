using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketingApp.API.Models;
using TicketingApp.Infrastructure.Settings;

namespace TicketingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, model.Role ?? "User");

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
    if (!result.Succeeded)
        return Unauthorized("Invalid login");

    var user = await _userManager.FindByEmailAsync(model.Email);
    var roles = await _userManager.GetRolesAsync(user!);

    var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user!.Id),
        new Claim(ClaimTypes.Name, user!.UserName!),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, user.Email!)
    };

    foreach (var role in roles)
        authClaims.Add(new Claim(ClaimTypes.Role, role));

    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

    var token = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
        claims: authClaims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    );

    return Ok(new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token),
        expiration = token.ValidTo
    });
    }

}