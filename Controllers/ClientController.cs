using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class ClientController(IClientService service, ILogger logger) : ControllerBase
{
    private readonly IClientService _service = service;
    private readonly ILogger _logger = logger;

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
            var token = _service.GenerateJwtToken(client);

            return Ok(new
            {
                message = "Login successful",
                token = token,
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
}
