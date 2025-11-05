using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class Student : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public string? Patronymic { get; set; } = "";
    
    public string FullName { get; private set; } = "";
    
    public string? AcademicGroup { get; set; } = "";
    
    public Guid? RoleId { get; set; }
    [ForeignKey("RoleId")]
    public StudentRole? Role { get; set; }
}