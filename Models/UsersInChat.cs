using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestDatabase;

public class UsersInChat
{
  [Key]
  public int UsersInChatID {get;set;}

  [Required]
  public int ChatID {get;set;}

  [Required]
  public int UserID {get;set;}

  [ForeignKey("ChatID")]
  public Chat Chat {get;set;}

  [ForeignKey("UserID")]
  public User User {get;set;}

}
