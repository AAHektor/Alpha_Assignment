using Business.Models;
using Data.Entities;
using Data.Models;
using Data.Repositories;
using System.Linq.Expressions;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
    Task<ProjectResult<ProjectEntity>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<ProjectEntity>>> GetProjectsAsync();
    Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormData formData);
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;


    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
        {
            // return new ProjectResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied. " };
        }

        var projectEntity = new ProjectEntity
        {
            ProjectName = formData.ProjectName,
            Description = formData.Description,
            StartDate = formData.StartDate,
            EndDate = formData.EndDate,
            Budget = formData.Budget,
            ClientId = formData.ClientId,
            UserId = formData.UserId,
            StatusId = formData.StatusId,

        };
        var result = await _projectRepository.AddAsync(projectEntity);
        return result.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 201 }
            : new ProjectResult { Succeeded = false, StatusCode = 400, Error = result.Error };
    }

    public async Task<ProjectResult<IEnumerable<ProjectEntity>>> GetProjectsAsync()
    {
        var response = await _projectRepository.GetAllAsync(
            selector: x => x,
            orderByDescending: true,
            sortBy: s => s.Created,
            where: null,
            includes: [

            include => include.User,
            include => include.Status,
            include => include.Client
            ]

        );

        return new ProjectResult<IEnumerable<ProjectEntity>> { Succeeded = true, StatusCode = 200, Result = response.Result };
    }

    public async Task<ProjectResult<ProjectEntity>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync(
            where: x => x.Id == id,
            includes: [

            include => include.User,
            include => include.Status,
            include => include.Client
            ]

        );
        return response.Succeeded
            ? new ProjectResult<ProjectEntity> { Succeeded = true, StatusCode = 200, Result = response.Result }
            : new ProjectResult<ProjectEntity> { Succeeded = false, StatusCode = 404, Error = $"Project '{id}' was not found." };



    }

    public async Task<ProjectResult> UpdateProjectAsync(UpdateProjectFormData formData)
    {
        var response = await _projectRepository.GetAsync(x => x.Id == formData.Id);

        if (!response.Succeeded || response.Result == null)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Project with ID '{formData.Id}' not found."
            };
        }

        var projectEntity = response.Result;
        projectEntity.ProjectName = formData.ProjectName;
        projectEntity.Description = formData.Description;
        projectEntity.StartDate = formData.StartDate;
        projectEntity.EndDate = formData.EndDate;
        projectEntity.Budget = formData.Budget;
        projectEntity.StatusId = formData.StatusId;
        projectEntity.ClientId = formData.ClientId;

        var updateResult = await _projectRepository.UpdateAsync(projectEntity);

        return updateResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = 500, Error = updateResult.Error };
    }

    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync(x => x.Id == id);

        if (!response.Succeeded || response.Result == null)
        {
            return new ProjectResult
            {
                Succeeded = false,
                StatusCode = 404,
                Error = $"Project with ID '{id}' not found."
            };
        }

        var deleteResult = await _projectRepository.DeleteAsync(response.Result);

        return deleteResult.Succeeded
            ? new ProjectResult { Succeeded = true, StatusCode = 200 }
            : new ProjectResult { Succeeded = false, StatusCode = 500, Error = deleteResult.Error };
    }


}
