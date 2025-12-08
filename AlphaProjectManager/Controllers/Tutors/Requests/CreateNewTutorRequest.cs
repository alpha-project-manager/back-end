using Domain.Entities;

namespace AlphaProjectManager.Controllers.Tutors.Requests;

public class CreateNewTutorRequest
{
    public required string FirstName { get; set; }

    public string? LastName { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";

    public Tutor ToTutorEntity()
    {
        return new Tutor
        {
            Id = Guid.NewGuid(),
            FirstName = FirstName,
            LastName = LastName,
            Patronymic = Patronymic
        };
    }
}