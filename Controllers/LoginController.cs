using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TestDatabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        public Context context = new Context();
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginInfo userLoginInfo)
        {
            if (string.IsNullOrWhiteSpace(userLoginInfo.Username) || string.IsNullOrWhiteSpace(userLoginInfo.Password))
            {
                return BadRequest(new { status = "error", error = "empty data" });
            }

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Username == userLoginInfo.Username && u.Password == userLoginInfo.Password);

            if (user == null)
            {
                return Unauthorized(new { status = "error", error = "user not found" });
            }

            return Ok(new { status = "success", data = new { user } });
        }
        public class UserLoginInfo{
            public string Username {get; set;}
            public string Password {get; set;}
        }
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