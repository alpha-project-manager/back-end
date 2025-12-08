using System.ComponentModel.DataAnnotations;

namespace AlphaProjectManager.Controllers.Authorization.Requests;

public class RegisterRequest
{
    [EmailAddress]
    public required string Email { get; set; }
    
    [DataType(DataType.Password)]
    public required string Password { get; set; }
    
    public required bool IsTutor { get; set; }
    
    public required string FirstName { get; set; }

    public string? LastName { get; set; } = "";
    
    public string? Patronymic { get; set; } = "";
}