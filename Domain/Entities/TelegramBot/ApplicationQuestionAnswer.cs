using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities.TelegramBot;

public class ApplicationQuestionAnswer : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid ApplicationId { get; set; }
    [ForeignKey("ApplicationId")] 
    public ProjectApplication ProjectApplication { get; set; } = null!;
    
    public required string QuestionTitle { get; set; }
    
    public required string Answer { get; set; }
    
    public required long TimeStamp { get; set; }
}