namespace TestDatabase.Dtos.RequestDtos
{
  public class UserChangeEmailRequestDto
  {
    public string SessionToken { get; set; }
    public string NewEmail { get; set; }
  }
}
