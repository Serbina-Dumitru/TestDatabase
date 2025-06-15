namespace TestDatabase.Dtos.RequestDtos
{
  public class UserChangeUsernameRequestDto
  {
    public string SessionToken { get; set; }
    public string NewUserName { get; set; }
  }
}
