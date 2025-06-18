namespace TestDatabase.Dtos.RequestDtos
{
  public class AddUserToChatRequestDto
  {
    public string SessionToken { get; set; }
    public string ChatID { get; set; }
    public string Username { get; set; }
  }
}
