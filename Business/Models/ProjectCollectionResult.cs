using Data.Entities;

namespace Business.Models;


public class ProjectCollectionResult
{
    public bool Succeeded { get; set; } = true;
    public string? Error { get; set; }
    public List<ProjectEntity> Result { get; set; } = [];
}
