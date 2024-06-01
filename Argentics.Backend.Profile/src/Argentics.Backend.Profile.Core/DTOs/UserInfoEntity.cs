using MongoDB.Bson;

namespace Argentics.Backend.Profile.Core.DTOs
{
    public  class UserInfoEntity
    {
        public ObjectId Id { get; set; }
        public int HighScore { get; set; }
        public string Username { get; set; }
    }
}
