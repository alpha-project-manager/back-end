using Domain.Entities;

namespace Application.Models;

public class ImportProjectsActionResult : ServiceActionResult
{
    public List<Project> UpdatedProjects { get; set; } = [];
    
    public List<Project> CreatedProjects { get; set; } = [];
}