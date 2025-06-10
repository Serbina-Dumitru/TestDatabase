using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestDatabase.Functionality;
using System.ComponentModel.DataAnnotations;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly Context _context;
    private DbFunctionality _dbFunctionality;
    public UserController(Context context)
    {
      _context = context;
      _dbFunctionality = new DbFunctionality(_context);
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] UserTokenInfo userToken){
      if (string.IsNullOrWhiteSpace(userToken.SessionToken))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }

      var user = _dbFunctionality.FindUserByToken(userToken.SessionToken);

      if (user == null)
      {
        return Unauthorized(new { status = "error", error = "user not found" });
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }

      user = _dbFunctionality.DeleteUser(user);
      return Ok(new {status = "success",data = new {user = user}});
    }

    [HttpPut("change-user-name")]
    public async Task<IActionResult> ChangeUserName([FromBody] UserTokenAndUserName userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewUserName)   )
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User? user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);

      if (user == null)
      {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }

      User? existingUser = _dbFunctionality.FindUserByUsername(userInfo.NewUserName);
      if(existingUser != null){
        return Conflict(new{status = "error",error="User with that username already exists."});
      }

      user = _dbFunctionality.ChangeUsername(user, userInfo.NewUserName);
      return Ok(new {status = "success",data = new {user = user}});
    }

    [HttpPut("change-user-password")]
    public async Task<IActionResult> ChangeUserPassword([FromBody] UserTokenAndUserPassword userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewPassword))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User? user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);

      if (user == null) {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }

      if(userInfo.NewPassword.Count() < 8){
        return BadRequest(new {status = "error", error = "The password should be at least 8 characters."});
      }
      if(!userInfo.NewPassword.Any(c => char.IsUpper(c))){
        return BadRequest(new {status = "error", error = "The password should contain at least one upercase letter."});
      }
      if(!userInfo.NewPassword.Any(c => char.IsDigit(c))){
        return BadRequest(new {status ="error", error = "The password should contain at least one digit."});
      }

      user = _dbFunctionality.ChangeUserPassword(user, userInfo.NewPassword);

      return Ok(new {status = "success",data = new {user = user}});

    }
    [HttpPut("change-user-email")]
    public async Task<IActionResult> ChangeUserEmail([FromBody] UserTokenAndUserEmail userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewEmail))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User? user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);

      if (user == null) {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }

      if(!new EmailAddressAttribute().IsValid(userInfo.NewEmail)){
        return BadRequest(new {status = "error", error = "The provided email is invalid."});
      }

      user = _dbFunctionality.ChangeUserEmail(user, userInfo.NewEmail);

      return Ok(new {status = "success",data = new {user = user}});
    }

    public class UserTokenInfo{
      public string SessionToken {get; set;}
    }
    public class UserTokenAndUserName {
      public string SessionToken {get; set;}
      public string NewUserName {get; set;}
    }
    public class UserTokenAndUserPassword {
      public string SessionToken {get; set;}
      public string NewPassword {get; set;}
    }
    public class UserTokenAndUserEmail {
      public string SessionToken {get; set;}
      public string NewEmail {get; set;}
    }
  }
}
