using future_chat_server.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace future_chat_server.Models
{
    public class ChatConfig
    {
        public string conString {  get; set; }
        public string chatDatabase { get; set; }
        public string userCollection { get; set; }
        public string chatCollection { get; set; }

    }
}
