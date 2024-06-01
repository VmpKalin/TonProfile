using Argentics.Backend.Profile.Core.DTOs;
using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Core.Abstractions;

public interface IProfileService
{
    Task<ProfileCoreResponse> AddOrUpdateAsync(AddOrUpdateUserProfileRequest request);
    Task<ProfileCoreResponse<JObject>> Read(string userId);
    Task<ProfileCoreResponse<List<UserLeaderboardInfo>>> ReadLeaderboards();
}