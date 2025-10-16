using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class User : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string Email { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public required string Salt { get; set; }
    
    public required Guid CalendarSettingsId { get; set; }
    [ForeignKey("CalendarSettingsId")] 
    public CalendarSettings CalendarSettings { get; set; } = null!;
    
    public Guid? TutorId { get; set; }
    [ForeignKey("TutorId")]
    public Tutor? Tutor { get; set; }
}