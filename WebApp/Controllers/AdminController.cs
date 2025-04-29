using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {

        [Route("projects")]
        public IActionResult Projects()
        {
            var model = new ProjectsViewModel
            {
                Projects = new List<ProjectViewModel>(), // eller hämta riktiga projekt
                AddProjectFormData = new AddProjectViewModel(),
                EditProjectFormData = new EditProjectViewModel()
            };

            return View(model);
        }


        [Route("members")]
        public IActionResult Members()
        {
            return View();
        }

    }
}
