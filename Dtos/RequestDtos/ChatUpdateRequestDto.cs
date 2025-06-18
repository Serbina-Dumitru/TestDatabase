namespace TestDatabase.Dtos.RequestDtos
{
  public class ChatUpdateRequestDto
  {
    public string SessionToken { get; set; }
    public string ChatID { get; set; }
    public string ChatName { get; set; }
  }
}
