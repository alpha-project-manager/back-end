using AlphaProjectManager.Controllers.TestController.Responses;
using AlphaProjectManager.Controllers.Utility;
using Application.DataQuery;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.TestController;

[Route("test")]
public class TestController : ControllerBase
{
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
            Projects = []
        });
    }
}