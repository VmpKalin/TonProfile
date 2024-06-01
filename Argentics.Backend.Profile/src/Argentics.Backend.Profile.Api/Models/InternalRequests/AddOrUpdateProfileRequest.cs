using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Api.Models.InternalRequests;

public sealed class AddOrUpdateProfileRequest
{
    public string UserId { get; }
    public JObject ProfileJson { get; }

    public AddOrUpdateProfileRequest(string userId, JObject profileJson)
    {
        UserId = userId;
        ProfileJson = profileJson;
    }    
}