using AlphaProjectManager.Controllers.Tutors.Responses;
using Domain.Entities;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.Projects.Responses;

public class ProjectBriefResponse
{
    public required Guid Id { get; set; }
    
    public required string Title { get; set; }
    
    public required string TeamTitle { get; set; }
    
    public required ProjectStatus Status { get; set; }
    
    public required Semester Semester { get; set; }
    
    public required int AcademicYear { get; set; }
    
    public TutorResponse? Tutor { get; set; }

    public static ProjectBriefResponse FromProject(Project project)
    {
        return new ProjectBriefResponse
        {
            Id = project.Id,
            Title = project.Title,
            TeamTitle = project.TeamTitle,
            Status = project.Status,
            Semester = project.Semester,
            AcademicYear = project.AcademicYear,
            Tutor = project.Tutor != null ? TutorResponse.FromTutor(project.Tutor) : null
        };
    }
}