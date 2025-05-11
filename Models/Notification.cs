using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestDatabase;

public class Notification
{
  [Key]
  public int NotificationID {get;set;}

  [Required]
  public int UserID {get;set;}

  public string Content {get;set;}

  [Required]
  public DateTime Timestamp {get;set;}

  [ForeignKey("UserID")]
  public User User {get;set;}
}
