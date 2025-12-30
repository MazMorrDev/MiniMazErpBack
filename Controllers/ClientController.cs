using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class ClientController(ClientService service) : ControllerBase
{
    private readonly ClientService _service = service;

    [HttpPost]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientDto clientDto)
    {
        try
        {
            return Ok(await _service.RegisterClient(clientDto));
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpGet("login")] // Hay q tener en cuenta que tipo de método Http es para el login
    public async Task<IActionResult> LoginClient([FromBody] LoginClientDto clientDto)
    {
        try
        {
            return Ok(await _service.LoginClient(clientDto));
        }
        catch (Exception)
        {
            throw;
        }
    }
}
