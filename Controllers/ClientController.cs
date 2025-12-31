using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class ClientController(IClientService service, ILogger logger, IConfiguration configuration) : ControllerBase
{
    private readonly IClientService _service = service;
    private readonly ILogger _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientDto clientDto)
    {
        try
        {
            return Ok(await _service.RegisterClient(clientDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // TODO: estudiar el correcto funcionamiento y configuración de los JWT  
    [HttpPost("login")]
    public async Task<IActionResult> LoginClient([FromBody] LoginClientDto clientDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var client = await _service.LoginClient(clientDto);

            if (client == null)
                return Unauthorized(new { message = "Invalid credentials" }); // ✅ 401

            // ✅ Generar token JWT (esto es lo que falta)
            var token = GenerateJwtToken(client);

            return Ok(new
            {
                message = "Login successful",
                token,
                user = new { client.Id, client.Name }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    private string GenerateJwtToken(Client client)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT_KEY"] ?? "fallback_key_32_chars_long_123456");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
                new Claim(ClaimTypes.Name, client.Name),
                new Claim("client_id", client.Id.ToString())
            ]),

            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration[Env.GetString("JWT_EXPIRE_HOURS")] ?? "24")),
            Issuer = _configuration[Env.GetString("JWT_ISSUER")],
            Audience = _configuration[Env.GetString("JWT_AUDIENCE")],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
