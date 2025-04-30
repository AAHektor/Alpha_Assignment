using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;


public class ClientEntity
{
    [Key]
    public string Id { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];

}
