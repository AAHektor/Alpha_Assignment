using Business.Models;
using Data.Contexts;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IClientService
{
    Task<IEnumerable<ClientModel>> GetAllClientsAsync();
    Task<ClientResult> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository, AppDbContext context) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly AppDbContext _context = context;

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
            ClientId = entity.DisplayName
        });

        return new ClientResult
        {
            Succeeded = true,
            StatusCode = result.StatusCode,
            Result = mappedClients
        };
    }

    public async Task<IEnumerable<ClientModel>> GetAllClientsAsync()
    {
        var clients = await _context.Clients.ToListAsync();

        Console.WriteLine("==== CLIENTER FUNNA ====");
        foreach (var c in clients)
        {
            Console.WriteLine($"{c.Id} - {c.DisplayName}");
        }

        return clients.Select(c => new ClientModel
        {
            Id = c.Id,
            Name = c.DisplayName
        });
    }



}
