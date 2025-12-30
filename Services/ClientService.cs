using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class ClientService(AppDbContext context) : IClientService
{
    private readonly AppDbContext _context = context;

    public async Task<Client> RegisterClient(RegisterClientDto clientDto)
    {
        try
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
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<Client?> LoginClient(LoginClientDto clientDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(clientDto.Name)) throw new ArgumentException("Name is required", nameof(clientDto));
            if (string.IsNullOrWhiteSpace(clientDto.Password)) throw new ArgumentException("Password is required", nameof(clientDto));

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Name == clientDto.Name);
            if (client is null) return null;

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(clientDto.Password, client.HashedPassword);

            // Si devuelve null es porq no metió la contraseña correcta o el nombre de cliente no exite
            return isPasswordValid ? client : null;
        }
        catch (Exception)
        {
            throw;
        }
    }


}
