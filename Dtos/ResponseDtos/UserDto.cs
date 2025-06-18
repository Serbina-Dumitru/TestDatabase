namespace TestDatabase.Dtos.ResponseDtos
{
  public class UserDto
  {
    public int UserID { get; set; }
    public string Username { get; set; }
    public string SessionToken { get; set; }
    public string UserProfilePicturePath { get; set; }
  }
}
