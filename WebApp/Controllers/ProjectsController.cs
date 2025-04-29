using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ProjectsController(IProjectService projectService) : Controller
    {
        private readonly IProjectService _projectService = projectService;

        public async Task<IActionResult> Index()
        {
            var projectsResult = await _projectService.GetProjectsAsync();

            var viewModel = new ProjectsViewModel
            {
                Projects = projectsResult.Result.Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectClient = p.ClientName, 
                    ProjectDescription = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Budget = p.Budget,
                    Status = p.Status.StatusName
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProjectViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, error = "Invalid data" });

            var addProjectFormData = new AddProjectFormData
            {
                ProjectName = model.ProjectName,
                ClientName = model.ClientName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Budget = model.Budget,
                StatusId = model.StatusId
            };

            var result = await _projectService.CreateProjectAsync(addProjectFormData);

            return Json(new { success = result.Succeeded });
        }

        [HttpPost]
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
                ClientName = model.ClientName,
                StatusId = model.StatusId
            };

            var result = await _projectService.UpdateProjectAsync(updateFormData);

            return Json(new { success = result.Succeeded });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Json(new { success = false, error = "Invalid project ID" });

            var result = await _projectService.DeleteProjectAsync(id);

            return Json(new { success = result.Succeeded });
        }
    }
}
