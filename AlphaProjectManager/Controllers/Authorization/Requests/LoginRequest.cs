using System.ComponentModel.DataAnnotations;

namespace AlphaProjectManager.Controllers.Authorization.Requests;

public class LoginRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}