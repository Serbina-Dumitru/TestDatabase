using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestDatabase;

public class Message
{
  [Key]
  public int MessageID { get; set; }

  [Required]
  public int UserID { get; set; }

  public string Content { get; set; }

  [Required]
  public DateTime TimeStamp { get; set; }

  [Required]
  public int ChatID { get; set; }

  [Required]
  public bool IsSeen { get; set; }

  [Required]
  public bool IsFile { get; set; }

  [Required]
  [ForeignKey("UserID")]
  public User Sender { get; set; }

  [Required]
  [ForeignKey("ChatID")]
  public Chat Chat { get; set; }
}
