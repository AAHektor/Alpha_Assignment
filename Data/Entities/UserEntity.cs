using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string? FullName { get; set; } = null!;
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];

}
