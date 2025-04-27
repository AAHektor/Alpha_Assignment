using Data.Contexts;
using Data.Entities;
using Data.Repositories;

namespace Data.Repositories;

public interface  IClientRepository : IBaseRepository<ClientEntity>
{
    
}

public class ClientRepository(AppDbContext context) : BaseRepository<ClientEntity>(context)
{
}

