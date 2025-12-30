namespace MiniMazErpBack;

public interface IClientService
{
    Task<Client> RegisterClient(RegisterClientDto clientDto);
    Task<Client?> LoginClient(LoginClientDto clientDto);
}
