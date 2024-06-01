using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Core.DTOs;

/// <summary>
/// Request to save new or update existed user profile
/// </summary>
public sealed class AddOrUpdateUserProfileRequest
{
    /// <summary>
    /// UserID received from the access JWT token
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// User profile as a JObject
    /// </summary>
    public JObject? ProfileJson { get; set; }
}