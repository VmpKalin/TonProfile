using Argentics.Backend.Profile.Data.Repositories.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Argentics.Backend.Profile.Data;

public static class DependencyInjection
{
    public static void AddDataAccessLevel(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMongoClient>(sp => new MongoClient(configuration["ConnectionStrings:MongoDb"]));
        services.AddScoped<IProfilesRepository, ProfilesRepository>();
    }
}