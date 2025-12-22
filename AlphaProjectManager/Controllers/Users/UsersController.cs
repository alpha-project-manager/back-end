using AlphaProjectManager.Controllers.Base.Responses;
using AlphaProjectManager.Controllers.ProjectCases.Responses;
using AlphaProjectManager.Controllers.Shared;
using AlphaProjectManager.Controllers.Users.Requests;
using AlphaProjectManager.Controllers.Users.Responses;
using AlphaProjectManager.Controllers.Utility;
using Application.DataQuery;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AlphaProjectManager.Controllers.Users;

[Route("/api/users")]
public class UsersController : ControllerBase
{
    private readonly BaseService<User> _userService;
    private readonly BaseService<CalendarSettings> _calendarSettingService;

    public UsersController(BaseService<User> userService, BaseService<CalendarSettings> calendarSettingService)
    {
        _userService = userService;
        _calendarSettingService = calendarSettingService;
    }
    
    /// <summary>
    /// Обновить настройки подключения календаря
    /// </summary>
    [HttpPut("calendar-settings")]
    [ProducesResponseType(typeof(CalendarSettingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseStatusResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromBody] UpdateCalendarSettingsRequest dto)
    {
        if (!ClaimsHelper.TryGetUserId(User, out var userId))
        {
            return SharedResponses.FailedRequest("User's claim with ID not found.");
        }
        var foundUser = await _userService.GetByIdOrDefaultAsync(userId!.Value);
        if (foundUser == null)
        {
            return SharedResponses.NotFoundObjectResponse<User>(userId.Value);
        }
        var calendarSettings = (await _calendarSettingService.GetByIdOrDefaultAsync(foundUser.CalendarSettingsId))!;
        dto.ApplyToCalendarSettings(calendarSettings);
        await _calendarSettingService.UpdateAsync(calendarSettings);
        return Ok(CalendarSettingResponse.FromCalendarSettings(calendarSettings));
    }
}