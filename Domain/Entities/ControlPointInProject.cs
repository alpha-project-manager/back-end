using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities;

public class ControlPointInProject : IHasId
{
    [Key]
    public required Guid Id { get; set; }
    
    public Guid? ControlPointId { get; set; }
    [ForeignKey("ControlPointId")] 
    public ControlPoint ControlPoint { get; set; } = null!;
    
    public required Guid ProjectId { get; set; }
    [ForeignKey("ProjectId")] 
    public Project Project { get; set; } = null!;

    public string Title { get; set; } = "";
    
    public string VideoUrl { get; set; } = "";

    public int CompanyMark { get; set; } = 0;
    
    public int UrfuMark { get; set; } = 0;
    
    public required bool Completed { get; set; }
    
    public required DateTime Date { get; set; }
}