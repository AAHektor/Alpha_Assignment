using Business.Models;
using Data.Models;
using Data.Repositories;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync(x => x);

        if (!result.Succeeded || result.Result == null)
        {
            return new ClientResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "No clients found."
            };
        }

        var mappedClients = result.Result.Select(entity => new Client
        {
            Id = entity.Id,
            ClientId = entity.ClientId
        });

        return new ClientResult
        {
            Succeeded = true,
            StatusCode = result.StatusCode,
            Result = mappedClients
        };
    }
}
