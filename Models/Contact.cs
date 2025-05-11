using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestDatabase;

public class Contact
{
  [Key]
  public int ContactID {get;set;}

  [Required]
  public int UserID {get;set;}

  [Required]
  public int ContactUserID {get;set;}

  [ForeignKey("UserID")]
  public User User {get;set;}

  [ForeignKey("ContactUserID")] 
  public User ContactUser {get;set;}
}
