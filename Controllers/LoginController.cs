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
        [HttpPost("login")]
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
        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody] UserTokenInfo userTokenInfo)
        {
            if (string.IsNullOrWhiteSpace(userTokenInfo.SessionToken))
            {
                return BadRequest(new { status = "error", error = "empty data" });
            }

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.SessionToken == userTokenInfo.SessionToken);

            if (user == null)
            {
                return Unauthorized(new { status = "error", error = "user not found" });
            }

            if (user.SessionTokenExpirationDate < DateTime.Now){
                return Unauthorized(new { status = "error", error = "expired session token" });   
            }
            return Ok(new { status = "success", data = new { user } });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] NewUserInfo newUserInfo){
            if (string.IsNullOrWhiteSpace(newUserInfo.Username) ||
                string.IsNullOrWhiteSpace(newUserInfo.Password) || 
                string.IsNullOrWhiteSpace(newUserInfo.Email))
            {
                return BadRequest(new { status = "error", 
                                  error = "Username, password, and email must not be empty." });
            }
            if(await context.Users.
                FirstOrDefaultAsync(u => u.Username == newUserInfo.Username) != null){
                return Conflict(new {status = "error", 
                                     error = "A user with this username already exists."});
            }
            Random random = new Random();
            User user = new User(){
                Username = newUserInfo.Username,
                Password = newUserInfo.Password,
                Email = newUserInfo.Email,
                IsOnline = false,
                SessionToken = newUserInfo.Username+random.Next(1000, 9999),
                SessionTokenExpirationDate = DateTime.Now.AddDays(30),
                UserProfilePicturePath = "./test"
                };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Created();
        }
        public class UserTokenInfo{
            public string SessionToken {get; set;}
        }
        public class UserLoginInfo{
            public string Username {get; set;}
            public string Password {get; set;}
        }
        public class NewUserInfo{
            public string Username {get; set;}
            public string Password {get;set;}
            public string Email {get; set;}
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