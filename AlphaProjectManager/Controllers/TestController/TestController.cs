using AlphaProjectManager.Controllers.TestController.Responses;
using AlphaProjectManager.Controllers.Utility;
using Application.DataQuery;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.TestController;

[Route("test")]
public class TestController : ControllerBase
{
    private List<Project> _projects =
    [
        new Project { Id = Guid.NewGuid(), Title = "Проект 1" },
        new Project { Id = Guid.NewGuid(), Title = "Проект 2" },
        new Project { Id = Guid.NewGuid(), Title = "Проект 3" },
    ];
    
    public TestController()
    {
        
    }
    
    /// <summary>
    /// Получить весь список проектов
    /// </summary>
    [HttpGet("get-all-projects")]
    [ProducesResponseType(typeof(GetProjectListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProjects()
    {
        return Ok(new GetProjectListResponse
        {
            Completed = true,
            Message = "",
            Projects = _projects.Select(p => p.Title).ToList()
        });
    }
}