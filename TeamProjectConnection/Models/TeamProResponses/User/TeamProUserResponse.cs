namespace TeamProjectConnection.Models.TeamProResponses.User;

public class TeamProUserResponse
{
    public int Id { get; set; }
    
    public TeamProPersonResponse Person { get; set; } = null!;
    
    public TeamProContactsResponse Contacts { get; set; } = null!;
    
    public TeamProRolesResponse Roles { get; set; } = null!;
}