namespace TestDatabase.Dtos.RequestDtos
{
  public class GetDeletedMessagesRequestDto
  {
    public string SessionToken { get; set; }
    public string ChatID { get; set; }
  }
}
