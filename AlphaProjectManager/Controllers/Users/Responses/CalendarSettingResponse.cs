using Domain.Entities;

namespace AlphaProjectManager.Controllers.Users.Responses;

public class CalendarSettingResponse
{
    public required string ServerUrl { get; set; }
    
    public string? Login { get; set; }

    public string? Password { get; set; }

    public static CalendarSettingResponse FromCalendarSettings(CalendarSettings calendarSettings)
    {
        return new CalendarSettingResponse
        {
            ServerUrl = calendarSettings.ServerUrl,
            Login = calendarSettings.Login,
            Password = calendarSettings.Password
        };
    }
}