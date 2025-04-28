using Business.Models;
using Data.Entities;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager)
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
}
