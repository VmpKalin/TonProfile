using MongoDB.Bson;
using MongoDB.Driver;

namespace Argentics.Backend.Profile.Data.Repositories.Mongo;

public interface IProfilesRepository
{
    Task AddAsync(string entity);
    Task<List<BsonDocument>> GetAllUsers();
    Task<string?> UpdateAsync(string entity);
    Task<BsonDocument?> GetByUserIdAsync(string userId);
    Task<long> CountByUserIdAsync(string userId);
}