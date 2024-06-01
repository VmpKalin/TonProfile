using System.Net;
using Argentics.Backend.Profile.Api.Controllers;
using Argentics.Backend.Profile.Api.Models.InternalRequests;
using Argentics.Backend.Profile.Core.Abstractions;
using Argentics.Backend.Profile.Core.DTOs;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Api.Tests.Controllers;

public class ProfilesControllerTests
{
    private readonly Mock<IProfileService> _mockProfileService;
    private readonly Mock<IValidator<AddOrUpdateProfileRequest>> _mockSaveProfileValidator;
    private readonly Mock<ILogger<ProfilesController>> _mockLogger = new();

    public ProfilesControllerTests()
    {
        _mockProfileService = new Mock<IProfileService>();
        _mockSaveProfileValidator = new Mock<IValidator<AddOrUpdateProfileRequest>>();
    }
    
    [Fact]
    public async Task AddOrUpdateSuccess()
    {
        ProfilesController controller = new ProfilesController(
            _mockProfileService.Object, 
            _mockSaveProfileValidator.Object, 
            _mockLogger.Object);

        var accessToken =
            "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzU1MTQ4MTAsImV4cCI6MTcwNzA1MDgxMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsInVzZXJpZCI6ImIxNTIxZDFkLTlkNjgtNGM5Mi1hMTUxLTNhMTE4N2M0MWFiNiJ9.adommBbCbah8sBb4-Mirr1Sq031LW5TUHG7ebMoXTIc";
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = accessToken;

        _mockSaveProfileValidator
            .Setup(s =>
                s.ValidateAsync(It.IsAny<AddOrUpdateProfileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _mockProfileService
            .Setup(s => s.AddOrUpdateAsync(It.IsAny<AddOrUpdateUserProfileRequest>()))
            .ReturnsAsync(ProfileCoreResponse.Success());

        var request = new JObject();
        
        //Act
        var result = await controller.AddOrUpdate(request);
        
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task AddOrUpdateFailUnauthorized()
    {
        ProfilesController controller = new ProfilesController(
            _mockProfileService.Object, 
            _mockSaveProfileValidator.Object, 
            _mockLogger.Object);

        var accessToken =
            "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzU1MTQ4MTAsImV4cCI6MTcwNzA1MDgxMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsInVzZXJJZCI6ImIxNTIxZDFkLTlkNjgtNGM5Mi1hMTUxLTNhMTE4N2M0MWFiNiJ9.6lg52iKP5qV0ek5Fmu9a0pAB8JYFJt6thY2tQ6MdVG8";
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = accessToken;

        var request = new JObject();
        
        //Act
        var result = await controller.AddOrUpdate(request);
        
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(result.StatusCode,HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task ReadSuccess()
    {
        ProfilesController controller = new ProfilesController(
            _mockProfileService.Object, 
            _mockSaveProfileValidator.Object, 
            _mockLogger.Object);

        var accessToken =
            "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzU1MTQ4MTAsImV4cCI6MTcwNzA1MDgxMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsInVzZXJpZCI6ImIxNTIxZDFkLTlkNjgtNGM5Mi1hMTUxLTNhMTE4N2M0MWFiNiJ9.adommBbCbah8sBb4-Mirr1Sq031LW5TUHG7ebMoXTIc";
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = accessToken;

        _mockProfileService
            .Setup(s => s.Read(It.IsAny<Guid>()))
            .ReturnsAsync(ProfileCoreResponse<JObject>.Success(new JObject()));

        //Act
        var result = await controller.Read();
        
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task ReadFailUnauthorized()
    {
        ProfilesController controller = new ProfilesController(
            _mockProfileService.Object, 
            _mockSaveProfileValidator.Object, 
            _mockLogger.Object);

        var accessToken =
            "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE2NzU1MTQ4MTAsImV4cCI6MTcwNzA1MDgxMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSIsInVzZXJJZCI6ImIxNTIxZDFkLTlkNjgtNGM5Mi1hMTUxLTNhMTE4N2M0MWFiNiJ9.6lg52iKP5qV0ek5Fmu9a0pAB8JYFJt6thY2tQ6MdVG8";
        controller.ControllerContext = new ControllerContext();
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers["Authorization"] = accessToken;

        //Act
        var result = await controller.Read();
        
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(result.StatusCode,HttpStatusCode.Unauthorized);
    }
}