namespace TeamProjectConnection.Models.TeamProResponses.User;

public class TeamProRolesResponse
{
    public bool IsAdmin { get; set; }
    
    public bool IsAuditor { get; set; }
    
    public bool IsCurator { get; set; }
    
    public bool IsExternal { get; set; }
    
    public bool IsStudent { get; set; }
    
    public bool IsExpert { get; set; }
    
    public bool IsProgramHead { get; set; }
}