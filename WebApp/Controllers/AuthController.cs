using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Security.Claims;

namespace Presentation.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<UserEntity> _signInManager;


        public AuthController(IAuthService authService, SignInManager<UserEntity> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
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

            if (result.Succeeded && result.User is not null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.User.Id),
                    new Claim(ClaimTypes.Email, result.User.Email ?? ""),
                    new Claim("FullName", result.User.FullName ?? "")
                };

                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);


                return RedirectToAction("Index", "Projects", new { area = "admin" });
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
                return View("SignIn", model);

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

            ViewBag.ErrorMessage = "Email or password is incorrect";
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
