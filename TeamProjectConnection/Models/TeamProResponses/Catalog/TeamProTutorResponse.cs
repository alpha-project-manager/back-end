namespace TeamProjectConnection.Models.TeamProResponses.Catalog;

public class TeamProTutorResponse
{
    public string Fullname { get; set; } = null!;
    
    public string AvatarUrl { get; set; } = null!;
    
    public bool IsExternal { get; set; }
}