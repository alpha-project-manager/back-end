using Domain.Entities;

namespace AlphaProjectManager.Controllers.Users.Requests;

public class UpdateCalendarSettingsRequest
{
    public required string ServerUrl { get; set; }
    
    public string? Login { get; set; }

    public string? Password { get; set; }

    public void ApplyToCalendarSettings(CalendarSettings calendarSettings)
    {
        calendarSettings.ServerUrl = ServerUrl;
        calendarSettings.Login = Login;
        calendarSettings.Password = Password;
    }
}