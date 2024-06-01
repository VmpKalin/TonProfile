using System.Text;
using Argentics.Backend.Profile.Core.Abstractions;
using Argentics.Backend.Profile.Core.DTOs;
using Argentics.Backend.Profile.Core.Tools;
using Argentics.Backend.Profile.Data.Repositories.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Core.Services;

public class ProfileService : IProfileService
{
    private readonly IProfilesRepository _profilesRepository;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(
        IProfilesRepository profilesRepository,
        ILogger<ProfileService> logger)
    {
        _profilesRepository = profilesRepository;
        _logger = logger;
    }

    public async Task<ProfileCoreResponse> AddOrUpdateAsync(AddOrUpdateUserProfileRequest request)
    {
        bool isProfileExists = false;

        try
        {
            isProfileExists = await CheckIfProfileExists(request.UserId);

            var profileAsJson = request.ProfileJson!.ToString();
            if (isProfileExists)
            {
                await _profilesRepository.UpdateAsync(profileAsJson);
            }
            else
            {
                await _profilesRepository.AddAsync(profileAsJson);
            }

            return ProfileCoreResponse.Success();
        }
        catch (Exception ex)
        {
            var sb = new StringBuilder("Error occured while")
                .Append(isProfileExists ? " updating " : " adding ").Append("user's profile");
            _logger.LogError(ex, sb.ToString());
            return ProfileCoreResponse.Fail(new[] {FailureReason.MongoDbError});
        }
    }

    public async Task<ProfileCoreResponse<JObject>> Read(string userId)
    {
        bool isProfileExists = false;

        try
        {
            isProfileExists = await CheckIfProfileExists(userId);
            if (!isProfileExists)
            {
                _logger.LogDebug("Profile for user {userID} wasn't found", userId);
                return ProfileCoreResponse<JObject>.Fail(null, new[] {FailureReason.NotFound});
            }

            var profile = await _profilesRepository.GetByUserIdAsync(userId.ToString());
            if (profile is null)
            {
                _logger.LogDebug("Profile for user {userID} wasn't found", userId);
                return ProfileCoreResponse<JObject>.Fail(null, new[] {FailureReason.NotFound});
            }

            var profileJsonStr = profile.ToJson();
            _logger.LogDebug("Profile for user {userID} found. Profile: {profile}", userId, profileJsonStr);
            var profileJson = JObject.Parse(profileJsonStr);
            return ProfileCoreResponse<JObject>.Success(profileJson);
        }
        catch (Exception ex)
        {
            var sb = new StringBuilder("Error occured while")
                .Append(isProfileExists ? " updating " : " adding ").Append("user's profile");
            _logger.LogError(ex, sb.ToString());
            return ProfileCoreResponse<JObject>.Fail(null, new[] {FailureReason.MongoDbError});
        }
    }

    public async Task<ProfileCoreResponse<List<UserLeaderboardInfo>>> ReadLeaderboards()
    {
        try
        {
            var mongoProfiles = await _profilesRepository.GetAllUsers();
            if (mongoProfiles is null)
            {
                return ProfileCoreResponse<List<UserLeaderboardInfo>>.Fail(null, new[] { FailureReason.NotFound });
            }

            var profiles = mongoProfiles
                .Select(x => BsonSerializer.Deserialize<UserInfoEntity>(x))
                .Where(x => x != null)
                .ToList();

            var leaderboards = profiles.Select(x => new UserLeaderboardInfo
            {
                Username = x.Username,
                Score = x.HighScore
            }).ToList();

            return ProfileCoreResponse<List<UserLeaderboardInfo>>.Success(leaderboards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while getting leaderboards");
            return ProfileCoreResponse<List<UserLeaderboardInfo>>.Fail(null, new[] { FailureReason.MongoDbError });
        }
    }


    private async Task<bool> CheckIfProfileExists(string userId)
    {
        var userProfilesAmount = await _profilesRepository.CountByUserIdAsync(userId.ToString());
        if (userProfilesAmount > 1)
        {
            _logger.LogCritical("Data inconsistency. User {userID} has more than 1 profile record", userId.ToString());
            throw new Exception(FailureReason.DateInconsistency.ToString());
        }

        var isUserProfileExists = userProfilesAmount is 1;
        if (isUserProfileExists)
        {
            _logger.LogDebug("Profile for user {userID} exists and will be updated", userId.ToString());
            return true;
        }

        _logger.LogDebug("Profile for user {userID} is not exists and will be created", userId.ToString());
        return false;
    }
}