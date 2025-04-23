using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        [Route("auth/signup")]
        public IActionResult Signup()
        {
             return View();
        }

        [HttpPost]
        [Route("auth/signup")]

        public IActionResult Signup(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            return View();
        }

        [Route("auth/login")]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("auth/login")]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            return View();
        }

    }
}
