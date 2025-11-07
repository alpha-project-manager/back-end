namespace TeamProjectConnection.Models.TeamProResponses.ProjectDetails;

public class TeamProProgramInfoResponse
{
    public string Code { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    
    public TeamProProgramHeadResponse ProgramHead { get; set; } = null!;
}