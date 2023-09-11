using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace future_chat_server.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id{ get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string idUser { get; set; }
        public string userName { get; set; }
        public string message { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

    }

    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string chatName { get; set; } = null!;

        [BsonElement("Pass")]
        public string chatPass { get; set; } = null!;

        public List<ChatMessage> messages { get; set; } = null;

        public List<User> adminUsers { get; set; } = null;
    }
}