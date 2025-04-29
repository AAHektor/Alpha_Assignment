using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("admin/projects")]
    public class ProjectsController(IProjectService projectService) : Controller
    {
        private readonly IProjectService _projectService = projectService;

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var projectsResult = await _projectService.GetProjectsAsync();

            var viewModel = new ProjectsViewModel
            {
                Projects = projectsResult.Result.Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectClient = p.ClientId,  
                    ProjectDescription = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Budget = p.Budget,
                    Status = p.Status.StatusName
                })
            };

            return View(viewModel);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddProjectViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, error = "Invalid data" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var formData = new AddProjectFormData
            {
                ProjectName = model.ProjectName,
                ClientId = model.ClientId,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Budget = model.Budget,
                StatusId = model.StatusId ?? 1,
                UserId = userId 
            };

            var result = await _projectService.CreateProjectAsync(formData);

            return Json(new { success = result.Succeeded, error = result.Error });
        }


        [HttpPost("update")]
        public async Task<IActionResult> Update(EditProjectViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, error = "Invalid data" });

            var updateFormData = new UpdateProjectFormData
            {
                Id = model.Id,
                ProjectName = model.ProjectName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Budget = model.Budget,
                ClientId = model.ClientId,
                StatusId = model.StatusId
            };

            var result = await _projectService.UpdateProjectAsync(updateFormData);

            return Json(new { success = result.Succeeded });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false, error = "Invalid project ID" });

            var result = await _projectService.DeleteProjectAsync(id);

            return Json(new { success = result.Succeeded });
        }
    }
}
