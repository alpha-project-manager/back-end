using Domain.Interfaces;

namespace Domain.Entities;

public class Project : IHasId
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
}