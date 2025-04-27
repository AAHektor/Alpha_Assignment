using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<UserEntity>(context)
{
}

