using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.TestController.Responses;

public class GetProjectListResponse : BaseStatusResponse
{
    public List<string> Projects { get; set; } = [];
}