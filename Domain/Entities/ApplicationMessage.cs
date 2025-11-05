using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class ApplicationMessage : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid ApplicationId { get; set; }
    [ForeignKey("ApplicationId")] 
    public ProjectApplication ProjectApplication { get; set; } = null!;

    public required string Content { get; set; }
    
    public required ApplicationMsgDirection Direction { get; set; }
    
    public required long Timestamp { get; set; }
    
    public required bool IsRead { get; set; }
}