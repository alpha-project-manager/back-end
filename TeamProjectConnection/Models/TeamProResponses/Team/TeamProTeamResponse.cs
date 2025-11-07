namespace TeamProjectConnection.Models.TeamProResponses.Team;

public class TeamProTeamResponse
{
    public TeamProMemberResponse MainCurator { get; set; } = null!;
    
    public TeamProMemberResponse[] AdditionalCurators { get; set; } = null!;
    
    public TeamProMemberResponse[] Students { get; set; } = null!;
}