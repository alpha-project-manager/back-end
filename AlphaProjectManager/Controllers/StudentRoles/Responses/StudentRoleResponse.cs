using Domain.Entities;

namespace AlphaProjectManager.Controllers.StudentRoles.Responses;

public class StudentRoleResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }

    public static StudentRoleResponse FromDomainEntity(StudentRole role)
    {
        return new StudentRoleResponse
        {
            Id = role.Id,
            Title = role.Title
        };
    }
}