using Business.Models;
using Data.Entities;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult> CreateUserAsync(SignUpFormData formData);
    Task<UserResult> GetUsersAsync();
}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<UserResult> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync(x => x);

        if (!result.Succeeded || result.Result == null)
        {
            return new UserResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "No users found."
            };
        }

        var mappedUsers = result.Result.Select(entity => new User
        {
            Id = entity.Id,
            Email = entity.Email,
            FullName = entity.FullName
        });

        return new UserResult
        {
            Succeeded = true,
            StatusCode = result.StatusCode,
            Result = mappedUsers
        };
    }

    public async Task<UserResult> CreateUserAsync(SignUpFormData formData)
    {


        if (formData == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "form data can't be null" };

        var existsResult = await _userRepository.ExistsAsync(x => x.Email == formData.Email);
        if (existsResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "User with this email already exists" };

        try
        {
            var userEntity = new UserEntity
            {
                FullName = formData.FullName!,
                Email = formData.Email!,
                UserName = formData.Email! 
            };

            var result = await _userManager.CreateAsync(userEntity, formData.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Debug.WriteLine($"[UserService] Error creating user: {error.Description}");
                }
            }


            if (!result.Succeeded)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                Debug.WriteLine($"User creation failed: {errorMessages}");
                return new UserResult
                {
                    Succeeded = false,
                    StatusCode = 500,
                    Error = errorMessages
                };
            }

            return new UserResult
            {
                Succeeded = true,
                StatusCode = 201
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Console.WriteLine($"[UserService] Exception: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"[UserService] InnerException: {ex.InnerException.Message}");

            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }


}
