using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class StudentAttendance : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid MeetingId { get; set; }
    [ForeignKey("MeetingId")] 
    public Meeting Meeting { get; set; } = null!;
    
    public required Guid StudentId { get; set; }
    [ForeignKey("StudentId")]
    public Student Student { get; set; } = null!;
    
    public required bool Attended { get; set; }
}