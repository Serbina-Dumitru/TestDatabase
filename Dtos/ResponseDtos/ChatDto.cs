namespace TestDatabase.Dtos.ResponseDtos
{
  public class ChatDto
  {
    public int ChatID { get; set; }
    public string ChatName { get; set; }
    public MessageToClientDto? LastMessage {  get; set; }
  }
}
