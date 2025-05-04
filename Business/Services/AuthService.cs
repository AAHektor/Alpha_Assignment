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

/* GENERERAD AV CHATGPT 4o */
/* Den här koden loggar in en användare: den kollar att inloggningsdatan finns, att */
/* användaren finns, att lösenordet stämmer, loggar ut tidigare sessioner, skapar nya */
/* inloggningsclaims och loggar in användaren.*/
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

        await _signInManager.SignOutAsync();

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email ?? ""),
        new Claim("FullName", user.FullName ?? "")
    };

        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }




    /* GENERERAD AV CHATGPT 4o */
    /* Den här koden registrerar en ny användare: den kollar att all data finns, försöker skapa */
    /* användaren, och om det lyckas loggar den in användaren direkt med claims. */
    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _userService.CreateUserAsync(formData);

        if (!result.Succeeded || result.User == null)
        {
            return new AuthResult
            {
                Succeeded = false,
                StatusCode = 400,
                Error = result.Error ?? "Could not create user"
            };
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, result.User.Id),
        new Claim(ClaimTypes.Email, result.User.Email ?? ""),
        new Claim("FullName", result.User.FullName ?? "")
    };

        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, principal);

        return new AuthResult
        {
            Succeeded = true,
            StatusCode = 201,
            User = result.User
        };
    }




    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 200 };
    }
}