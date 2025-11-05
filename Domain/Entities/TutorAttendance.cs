using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class TutorAttendance : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid MeetingId { get; set; }
    [ForeignKey("MeetingId")] 
    public Meeting Meeting { get; set; } = null!;
    
    public required Guid TutorId { get; set; }
    [ForeignKey("TutorId")]
    public Tutor Tutor { get; set; } = null!;
    
    public required bool Attended { get; set; }
}