using TeamProjectConnection.Models;

namespace TeamProjectConnection.Intefaces;

public interface ITeamProAuthManager
{
    public Task<bool> TryAuthAsync(string login, string password);

    public bool IsAuthorized();

    public bool TryGetSavedToken(out TokenResponse? tokenInfo);
}