namespace TestDatabase.Dtos.RequestDtos
{
  public class GetMessagesInChatRequestDto
  {
    public string SessionToken { get; set; }
    public string ChatID { get; set; }
  }
}
