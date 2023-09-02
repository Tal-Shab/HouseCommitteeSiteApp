using Microsoft.AspNetCore.Components.Authorization;

namespace HouseCommitteeAppUI.Helpers;

public static class AuthenticationStateProviderHelpers
{
    public static async Task<UserModel> GetUserFromAuthAsync(
        this AuthenticationStateProvider provider,
        IUserData userData)
    {
        var authState = await provider.GetAuthenticationStateAsync();
        string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
        return await userData.GetUserFromAutenticationAsync(objectId);
    }
}
