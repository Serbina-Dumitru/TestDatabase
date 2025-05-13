using System.ComponentModel.DataAnnotations;

namespace TestDatabase;

public class User
{
  [Key]
  [Required]
  public int UserID {get;set;}

  [Required]
  [StringLength(100)]
  public string Username {get;set;}

  [Required]
  [StringLength(255)]
  public string Password {get;set;}

  [Required]
  [StringLength(255)]
  public string Email {get;set;}

  [Required]
  public bool IsOnline {get;set;}

  [Required]
  [StringLength(255)]
  public string SessionToken {get;set;}

  [Required]
  public DateTime SessionTokenExpirationDate {get;set;}

  [Required]
  [StringLength(255)]
  public string UserProfilePicturePath {get;set;}

  public ICollection<Message>       SentMessages  {get;set;}
  public ICollection<Notification>  Notifications {get;set;}
  public ICollection<UsersInChat>   UsersInChats  {get;set;}
  public ICollection<Contact>       Contacts      {get;set;}
  public ICollection<Contact>       ContactedBy   {get;set;}
}
