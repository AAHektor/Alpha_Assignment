namespace Presentation.Models;
using System.Linq;

public class ProjectsViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];
    public AddProjectViewModel AddProjectFormData { get; set; } = new();
    public EditProjectViewModel EditProjectFormData { get; set; } = new ();
    public int AllCount => Projects?.Count() ?? 0;

    public int StartedCount => Projects?.Count(p => p.Status.Id == 1) ?? 0;

    public int CompletedCount => Projects?.Count(p => p.Status.Id == 2) ?? 0;



}
