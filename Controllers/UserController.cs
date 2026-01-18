using Microsoft.AspNetCore.Mvc;

namespace MiniMazErpBack;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IJwtService jwtService, ILogger<UserController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger _logger = logger;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userDto)
    {
        try
        {
            return Ok(await _userService.RegisterUser(userDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var user = await _userService.LoginUser(userDto);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" }); // 401

            var token = _jwtService.GenerateJwtToken(user);

            return Ok(new
            {
                message = "Login successful",
                token,
                user = new { user.Id, user.Name }
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
