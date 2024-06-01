using Argentics.Backend.Profile.Core.DTOs;
using Argentics.Backend.Profile.Core.Services;
using Argentics.Backend.Profile.Data.Repositories.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Argentics.Backend.Profile.Core.Tests.Services;

public class ProfileServiceTests
{
    private readonly Mock<IProfilesRepository> _mockProfilesRepository;

    private ProfileService _profileService;
    
    public ProfileServiceTests()
    {
        _mockProfilesRepository = new Mock<IProfilesRepository>();
        Mock<ILogger<ProfileService>> mockLogger = new();

        _profileService = new ProfileService(
            _mockProfilesRepository.Object, 
            mockLogger.Object);
    }
    
    [Fact]
    public async Task ReadSuccess()
    {
        long usersCount = 1;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        var bsonDocument = new BsonDocument();
        _mockProfilesRepository
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(bsonDocument);

        var userId = Guid.NewGuid();
        
        //Act
        var response =  await _profileService.Read(userId);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
    }
    
    [Fact]
    public async Task ReadFailProfileNotExist()
    {
        long usersCount = 0;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        var userId = Guid.NewGuid();
        
        //Act
        var response =  await _profileService.Read(userId);
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
    }
    
    [Fact]
    public async Task ReadFailMongoDbError()
    {
        long usersCount = 2;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        var userId = Guid.NewGuid();
        //Act
        var response =  await _profileService.Read(userId);
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.True(response.FailureReasons?.Contains(FailureReason.MongoDbError));
    }
    
    [Fact]
    public async Task ReadFailProfileIsNull()
    {
        long usersCount = 1;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        var userId = Guid.NewGuid();
        
        //Act
        var response =  await _profileService.Read(userId);
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.True(response.FailureReasons?.Contains(FailureReason.NotFound));
    }
    
    [Fact]
    public async Task AddOrUpdateAsyncSuccessProfileExists()
    {
        var profileJson = "{'userName':'someUsername'}";
        var profileJObj = JObject.Parse(profileJson);
        var request = new AddOrUpdateUserProfileRequest()
        {
            ProfileJson = profileJObj,
            UserId = Guid.NewGuid()
        };
        
        long usersCount = 1;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        var bsonDocument = new BsonDocument();
        _mockProfilesRepository
            .Setup(s => s.GetByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(bsonDocument);

        //Act
        var response =  await _profileService.AddOrUpdateAsync(request);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
    }
    
    [Fact]
    public async Task AddOrUpdateAsyncSuccessProfileNotExist()
    {
        var profileJson = "{'userName':'someUsername'}";
        var profileJObj = JObject.Parse(profileJson);
        var request = new AddOrUpdateUserProfileRequest()
        {
            ProfileJson = profileJObj,
            UserId = Guid.NewGuid()
        };

        long usersCount = 0;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        //Act
        var response =  await _profileService.AddOrUpdateAsync(request);
        Assert.NotNull(response);
        Assert.True(response.IsSuccess);
    }
    
    [Fact]
    public async Task AddOrUpdateAsyncFailMongoDbError()
    {
        var profileJson = "{'userName':'someUsername'}";
        var profileJObj = JObject.Parse(profileJson);
        var request = new AddOrUpdateUserProfileRequest()
        {
            ProfileJson = profileJObj,
            UserId = Guid.NewGuid()
        };
        
        var usersCount = 2;
        _mockProfilesRepository
            .Setup(s => s.CountByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync(usersCount);

        var userId = Guid.NewGuid();
        //Act
        var response =  await _profileService.AddOrUpdateAsync(request);
        Assert.NotNull(response);
        Assert.False(response.IsSuccess);
        Assert.True(response.FailureReasons?.Contains(FailureReason.MongoDbError));
    }
}