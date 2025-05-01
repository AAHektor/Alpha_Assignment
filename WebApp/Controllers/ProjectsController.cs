using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Presentation.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("admin/projects")]
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IClientService _clientService;
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService, IClientService clientService, AppDbContext context)
        {
            _projectService = projectService;
            _clientService = clientService;
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var projectsResult = await _projectService.GetProjectsByUserAsync(userId);


            var viewModel = new ProjectsViewModel
            {
                Projects = projectsResult.Result.Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    ProjectClient = p.Client.DisplayName,
                    ProjectDescription = p.Description,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Budget = p.Budget,
                    Status = p.Status.StatusName,
                    StatusId = p.StatusId
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddProjectViewModel model)
        {
            ModelState.Remove("ClientId");

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, debugStatus = 1, error = "ModelState invalid", modelState = ModelState });
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string clientId;

            if (!string.IsNullOrWhiteSpace(model.ClientId) && Guid.TryParse(model.ClientId, out _))
            {
                // ClientId är redan ett GUID från dropdown
                clientId = model.ClientId;
            }
            else if (!string.IsNullOrWhiteSpace(model.ClientName))
            {
                // Sök om klienten redan finns
                var existingClient = _context.Clients.FirstOrDefault(c => c.DisplayName == model.ClientName.Trim());

                if (existingClient != null)
                {
                    clientId = existingClient.Id; // ✅ Använd befintligt ID
                }
                else
                {
                    var newClient = new ClientEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        DisplayName = model.ClientName.Trim()
                    };
                    _context.Clients.Add(newClient);
                    await _context.SaveChangesAsync();
                    clientId = newClient.Id; // ✅ Använd nytt ID
                }
            }
            else
            {
                return Json(new { success = false, error = "Ingen klient vald eller angiven." });
            }


            var formData = new AddProjectFormData
            {
                ProjectName = model.ProjectName,
                ClientId = clientId,
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
                StartDate = model.StartDate.Value,
                EndDate = model.EndDate,
                Budget = model.Budget,
                ClientId = model.ClientId,
                StatusId = model.StatusId
            };

            var result = await _projectService.UpdateProjectAsync(updateFormData);

            return Json(new { success = result.Succeeded });
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteProjectRequestViewModel request)
        {
            if (string.IsNullOrEmpty(request.Id))
                return Json(new { success = false, error = "Invalid project ID" });

            var result = await _projectService.DeleteProjectAsync(request.Id);
            return Json(new { success = result.Succeeded });
        }

        public async Task<IActionResult> Edit(string id)
        {
            var projectResult = await _projectService.GetProjectByIdAsync(id);

            if (!projectResult.Succeeded || projectResult.Result == null)
                return NotFound();

            var project = projectResult.Result;


            var model = new EditProjectViewModel
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                ClientId = project.ClientId,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Budget = project.Budget,
                StatusId = project.StatusId,
                Clients = await GetClientsSelectListAsync()
            };

            return View(model);
        }

        private async Task<List<SelectListItem>> GetClientsSelectListAsync()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return clients.Select(c => new SelectListItem
            {
                Value = c.Id,
                Text = c.Name
            }).ToList();
        }

        [HttpGet("api/project/{id}")]
        public async Task<IActionResult> GetProject(string id)
        {
            var projectResult = await _projectService.GetProjectByIdAsync(id);
            if (!projectResult.Succeeded || projectResult.Result == null)
                return NotFound();

            var clients = await _clientService.GetAllClientsAsync();

            return Json(new
            {
                project = projectResult.Result,
                clients = clients
            });
        }

    }


}
