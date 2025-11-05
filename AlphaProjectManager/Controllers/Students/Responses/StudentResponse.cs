using AlphaProjectManager.Controllers.StudentRoles.Responses;
using Domain.Entities;

namespace AlphaProjectManager.Controllers.Students.Responses;

public class StudentResponse
{
    public required Guid Id { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string? Patronymic { get; set; } = "";
    
    public string? AcademicGroup { get; set; } = "";
    
    public StudentRoleResponse? Role { get; set; }

    public static StudentResponse FromStudent(Student student)
    {
        return new StudentResponse
        {
            FirstName = student.FirstName,
            LastName = student.LastName,
            Patronymic = student.Patronymic,
            AcademicGroup = student.AcademicGroup,
            Role = student.Role == null
                ? null
                : StudentRoleResponse.FromDomainEntity(student.Role),
            Id = student.Id
        };
    }
}