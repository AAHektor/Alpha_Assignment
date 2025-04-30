using Business.Models;
using Data.Models;
using Data.Repositories;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusResult<Status>> GetStatusByIdAsync(int id);
    Task<StatusResult<Status>> GetStatusByNameAsync(string statusName);
    Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    public async Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync(x => x);

        if (!result.Succeeded || result.Result == null)
        {
            return new StatusResult<IEnumerable<Status>>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error ?? "No statuses found."
            };
        }

        return result.Succeeded
            ? new StatusResult<IEnumerable<Status>>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = result.Result.Select(entity => new Status
                {
                    Id = entity.Id,
                    StatusName = entity.StatusName
                })
            }
            : new StatusResult<IEnumerable<Status>>
            {
                Succeeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
    }


    public async Task<StatusResult<Status>> GetStatusByNameAsync(string statusName)
    {
        var response = await _statusRepository.GetAsync(x => x.StatusName == statusName);

        if (!response.Succeeded || response.Result == null)
        {
            return new StatusResult<Status>
            {
                Succeeded = false,
                StatusCode = response.StatusCode,
                Error = response.Error ?? "Status not found."
            };
        }

        return response.Succeeded
            ? new StatusResult<Status>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = new Status
                {
                    Id = response.Result.Id,
                    StatusName = response.Result.StatusName
                }
            }
            : new StatusResult<Status>
            {
                Succeeded = false,
                StatusCode = response.StatusCode,
                Error = response.Error
            };
    }

    public async Task<StatusResult<Status>> GetStatusByIdAsync(int id)
    {
        var response = await _statusRepository.GetAsync(x => x.Id == id);

        if (!response.Succeeded || response.Result == null)
        {
            return new StatusResult<Status>
            {
                Succeeded = false,
                StatusCode = response.StatusCode,
                Error = response.Error ?? "Status not found."
            };
        }

        return response.Succeeded
            ? new StatusResult<Status>
            {
                Succeeded = true,
                StatusCode = 200,
                Result = new Status
                {
                    Id = response.Result.Id,
                    StatusName = response.Result.StatusName
                }
            }
            : new StatusResult<Status>
            {
                Succeeded = false,
                StatusCode = response.StatusCode,
                Error = response.Error
            };
    }

    public async Task<StatusResult> UpdateStatusAsync(UpdateStatusFormData formData)
    {
        var response = await _statusRepository.GetAsync(x => x.Id == formData.Id);

        if (!response.Succeeded || response.Result == null)
        {
            return new StatusResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Status with ID '{formData.Id}' not found."
            };
        }

        var statusEntity = response.Result;
        statusEntity.StatusName = formData.StatusName;

        var updateResult = await _statusRepository.UpdateAsync(statusEntity);

        return updateResult.Succeeded
            ? new StatusResult { Succeeded = true, StatusCode = 200 }
            : new StatusResult { Succeeded = false, StatusCode = 500, Error = updateResult.Error };
    }

    public async Task<StatusResult> DeleteStatusAsync(int id)
    {
        var response = await _statusRepository.GetAsync(x => x.Id == id);

        if (!response.Succeeded || response.Result == null)
        {
            return new StatusResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Status with ID '{id}' not found."
            };
        }

        var deleteResult = await _statusRepository.DeleteAsync(response.Result);

        return deleteResult.Succeeded
            ? new StatusResult { Succeeded = true, StatusCode = 200 }
            : new StatusResult { Succeeded = false, StatusCode = 500, Error = deleteResult.Error };
    }





}
