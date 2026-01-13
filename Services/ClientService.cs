using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DotNetEnv;

namespace MiniMazErpBack;

public class ClientService(AppDbContext context, ILogger<ClientService> logger) : IClientService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<ClientService> _logger = logger;

    public async Task<Client> RegisterClient(RegisterClientDto clientDto)
    {

            if (string.IsNullOrWhiteSpace(clientDto.Name)) throw new ArgumentException("Name is required", nameof(clientDto));
            if (string.IsNullOrWhiteSpace(clientDto.Password)) throw new ArgumentException("Password is required", nameof(clientDto));

            // Hash de la contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(clientDto.Password);
            var client = new Client
            {
                Name = clientDto.Name,
                HashedPassword = hashedPassword
            };

            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return client;
    }

    public async Task<Client?> LoginClient(LoginClientDto clientDto)
    {

            if (string.IsNullOrWhiteSpace(clientDto.Name)) throw new ArgumentException("Name is required", nameof(clientDto));
            if (string.IsNullOrWhiteSpace(clientDto.Password)) throw new ArgumentException("Password is required", nameof(clientDto));

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Name == clientDto.Name);
            if (client is null) return null;

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(clientDto.Password, client.HashedPassword);

            // Si devuelve null es porq no metió la contraseña correcta o el nombre de cliente no exite
            return isPasswordValid ? client : null;
 
    }

    // public string GenerateJwtToken(Client client)
    // {
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? "fallback_key_32_chars_long_123456");

    //     var tokenDescriptor = new SecurityTokenDescriptor
    //     {
    //         Subject = new ClaimsIdentity(
    //         [
    //             new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
    //             new Claim(ClaimTypes.Name, client.Name),
    //         ]),

    //         Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRE_HOURS") ?? "24")),
    //         Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "MiniMazErpBack",
    //         Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "MiniMazErpFront",
    //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //     };

    //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     return tokenHandler.WriteToken(token);
    // }
}
