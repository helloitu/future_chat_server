namespace future_chat_server.Models.Dto
{
    public class ChatMessageInput
    {
        public string idChat {  get; set; }
        public string idUser { get; set; }
        public string userName { get; set; }
        public string message { get; set; }
    }
}
