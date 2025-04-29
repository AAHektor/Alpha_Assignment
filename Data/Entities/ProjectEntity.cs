using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }
    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    [ForeignKey(nameof(Client))]
    public string ClientName { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    [ForeignKey(nameof(Status))]
    public string StatusId { get; set; } = null!;
    public StatusEntity Status { get; set; } = null!;
}
