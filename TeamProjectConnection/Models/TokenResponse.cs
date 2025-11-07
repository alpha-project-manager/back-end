namespace TeamProjectConnection.Models;

public class TokenResponse
{
    public required string AccessToken { get; set; }
    
    public required int ExpiresIn { get; set; }
    
    public string RefreshToken { get; set; }
    
    public int RefreshExpiresIn { get; set; }
    
    public string IdToken { get; set; }
}