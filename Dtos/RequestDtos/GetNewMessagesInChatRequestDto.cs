namespace TestDatabase.Dtos.RequestDtos
{
  public class GetNewMessagesInChatRequestDto
  {
    public string SessionToken { get; set; }
    public string ChatID { get; set; }
  }
}
