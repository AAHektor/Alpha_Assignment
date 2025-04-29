using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormData formData);
    Task<AuthResult> SignOutAsync();
    Task<AuthResult> SignUpAsync(SignUpFormData formData);
}

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Missing login info" };

        var user = await _signInManager.UserManager.FindByEmailAsync(formData.Email);
        if (user == null)
            return new AuthResult { Succeeded = false, StatusCode = 401, Error = "User not found" };

        var checkPassword = await _signInManager.UserManager.CheckPasswordAsync(user, formData.Password);
        if (!checkPassword)
            return new AuthResult { Succeeded = false, StatusCode = 401, Error = "Invalid password" };

        await _signInManager.SignInAsync(user, formData.IsPersistent);

        // ✅ Returnera ett lyckat resultat!
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }




    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied. " };

        var result = await _userService.CreateUserAsync(formData);
        return result.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 201 }
            : new AuthResult { Succeeded = false, StatusCode = 400, Error = result.Error };

    }

    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }
}