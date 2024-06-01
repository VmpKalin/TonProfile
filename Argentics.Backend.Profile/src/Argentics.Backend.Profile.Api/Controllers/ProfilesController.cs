using System.Net;
using Argentics.Backend.Profile.Api.Extensions;
using Argentics.Backend.Profile.Api.Models.InternalRequests;
using Argentics.Backend.Profile.Api.Models.Responses;
using Argentics.Backend.Profile.Core.Abstractions;
using Argentics.Backend.Profile.Core.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Api.Controllers;

[ApiController]
[Route("/profile/[controller]")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly IValidator<AddOrUpdateProfileRequest> _saveProfileValidator;
    private readonly ILogger<ProfilesController> _logger;

    public ProfilesController(
        IProfileService profileService,
        IValidator<AddOrUpdateProfileRequest> saveProfileValidator,
        ILogger<ProfilesController> logger)
    {
        _profileService = profileService;
        _saveProfileValidator = saveProfileValidator;
        _logger = logger;
    }

    [HttpPost]
    [Route("")]
    public async Task<ApiResponse> AddOrUpdate([FromBody] JObject profile, [FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogDebug("Profile {profile} will not be processed, because user is not authorized",
                profile.ToJson());
            return ApiResponse.Fail(HttpStatusCode.Unauthorized);
        }

        var request = new AddOrUpdateProfileRequest(userId, profile);

        var validationResult = await _saveProfileValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.GetValidationErrorsAsArray();
            _logger.LogDebug(
                "Profile add or update request will not processed because of failed validation. " +
                "Request: {request}, validation errors: {errors}", request, string.Join(',', validationErrors));
            return ApiResponse.Fail(HttpStatusCode.BadRequest, validationErrors);
        }

        // Here should be added a JSON validation.
        // Newtonsoft.Schema is a perfect package for this validation

        var savingResponse = await _profileService.AddOrUpdateAsync(new AddOrUpdateUserProfileRequest()
        {
            UserId = request.UserId, ProfileJson = request.ProfileJson
        });
        if (!savingResponse.IsSuccess)
        {
            var errors = savingResponse.CoreResponseErrorsToString();
            _logger.LogDebug("Add or update request processing failed. Request: {request}, errors: {errors}",
                request, errors);
            return ApiResponse.Fail(HttpStatusCode.InternalServerError, errors!);
        }

        _logger.LogDebug("Add or insert request successfully completed for user: {userId}. Request: {request}",
            userId, request);
        return ApiResponse.Success();
    }

    [HttpGet]
    [Route("")]
    public async Task<ApiResponse<JObject>> Read([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return ApiResponse<JObject>.Fail(null, HttpStatusCode.Unauthorized);
        }

        var readProfileResult = await _profileService.Read(userId);
        if (!readProfileResult.IsSuccess)
        {
            var errors = readProfileResult.CoreResponseErrorsToString();
            _logger.LogDebug("Error(-s) occured while receiving profile for user {userID}. Errors: {errors}",
                userId.ToString(), errors);
            return ApiResponse<JObject>.Fail(null, HttpStatusCode.NotFound, errors!);
        }

        _logger.LogDebug("Profile for user {userID} successfully received", userId.ToString());
        return ApiResponse<JObject>.Success(readProfileResult.Body);
    }

    [HttpGet]
    [Route("Leaderboards")]
    public async Task<ApiResponse<List<UserLeaderboardInfo>>> ReadLeaderboards()
    {
        var readProfileResult = await _profileService.ReadLeaderboards();
        if (!readProfileResult.IsSuccess)
        {
            var errors = readProfileResult.CoreResponseErrorsToString();
            _logger.LogDebug("Error(-s) occured while receiving leaderboards. Errors: {errors}", errors);
            return ApiResponse<List<UserLeaderboardInfo>>.Fail(null, HttpStatusCode.NotFound, errors!);
        }

        return ApiResponse<List<UserLeaderboardInfo>>.Success(readProfileResult.Body);
    }
}