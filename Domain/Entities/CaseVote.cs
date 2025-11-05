using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class CaseVote : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public required Guid CaseId { get; set; }
    [ForeignKey("CaseId")] 
    public ProjectCase Case { get; set; } = null!;
    
    public required Guid UserId { get; set; }
    [ForeignKey("UserId")] 
    public User User { get; set; } = null!;
    
    public required CaseReactionType ReactionType { get; set; }
}