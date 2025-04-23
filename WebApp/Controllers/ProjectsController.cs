using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class ProjectsController : Controller
{
    [Route("admin/projects")]
    public IActionResult Index()
    {
        var viewModel = new ProjectsViewModel()
        {
            Projects = SetProjects()
        };

        return View(viewModel); 
    }

    private IEnumerable<ProjectViewModel> SetProjects()
    {
        var projects = new List<ProjectViewModel>();

        projects.Add(new ProjectViewModel
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = "Website Redesign",
            ProjectClient = "ABC Corp",
            ProjectDescription = "<p>Redesign the company website to improve user experience and accessibility.</p>",

        });

        return projects;
    } 

}
