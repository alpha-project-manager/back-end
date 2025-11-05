using Domain.Entities;

namespace AlphaProjectManager.Controllers.Students.Requests;

public class UpdateStudentRequest
{
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string? Patronymic { get; set; } = "";
    
    public string? AcademicGroup { get; set; } = "";
    
    public Guid? RoleId { get; set; }

    public void ApplyToStudent(Student student)
    {
        student.FirstName = FirstName;
        student.LastName = LastName;
        student.Patronymic = Patronymic;
        student.AcademicGroup = AcademicGroup;
        student.RoleId = RoleId;
    }
}