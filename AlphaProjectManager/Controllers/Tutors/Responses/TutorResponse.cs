using Domain.Entities;

namespace AlphaProjectManager.Controllers.Tutors.Responses;

public class TutorResponse
{
    public required Guid Id { get; set; }
    
    public required string FirstName { get; set; }

    public string? LastName { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";
    
    public required string FullName { get; set; }

    public static TutorResponse FromTutor(Tutor tutor)
    {
        return new TutorResponse
        {
            Id = tutor.Id,
            FullName = tutor.FullName,
            FirstName = tutor.FirstName,
            LastName = tutor.LastName,
            Patronymic = tutor.Patronymic
        };
    }
}