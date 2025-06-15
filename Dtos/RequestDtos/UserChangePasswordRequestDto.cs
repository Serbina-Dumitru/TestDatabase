namespace TestDatabase.Dtos.RequestDtos
{
  public class UserChangePasswordRequestDto
  {
    public string SessionToken { get; set; }
    public string NewPassword { get; set; }
  }
}
