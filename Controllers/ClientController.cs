using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class ClientController(ClientService service, ILogger<ClientController> logger) : ControllerBase
{
    private readonly ClientService _service = service;
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

    [HttpPost("login")]
    public async Task<IActionResult> LoginClient([FromBody] LoginClientDto clientDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var client = await _service.LoginClient(clientDto);

            if (client == null)
                return Unauthorized(new { message = "Invalid credentials" }); // 401

            var token = _service.GenerateJwtToken(client);

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


}
