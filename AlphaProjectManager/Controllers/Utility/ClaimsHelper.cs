using System.Security.Claims;
using Domain;

namespace AlphaProjectManager.Controllers.Utility;

public static class ClaimsHelper
{
    public static bool TryGetUserId(ClaimsPrincipal user, out Guid? userId)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == AuthOptions.ClaimTypeUserId);
        userId = null;
        if (userIdClaim == null)
        {
            return false;
        }

        if (Guid.TryParse(userIdClaim.Value, out var parsedId))
        {
            userId = parsedId;
            return true;
        }
        return false;
    }
}