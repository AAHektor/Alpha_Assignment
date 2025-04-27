using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public class StatusRepository(AppDbContext context) : BaseRepository<StatusEntity>(context)
{
}

