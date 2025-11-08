namespace AlphaProjectManager.Controllers.Projects.Requests;

public class ImportProjectsFromTeamProRequest
{
    public required string Login { get; set; }
    
    public required string Password { get; set; }
}