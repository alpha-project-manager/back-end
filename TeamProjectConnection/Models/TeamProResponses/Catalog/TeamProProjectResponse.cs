namespace TeamProjectConnection.Models.TeamProResponses.Catalog;

public class TeamProProjectResponse
{
    public Guid Id { get; set; }
    
    public bool IsActive { get; set; }
    
    public string ProjectKind { get; set; } = null!;
    
    public int InstanceNumber { get; set; }
    
    public string PassportNumber { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    
    public string CustomerOrganizationName { get; set; } = null!;
    
    public int NumberOfIterations { get; set; }
    
    public int NumberOfStudents { get; set; }

    public TeamProTutorResponse MainCurator { get; set; } = null!;
    
    public List<TeamProStudentResponse> Students { get; set; } = null!;
    
    public TeamProIterationResponse CurrentIteration { get; set; } = null!;
}