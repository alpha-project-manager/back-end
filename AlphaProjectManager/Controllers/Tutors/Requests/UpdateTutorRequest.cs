using Domain.Entities;

namespace AlphaProjectManager.Controllers.Tutors.Requests;

public class UpdateTutorRequest
{
    public required string FirstName { get; set; }

    public string? LastName { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";

    public void ApplyToTutor(Tutor tutor)
    {
        tutor.FirstName = FirstName;
        tutor.LastName = LastName;
        tutor.Patronymic = Patronymic;
    }
}