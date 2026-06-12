using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmeza.API.DTOs;
using Firmeza.API.Services;
using Firmeza.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Firmeza.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, IEmailService emailService)
    {
        _userManager = userManager;
        _config = config;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized("Credenciales incorrectas");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerarToken(user, roles.FirstOrDefault() ?? "Cliente");

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            NombreCompleto = user.NombreCompleto,
            Rol = roles.FirstOrDefault() ?? "Cliente"
        });
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            NombreCompleto = dto.NombreCompleto
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, "Cliente");
        var token = GenerarToken(user, "Cliente");

        // Enviar correo de bienvenida
        try
        {
            await _emailService.EnviarCorreoAsync(
                dto.Email,
                "Bienvenido a Firmeza",
                $"<h2>Hola {dto.NombreCompleto}!</h2><p>Tu cuenta ha sido creada exitosamente en Firmeza.</p>"
            );
        }
        catch { /* No bloquear el registro si el correo falla */ }

        return Ok(new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            NombreCompleto = user.NombreCompleto,
            Rol = "Cliente"
        });
    }

    private string GenerarToken(ApplicationUser user, string rol)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.NombreCompleto),
            new Claim(ClaimTypes.Role, rol)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
