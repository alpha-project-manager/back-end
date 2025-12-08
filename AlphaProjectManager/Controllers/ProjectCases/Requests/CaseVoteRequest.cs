using Domain.Enums;

namespace AlphaProjectManager.Controllers.ProjectCases.Requests;

public class CaseVoteRequest
{
    public CaseReactionType ReactionType { get; set; }
}