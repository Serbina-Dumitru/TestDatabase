using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AutentificationController : ControllerBase
  {
      private readonly Context _context;
      public AutentificationController(Context context)
      {
          _context = context;
      }
      Random random = new Random();

      [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] UserLoginInfo userLoginInfo)
      {
          if (string.IsNullOrWhiteSpace(userLoginInfo.Username) || string.IsNullOrWhiteSpace(userLoginInfo.Password))
          {
            return BadRequest(new { status = "error", error = "empty data" });
          }

          var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == userLoginInfo.Username && u.Password == userLoginInfo.Password);

          if (user == null)
          {
            return Unauthorized(new { status = "error", error = "user not found" });
          }
          if(user.IsAccountDeleted){
            return Forbid();
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

          var user = await _context.Users
              .FirstOrDefaultAsync(u => u.SessionToken == userTokenInfo.SessionToken);

          if (user == null)
          {
              return Unauthorized(new { status = "error", error = "user not found" });
          }

          if(user.IsAccountDeleted){
            return Forbid();
          }

          if (user.SessionTokenExpirationDate < DateTime.Now){
              user.SessionToken = user.Username+random.Next(1000, 9999);
              user.SessionTokenExpirationDate = DateTime.Now.AddDays(30);
              _context.Users.Update(user);
              _context.SaveChanges();
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
          if(await _context.Users.
              FirstOrDefaultAsync(u => u.Username == newUserInfo.Username) != null){
              return Conflict(new {status = "error",
                                    error = "A user with this username already exists."});
          }
          User user = new User(){
              Username = newUserInfo.Username,
              Password = newUserInfo.Password,
              Email = newUserInfo.Email,
              IsOnline = false,
              SessionToken = newUserInfo.Username+random.Next(1000, 9999),
              SessionTokenExpirationDate = DateTime.Now.AddDays(30),
              UserProfilePicturePath = "./test"
              };
          await _context.Users.AddAsync(user);
          await _context.SaveChangesAsync();
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
  }
}
