using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.Meetings.Requests;
using AlphaProjectManager.Controllers.Meetings.Responses;
using AlphaProjectManager.Controllers.ProjectCases.Responses;
using AlphaProjectManager.Controllers.Shared;
using Application.DataQuery;
using Application.Services;
using Application.Services.Meetings;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.Meetings;

[Route("/api/projects/{projectId:guid}/meetings")]
public class MeetingsController : ControllerBase
{
    private readonly MeetingService _meetingService;

    public MeetingsController(MeetingService meetingService)
    {
        _meetingService = meetingService;
    }
    
    /// <summary>
    /// Создать новое собрание
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MeetingFullResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateNewMeeting([FromRoute] Guid projectId, [FromBody] CreateMeetingRequest dto)
    {
        var meetingInfo = await _meetingService.CreateNewMeeting(projectId, dto.DateTime, dto.TodoTasks);
        if (meetingInfo == null)
        {
            return SharedResponses.NotFoundObjectResponse<Project>(projectId);
        }
        return Ok(MeetingFullResponse.FromDomainEntities(meetingInfo!.Meeting, meetingInfo.TodoTasks, 
            meetingInfo.StudentAttendances, meetingInfo.TutorAttendances));
    }
    
    /// <summary>
    /// Получить краткую информацию о всех собраниях
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(MeetingBriefListResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBrief([FromRoute] Guid projectId)
    {
        var meetings = await _meetingService.GetMeetingsForProject(projectId);
        return Ok(new MeetingBriefListResponse
        {
            Meetings = meetings.Select(kv => MeetingBriefResponse.FromMeeting(kv.Key, kv.Value)).ToList()
        });
    }
    
    /// <summary>
    /// Получить полную информацию о собрании по Id
    /// </summary>
    [HttpGet("{meetingId:guid}")]
    [ProducesResponseType(typeof(MeetingFullResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailsById([FromRoute] Guid meetingId)
    {
        var meetingInfo = await _meetingService.GetFullMeetingInfoById(meetingId);
        if (meetingInfo == null)
        {
            return SharedResponses.NotFoundObjectResponse<Meeting>(meetingId);
        }
        return Ok(MeetingFullResponse.FromDomainEntities(meetingInfo.Meeting, meetingInfo.TodoTasks, 
            meetingInfo.StudentAttendances, meetingInfo.TutorAttendances));
    }
    
    /// <summary>
    /// Обновить информацию о собрании по Id
    /// </summary>
    [HttpPut("{meetingId:guid}")]
    [ProducesResponseType(typeof(MeetingFullResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDetailsById([FromRoute] Guid meetingId, [FromBody] UpdateMeetingRequest dto)
    {
        var meetingInfo = await _meetingService.GetFullMeetingInfoById(meetingId);
        if (meetingInfo == null)
        {
            return SharedResponses.NotFoundObjectResponse<Meeting>(meetingId);
        }
        dto.ApplyToMeeting(meetingInfo.Meeting);
        await _meetingService.UpdateAsync(meetingInfo.Meeting);
        
        return Ok(MeetingFullResponse.FromDomainEntities(meetingInfo.Meeting, meetingInfo.TodoTasks, 
            meetingInfo.StudentAttendances, meetingInfo.TutorAttendances));
    }
    
    /// <summary>
    /// Удалить собрание
    /// </summary>
    [HttpDelete("{meetingId:guid}")]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMeeting([FromRoute] Guid meetingId)
    {
        var result = await _meetingService.DeleteMeeting(meetingId);
        if (result.Completed)
        {
            return SharedResponses.SuccessRequest("Meeting deleted.");
        }
        return SharedResponses.NotFoundObjectResponse<Meeting>(meetingId);
    }
    
    /// <summary>
    /// Изменить статус посещения собрания студентом с Id = studentId
    /// </summary>
    [HttpPut("{meetingId:guid}/attendances/student/{studentId:guid}")]
    [ProducesResponseType(typeof(AttendanceInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStudentAttendanceById([FromRoute] Guid meetingId, [FromRoute] Guid studentId,
        [FromBody] UpdateAttendanceRequest dto)
    {
        var attendance = await _meetingService.SetAttendanceOfStudent(meetingId, studentId, dto.Attended);
        return Ok(AttendanceInfoResponse.FromStudentAttendance(attendance));
    }
    
    /// <summary>
    /// Изменить статус посещения собрания куратором с Id = tutorId
    /// </summary>
    [HttpPut("{meetingId:guid}/attendances/tutor/{tutorId:guid}")]
    [ProducesResponseType(typeof(AttendanceInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTutorAttendanceById([FromRoute] Guid meetingId, [FromRoute] Guid tutorId,
        [FromBody] UpdateAttendanceRequest dto)
    {
        var attendance = await _meetingService.SetAttendanceOfTutor(meetingId, tutorId, dto.Attended);
        return Ok(AttendanceInfoResponse.FromTutorAttendance(attendance));
    }
}