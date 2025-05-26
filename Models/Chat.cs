using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TestDatabase;

public class Chat
{
  [Key]
  public int ChatID { get; set; }

  [Required]
  [StringLength(255)]
  public string ChatName { get; set; }

  public ICollection<Message> Messages { get; set; }
  public ICollection<UsersInChat> ChatMembers { get; set; }

}
