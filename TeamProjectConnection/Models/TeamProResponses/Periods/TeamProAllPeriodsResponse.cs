namespace TeamProjectConnection.Models.TeamProResponses.Periods;

public class TeamProAllPeriodsResponse
{
    public TeamProPeriodResponse[] Periods { get; set; }
    
    public required TeamProPeriodResponse Current { get; set; }
}