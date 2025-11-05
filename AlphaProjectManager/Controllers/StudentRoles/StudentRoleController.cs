using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.StudentRoles.Requests;
using AlphaProjectManager.Controllers.StudentRoles.Responses;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.StudentRoles;

[Route("api/student-roles")]
public class StudentRoleController : ControllerBase
{
    private readonly BaseService<StudentRole> _rolesService;

    public StudentRoleController(BaseService<StudentRole> rolesService)
    {
        _rolesService = rolesService;
    }
    
    /// <summary>
    /// Получить список всех ролей студентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(StudentRoleListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _rolesService.GetAsync(new DataQueryParams<StudentRole>());
        return Ok(new StudentRoleListResponse
        {
            Roles = roles.Select(StudentRoleResponse.FromDomainEntity).ToArray()
        });
    }
    
    /// <summary>
    /// Создать новую роль студента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudentRoleListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateNewRole([FromBody] CreateStudentRoleRequest dto)
    {
        var newRole = new StudentRole
        {
            Id = Guid.NewGuid(),
            Title = dto.Title
        };
        await _rolesService.CreateAsync(newRole);
        return Ok(StudentRoleResponse.FromDomainEntity(newRole));
    }
    
    /// <summary>
    /// Изменить роль студента
    /// </summary>
    [HttpPut($"{{{nameof(roleId)}:guid}}")]
    [ProducesResponseType(typeof(StudentRoleListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateStudentRoleRequest dto)
    {
        var role = await _rolesService.GetByIdOrDefaultAsync(roleId);
        if (role == null)
        {
            return SharedResponses.NotFoundObjectResponse<StudentRole>(roleId);
        }

        role.Title = dto.Title;
        await _rolesService.UpdateAsync(role);
        return Ok(StudentRoleResponse.FromDomainEntity(role));
    }
    
    /// <summary>
    /// Удалить роль студента
    /// </summary>
    [HttpDelete($"{{{nameof(roleId)}:guid}}")]
    [ProducesResponseType(typeof(StudentRoleListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRole(Guid roleId)
    {
        if (await _rolesService.TryRemoveAsync(roleId))
        {
            return SharedResponses.SuccessRequest();
        }
        return SharedResponses.NotFoundObjectResponse<StudentRole>(roleId);
    }
}