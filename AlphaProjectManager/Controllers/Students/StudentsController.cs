using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.StudentRoles.Requests;
using AlphaProjectManager.Controllers.StudentRoles.Responses;
using AlphaProjectManager.Controllers.Students.Requests;
using AlphaProjectManager.Controllers.Students.Responses;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaProjectManager.Controllers.Students;

[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly BaseService<Student> _studentService;
    private readonly BaseService<StudentInProject> _studentInProjectService;
    private readonly BaseService<StudentAttendance> _attendanceService;
    private readonly BaseService<StudentRole> _roleService;

    public StudentsController(BaseService<Student> studentService, BaseService<StudentInProject> studentInProjectService,
        BaseService<StudentAttendance> attendanceService, BaseService<StudentRole> roleService)
    {
        _studentService = studentService;
        _studentInProjectService = studentInProjectService;
        _attendanceService = attendanceService;
        _roleService = roleService;
    }
    
    /// <summary>
    /// Получить список студентов с фильтрами через query
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(StudentListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudents([FromQuery] int? skip, [FromQuery] int? take, 
        [FromQuery] Guid? roleId, [FromQuery] string? search)
    {
        var query = new DataQueryParams<Student>
        {
            Filters = roleId.HasValue ? [s => s.RoleId == roleId] : [],
            Paging = new PagingParams
            {
                Skip = skip ?? 0,
                Take = take ?? 50
            },
            IncludeParams = new IncludeParams<Student>
            {
                IncludeProperties = [s => s.Role]
            }
        };
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Expression = s => EF.Functions.ILike(s.FullName, $"%{search}%");
        }
        var students = await _studentService.GetAsync(query);
        return Ok(new StudentListResponse
        {
            Students = students.Select(StudentResponse.FromStudent).ToArray()
        });
    }
    
    /// <summary>
    /// Создать нового студента
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNewStudent([FromBody] CreateStudentRequest dto)
    {
        var newStudent = dto.CreateStudent();
        try
        {
            await _studentService.CreateAsync(newStudent);
        }
        catch (Exception e)
        {
            return SharedResponses.FailedRequest(e.Message);
        }
        return Ok(StudentResponse.FromStudent(newStudent));
    }
    
    /// <summary>
    /// Изменить данные студента по id
    /// </summary>
    [HttpPut($"{{{nameof(studentId)}:guid}}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudent(Guid studentId, [FromBody] UpdateStudentRequest dto)
    {
        var student = await _studentService.GetByIdOrDefaultAsync(studentId);
        if (student == null)
        {
            return SharedResponses.NotFoundObjectResponse<Student>(studentId);
        }
        try
        {
            dto.ApplyToStudent(student);
            await _studentService.UpdateAsync(student);
        }
        catch (Exception e)
        {
            return SharedResponses.FailedRequest(e.Message);
        }

        if (dto.RoleId.HasValue)
        {
            var role = await _roleService.GetByIdOrDefaultAsync(dto.RoleId.Value);
            student.Role = role;
        }
        return Ok(StudentResponse.FromStudent(student));
    }
    
    /// <summary>
    /// Удалить студента
    /// </summary>
    [HttpDelete($"{{{nameof(studentId)}:guid}}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveStudent(Guid studentId)
    {
        var attendances = await _attendanceService.GetAsync(new DataQueryParams<StudentAttendance>
        {
            Expression = at => at.StudentId == studentId
        });
        var inProjects = await _studentInProjectService.GetAsync(new DataQueryParams<StudentInProject>
        {
            Expression = at => at.StudentId == studentId
        });
        await _attendanceService.RemoveRangeAsync(attendances);
        await _studentInProjectService.RemoveRangeAsync(inProjects);
        if (await _studentService.TryRemoveAsync(studentId))
        {
            return SharedResponses.SuccessRequest();
        }
        return SharedResponses.FailedRequest("Failed to delete Student with provided Id from database.");
    }
}