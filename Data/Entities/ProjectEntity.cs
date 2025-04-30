using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    public decimal? Budget { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;

    [Required]
    public string ClientId { get; set; } = null!;

    [ForeignKey(nameof(ClientId))]
    public virtual ClientEntity Client { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = null!;

    [Required]
    public int StatusId { get; set; }

    [ForeignKey(nameof(StatusId))]
    public virtual StatusEntity Status { get; set; } = null!;
}
