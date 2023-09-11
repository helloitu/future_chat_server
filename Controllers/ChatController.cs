using future_chat_server.Models;
using future_chat_server.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;

namespace future_chat_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly Connector _connector;

        private readonly ILogger<ChatController> _logger;

        public ChatController(Connector conbase, ILogger<ChatController> logger)
        {
            _logger = logger;
            _connector = conbase;

        }

        [HttpGet]
        [Route("GetChats")]
        public ActionResult<IEnumerable<Chat>> GetChats()
        {
            var _con = _connector.Database.GetCollection<Chat>("Chat");
            return Ok(_con.Find(_ => true).ToList());
        }

        [HttpGet]
        [Route("GetChatMessages")]
        public ActionResult<IEnumerable<ChatMessage>> GetMessages(string chatUUID)
        {
            var _con = _connector.Database.GetCollection<Chat>("Chat");
            var filter = Builders<Chat>.Filter.Eq(u => u.Id, chatUUID);
            var result = _con.Find(filter).FirstOrDefault();
            if(result.messages == null || result.messages.Count == 0)
               return Ok(new List<ChatMessage>());

            return Ok(result.messages.ToList());
        }


        [HttpPost]
        [Route("CreateUser")]
        public string CreateUser([FromBody] UserInput args)
        {
            var _con = _connector.Database.GetCollection<User>("User");
            var upcomingUser = new User
            {
                userName = args.userName,
                userPass = args.userPass
            };
            _con.InsertOne(upcomingUser);
            return upcomingUser.Id;
        }


        [HttpPost]
        [Route("CreateChat")]
        public string CreateChat([FromBody] ChatInput args)
        {
            var _con = _connector.Database.GetCollection<Chat>("Chat");
            var upcomingChat = new Chat
            {
                chatName = args.Nome,
                chatPass = args.Pass,
                messages = new List<ChatMessage>()
            };
            _con.InsertOne(upcomingChat);
            return upcomingChat.Id;
        }

        [HttpPost]
        [Route("CreateMessage")]
        public ActionResult<bool> CreateMessage([FromBody] ChatMessageInput args)
        {
            var _con = _connector.Database.GetCollection<Chat>("Chat");
            var upcomingChatMessage = new ChatMessage
            {
                idUser = args.idUser,
                message = args.message,
                userName = args.userName
            };
            var filter = Builders<Chat>.Filter.Eq(u => u.Id, args.idChat);
            var result = _con.Find(filter).FirstOrDefault();
            result.messages.Add(upcomingChatMessage);
            // Atualize o chat na coleção
            var update = Builders<Chat>.Update
                .Set(c => c.messages, result.messages);

            var updateResult = _con.UpdateOne(filter, update);

            if (updateResult.ModifiedCount == 1)
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(false);
            }


        }

    }
}