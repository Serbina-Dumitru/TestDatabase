using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestDatabase.Functionality;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AutentificationController : ControllerBase
  {
      private readonly Context _context;
      private DbFunctionality _dbFunctionality;
      public AutentificationController(Context context)
      {
          _context = context;
          _dbFunctionality  = new DbFunctionality(_context);
      }
      Random random = new Random();

      [HttpPost("login")]
      public async Task<IActionResult> Login([FromBody] UserLoginInfo userLoginInfo)
      {
          if (string.IsNullOrWhiteSpace(userLoginInfo.Username) || string.IsNullOrWhiteSpace(userLoginInfo.Password))
          {
            return BadRequest(new { status = "error", error = "empty data" });
          }

          var user = _dbFunctionality.FindUserByUsernameAndPassword(userLoginInfo.Username, userLoginInfo.Password);

          if (user == null)
          {
            return Unauthorized(new { status = "error", error = "user not found" });
          }
          if(user.IsAccountDeleted){
            return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});//Forbid();
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

        var user = _dbFunctionality.FindUserByToken(userTokenInfo.SessionToken);

        if (user == null)
        {
          return Unauthorized(new { status = "error", error = "user not found" });
        }
        if(user.IsAccountDeleted){
          return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});//Forbid();
        }

        if (user.SessionTokenExpirationDate < DateTime.Now){
          _dbFunctionality.CreateNewSessionToken(user);
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
        if(_dbFunctionality.FindUserByUsername(newUserInfo.Username) != null){
          return Conflict(new {status = "error",
              error = "A user with this username already exists."});
        }

        User user = _dbFunctionality.CreateUser(newUserInfo.Username, newUserInfo.Password, newUserInfo.Email);
        return Ok(new { status = "success", data = new { user } });
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
