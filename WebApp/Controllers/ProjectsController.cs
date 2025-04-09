using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("projects")]
    public class ProjectsController : Controller
    {
        public IActionResult Projects()
        {
            return View();
        }
    }
}
