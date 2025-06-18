namespace TestDatabase.Dtos.RequestDtos
{
  public class MessageSaveRequestDto
  {
    public string SessionToken { get; set; }
    public string Content { get; set; }
    public string ChatID { get; set; }
  }
}
