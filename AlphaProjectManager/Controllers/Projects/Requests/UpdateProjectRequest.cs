using Domain.Entities;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.Projects.Requests;

public class UpdateProjectRequest
{
    public Guid? CaseId { get; set; }
    
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public Guid? TutorId { get; set; }
    
    public string? MeetingUrl { get; set; }
    
    public string? TeamTitle { get; set; }
    
    public ProjectStatus Status { get; set; }
    
    public Semester Semester { get; set; }
    
    public int AcademicYear { get; set; }

    public void ApplyToProject(Project project)
    {
        project.CaseId = CaseId;
        project.Title = Title;
        project.Description = Description;
        project.TutorId = TutorId;
        project.MeetingUrl = MeetingUrl ?? "";
        project.TeamTitle = TeamTitle ?? "";
        project.Status = Status;
        project.Semester = Semester;
        project.AcademicYear = AcademicYear;
    }
}