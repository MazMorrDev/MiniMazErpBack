namespace MiniMazErpBack;

public interface IUserService
{
    Task<User> RegisterUser(RegisterUserDto userDto);
    Task<User?> LoginUser(LoginUserDto userDto);
}
