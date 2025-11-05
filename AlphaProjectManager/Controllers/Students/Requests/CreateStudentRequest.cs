using Domain.Entities;

namespace AlphaProjectManager.Controllers.Students.Requests;

public class CreateStudentRequest
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string? Patronymic { get; set; } = "";
    
    public string? AcademicGroup { get; set; } = "";
    
    public Guid? RoleId { get; set; }

    public Student CreateStudent()
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            FirstName = FirstName,
            LastName = LastName,
            Patronymic = Patronymic,
            AcademicGroup = AcademicGroup,
            RoleId = RoleId
        };
    }
}