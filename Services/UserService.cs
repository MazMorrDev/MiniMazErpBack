using Microsoft.EntityFrameworkCore;

namespace MiniMazErpBack;

public class UserService(AppDbContext context, ILogger<UserService> logger) : IUserService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<User> RegisterUser(RegisterUserDto userDto)
    {

            if (string.IsNullOrWhiteSpace(userDto.Name)) throw new ArgumentException("Name is required", nameof(userDto));
            if (string.IsNullOrWhiteSpace(userDto.Password)) throw new ArgumentException("Password is required", nameof(userDto));

            // Hash de la contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = new User
            {
                Name = userDto.Name,
                HashedPassword = hashedPassword
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
    }

    public async Task<User?> LoginUser(LoginUserDto userDto)
    {

            if (string.IsNullOrWhiteSpace(userDto.Name)) throw new ArgumentException("Name is required", nameof(userDto));
            if (string.IsNullOrWhiteSpace(userDto.Password)) throw new ArgumentException("Password is required", nameof(userDto));

            var user = await _context.Users.FirstOrDefaultAsync(c => c.Name == userDto.Name);
            if (user is null) return null;

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(userDto.Password, user.HashedPassword);

            // Si devuelve null es porq no metió la contraseña correcta o el nombre de usuario no exite
            return isPasswordValid ? user : null;
 
    }

}
