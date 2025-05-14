using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TestDatabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
    //     app.MapGet("/GetAllUsers", () => {
    //   using(var _context = new Context()){
    //     var users =  _context.Users
    //       .Include(u => u.SentMessages)
    //       .Include(u => u.Notifications)
    //       .Include(u => u.UsersInChats).ThenInclude(uc => uc.Chat)
    //       .Include(u => u.Contacts).ThenInclude(c => c.ContactUser)
    //       .Include(u => u.ContactedBy)
    //       .ToList();
    //     //var options = new JsonSerializerOptions{ ReferenceHandler = ReferenceHandler.IgnoreCycles};
    //     //return Results.Json(users,options);
    //     return JsonConvert.SerializeObject(users, Formatting.Indented,
    //         new JsonSerializerSettings()
    //         { 
    //         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    //         });
    //     }
    //     })
    // .WithName("GetAllUsers");
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers(){
                using(var _context = new Context()){
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
}