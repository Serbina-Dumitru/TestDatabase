namespace TestDatabase.Dtos.RequestDtos
{
  public class MessageUpdateRequestDto
  {
    public string SessionToken { get; set; }
    public string MessageID { get; set; }
    public string Content { get; set; }
  }
}
