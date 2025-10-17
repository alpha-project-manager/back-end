using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities.TelegramBot;

public class ApplicationQuestion : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string MsgText { get; set; }
    
    public Guid? PrevQuestionId { get; set; }
    [ForeignKey("PrevQuestionId")]
    public ApplicationQuestion? PrevQuestion { get; set; }
    
    public Guid? NextQuestionId { get; set; }
    [ForeignKey("NextQuestionId")]
    public ApplicationQuestion? NextQuestion { get; set; }
    
}