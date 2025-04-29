namespace Presentation.Models;

public class ProjectViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectName { get; set; } = null;
    public string ProjectClient { get; set; } = null;
    public string ProjectDescription { get; set; } = null;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string Status { get; set; } = null!;
}
