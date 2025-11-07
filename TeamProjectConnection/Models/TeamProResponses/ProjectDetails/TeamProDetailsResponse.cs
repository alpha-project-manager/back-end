using TeamProjectConnection.Models.TeamProResponses.Periods;

namespace TeamProjectConnection.Models.TeamProResponses.ProjectDetails;

public class TeamProDetailsResponse
{
    public TeamProPeriodResponse Period { get; set; } = null!;
    
    public string Kind { get; set; } = null!;
    
    public string PassportNumber { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string Result { get; set; } = null!;
    
    public string Criteria { get; set; } = null!;
    
    public string Goal { get; set; } = null!;
    
    public string Experts { get; set; } = null!;
    
    public string CustomerOrganizationName { get; set; } = null!;
    
    public string CustomerPersonName { get; set; } = null!;
    
    public TeamProProgramInfoResponse Program { get; set; } = null!;
    
    public string Level { get; set; } = null!;
    
    public string ShortTitle { get; set; } = null!;
    
    public string TitleForDiploma { get; set; } = null!;
    
    public string Category { get; set; } = null!;
    
    public string Competences { get; set; } = null!;
}