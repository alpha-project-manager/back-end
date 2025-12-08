using AlphaProjectManager.Controllers.Base.Responses;

namespace AlphaProjectManager.Controllers.Authorization.Responses;

public class RegisterResponse : BaseStatusResponse
{
    public Guid? UserId { get; set; }
    
    public required string AccessToken { get; set; }
}