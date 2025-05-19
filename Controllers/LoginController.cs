using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class LoginController : ControllerBase
  {
    private readonly Context _context;

    public LoginController(Context context){
      _context = context;
    }

    [HttpGet("GetAllUsers")]
    public IActionResult GetAllUsers(){
      var users =  _context.Users
        .Include(u => u.SentMessages)
        .Include(u => u.Notifications)
        .Include(u => u.UsersInChats).ThenInclude(uc => uc.Chat)
        .Include(u => u.Contacts).ThenInclude(c => c.ContactUser)
        .Include(u => u.ContactedBy)
        .ToList();
      string res = JsonConvert.SerializeObject(users, Formatting.Indented,
          new JsonSerializerSettings()
          {
          ReferenceLoopHandling = ReferenceLoopHandling.Ignore
          });
      return Ok(new {status = "OK", data = res});
    }
  }
}
