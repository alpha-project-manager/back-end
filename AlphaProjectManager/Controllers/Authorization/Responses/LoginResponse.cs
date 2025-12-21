using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.Authorization.Responses;

public class LoginResponse : BaseStatusResponse
{
    public required Guid? UserId { get; set; }
    
    public required string AccessToken { get; set; }
    
    public string Fullname { get; set; }
}