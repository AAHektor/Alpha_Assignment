using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        public IActionResult SignUp()
        {
             return View();
        }

        [HttpPost]

        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var signUpFormData = new SignUpFormData
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _authService.SignUpAsync(signUpFormData);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.ErrorMessage = result.Error;
            return View(model);
        }

        public IActionResult SignIn(string returnUrl = "/admin/projects")
        {
            ViewBag.ErrorMessage = null;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "/admin/projects")

        {
            ViewBag.ErrorMessage = null;
            ViewBag.ReturnUrl = returnUrl;


            if (!ModelState.IsValid)
                return View("SignIn", model); ;

            var signInFormData = new SignInFormData
            {
                Email = model.Email,
                Password = model.Password,
            };

            var result = await _authService.SignInAsync(signInFormData);

            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            ViewBag.ErrorMessage = result.Error;
            return View("SignIn", model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.SignOutAsync();
            return RedirectToAction("SignIn", "Auth");
        }

    }
}
