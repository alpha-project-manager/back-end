namespace TeamProjectConnection.Models.TeamProResponses.Catalog;

public class TeamProCatalogResponse
{ 
    public List<TeamProProjectResponse> Items { get; set; } = null!;
    
    public int Total { get; set; }
}