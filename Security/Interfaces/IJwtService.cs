namespace MiniMazErpBack;

public interface IJwtService
{
    string GenerateJwtToken(Client client);
}
