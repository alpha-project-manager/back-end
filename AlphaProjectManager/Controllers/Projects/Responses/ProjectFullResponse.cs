using AlphaProjectManager.Controllers.Projects.ControlPointsInProject.Responses;
using AlphaProjectManager.Controllers.Projects.Meetings.Responses;
using AlphaProjectManager.Controllers.Students.Responses;
using AlphaProjectManager.Controllers.Tutors.Responses;
using Domain.Entities;
using Domain.Enums;

namespace AlphaProjectManager.Controllers.Projects.Responses;

public class ProjectFullResponse
{
    public required Guid Id { get; set; }
    
    public Guid? CaseId { get; set; }
    
    public required string Title { get; set; }
    
    public required string Description { get; set; }
    
    public string? MeetingUrl { get; set; }
    
    public required string TeamTitle { get; set; }
    
    public required ProjectStatus Status { get; set; }
    
    public required Semester Semester { get; set; }
    
    public required int AcademicYear { get; set; }
    
    public TutorResponse? Tutor { get; set; }
    
    public required List<ControlPointProjectResponse> ControlPoints { get; set; }
    
    public required List<StudentResponse> Students { get; set; }
    
    public required List<MeetingBriefResponse> Meetings { get; set; }

    public static ProjectFullResponse FromDomainEntities(Project project, ControlPointInProject[] controlPoints,
        Student[] students, Dictionary<Meeting, TodoTask[]>  meetings)
    {
        return new ProjectFullResponse
        {
            Id = project.Id,
            CaseId = project.CaseId,
            Title = project.Title,
            Description = project.Description,
            MeetingUrl = project.MeetingUrl,
            TeamTitle = project.TeamTitle,
            Status = project.Status,
            Semester = project.Semester,
            AcademicYear = project.AcademicYear,
            Tutor = project.Tutor == null ? null : TutorResponse.FromTutor(project.Tutor),
            ControlPoints = controlPoints.Select(ControlPointProjectResponse.FromControlPoint).ToList(),
            Students = students.Select(StudentResponse.FromStudent).ToList(),
            Meetings = meetings.Select(kv => MeetingBriefResponse.FromMeeting(kv.Key, kv.Value)).ToList()
        };
    }
}