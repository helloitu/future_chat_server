using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace future_chat_server.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string userName { get; set; } = null!;

        [BsonElement("Pass")]
        public string userPass { get; set; } = null!;

        List<String> chatsInvolved { get; set; } = null;
    }
}