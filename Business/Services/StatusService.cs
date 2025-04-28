using Business.Models;
using Data.Models;
using Data.Repositories;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusResult> GetStatusesAsync();
}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    public async Task<StatusResult> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync(x => x);

        if (!result.Succeeded || result.Result == null)
        {
            return new StatusResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = "No statuses found."
            };
        }

        var mappedStatuses = result.Result.Select(entity => new Status
        {
            Id = entity.Id,
            StatusName = entity.StatusName
        });

        return new StatusResult
        {
            Succeeded = true,
            StatusCode = result.StatusCode,
            Result = mappedStatuses
        };
    }
}
