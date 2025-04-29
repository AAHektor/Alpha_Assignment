using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

[Index(nameof(ClientId), IsUnique = true)]
public class ClientEntity
{
    [Key]
    public string Id { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];

}
