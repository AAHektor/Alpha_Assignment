using Data.Contexts;
using Data.Entities;

namespace Data.Repositories;

public class ProjectRepository(AppDbContext context) : BaseRepository<ProjectEntity>(context)
{
}

