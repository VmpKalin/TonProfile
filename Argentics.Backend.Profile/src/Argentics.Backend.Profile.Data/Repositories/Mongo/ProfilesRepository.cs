using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Argentics.Backend.Profile.Data.Repositories.Mongo;

public class ProfilesRepository : IProfilesRepository
{
    private const string DbName = "profiles";
    private const string CollectionName = "profiles";

    private readonly IMongoCollection<BsonDocument> _collection;

    public ProfilesRepository(IMongoClient mongoClient)
    {
        _collection = mongoClient
            .GetDatabase(DbName)
            .GetCollection<BsonDocument>(CollectionName);
    }

    public async Task<List<BsonDocument>> GetAllUsers()
    {
        var projection = Builders<BsonDocument>.Projection
            .Include("Username")
            .Include("HighScore");

        return await (await _collection.FindAsync(_ => true, new FindOptions<BsonDocument>()
        {
            Projection = projection
        })).ToListAsync();
    }

    public async Task AddAsync(string entity)
    {
        var document = BsonSerializer.Deserialize<BsonDocument>(entity);
        await _collection.InsertOneAsync(document);
    }

    public async Task<string?> UpdateAsync(string entity)
    {
        var document = BsonSerializer.Deserialize<BsonDocument>(entity);

        var userId = document["user_id"];
        
        var filter = Builders<BsonDocument>.Filter
            .Eq("user_id", userId);

        var update = Builders<BsonDocument>.Update
            .Set("last_modified", DateTime.UtcNow);

        foreach (var el in document)
        {
            // This fields can't be updated
            if (el.Name is "_id" or "user_id")
            {
                continue;
            }

            var field = el.Name;
            var newValue = document[field];
            update = update.Set(field, newValue);
        }

        await _collection.UpdateOneAsync(filter, update);
        return entity;
    }

    public async Task<BsonDocument?> GetByUserIdAsync(string userId)
    {
        var filter = Builders<BsonDocument>.Filter
            .Eq("user_id", userId);

        var projection = Builders<BsonDocument>.Projection
            .Exclude("_id")
            .Exclude("user_id")
            .Exclude("last_modified");

        var profile = await (await _collection.FindAsync(filter, new FindOptions<BsonDocument>()
        { 
            Projection = projection
        })).FirstOrDefaultAsync();
        return profile;
    }

    public async Task<long> CountByUserIdAsync(string userId)
    {
        var filter = Builders<BsonDocument>.Filter
            .Eq("user_id", userId);

        var docsAmount = await _collection.CountDocumentsAsync(filter);
        return docsAmount;
    }

    public Task<TEntity?> DeleteByUserIdAsync<TEntity>(string userId)
    {
        throw new NotImplementedException();
    }
}