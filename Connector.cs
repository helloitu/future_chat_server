using MongoDB.Driver;

namespace future_chat_server
{

    public class Connector
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;
        public Connector(string connectionString, string databaseName)
        {
            _mongoClient = new MongoClient(connectionString);
            _databaseName = databaseName;
        }

        public IMongoDatabase Database => _mongoClient.GetDatabase(_databaseName);
    }

    public interface IConnector
    {
        Connector _connector { get; set; }
    }
}