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
            Projects = [new(), new()]
        };

        return View(viewModel); 
    }
}
