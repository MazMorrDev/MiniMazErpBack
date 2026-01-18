namespace MiniMazErpBack;

public interface IJwtService
{
    string GenerateJwtToken(User user);
}
