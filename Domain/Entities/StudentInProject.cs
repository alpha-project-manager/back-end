using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;


public class StudentInProject
{
    public required Guid ProjectId { get; set; }
    [ForeignKey("ProjectId")] 
    public Project Project { get; set; } = null!;
    
    public required Guid StudentId { get; set; }
    [ForeignKey("StudentId")] 
    public Student Student { get; set; } = null!;
}