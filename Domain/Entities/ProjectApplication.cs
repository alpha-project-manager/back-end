using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.TelegramBot;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class ProjectApplication : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid CaseId { get; set; }
    [ForeignKey("CaseId")] 
    public ProjectCase ProjectCase { get; set; } = null!;
    
    public required string TeamTitle { get; set; }
    
    public required ApplicationStatus Status { get; set; }
    
    public required long ChatId { get; set; }
    
    public required string TelegramUsername { get; set; }
    
    public Guid? CurrentQuestionId { get; set; }
    [ForeignKey("NextQuestionId")] 
    public ApplicationQuestion? CurrentQuestion { get; set; }
}