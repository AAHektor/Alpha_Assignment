using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class AddProjectViewModel
{
    [Required]
    public string ProjectName { get; set; } = null!;


    public string? ClientId { get; set; }



    public string? ClientName { get; set; }

    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal? Budget { get; set; }

    [Required]
    public int? StatusId { get; set; }



}
