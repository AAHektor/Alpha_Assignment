using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Route("projects")]
    public class ProjectsController : Controller
    {
        [Route("")]
        public IActionResult Projects()
        {
            return View();
        }
    }
}
