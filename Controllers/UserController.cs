using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly Context _context;
    public UserController(Context context)
    {
      _context = context;
    }

    [HttpPost("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] UserTokenInfo userToken){
      if (string.IsNullOrWhiteSpace(userToken.SessionToken))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      var user = await _context.Users
        .FirstOrDefaultAsync(u => u.SessionToken == userToken.SessionToken);

      if (user == null)
      {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return Forbid();
      }

      user.IsAccountDeleted = true;
      int HowMuchIsWritten = await _context.SaveChangesAsync();
      if(HowMuchIsWritten == 0){
        return StatusCode(500,new{status="error",error="The server was unable to delete your account."});
      }
      user.Username = $"Deleted-Account-{DateTime.Now.Second}";
      await _context.SaveChangesAsync();

      return Ok(new {status = "success",data = new {user = user}});
    }

    [HttpPost("change-user-name")]
    public async Task<IActionResult> ChangeUserName([FromBody] UserTokenAndUserName userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewUserName)   )
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User? user = await _context.Users
        .FirstOrDefaultAsync(u => u.SessionToken == userInfo.SessionToken);

      if (user == null)
      {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return Forbid();
      }

      User? existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Username == userInfo.NewUserName);
      if(existingUser != null){
        return Conflict(new{status = "error",error="User with that username already exists."});
      }

      user.Username = userInfo.NewUserName;
      int HowMuchIsWritten = await _context.SaveChangesAsync();
      if(HowMuchIsWritten == 0){
        return StatusCode(500,new{status="error",error="The server was unable to change your user name."});
      }

      return Ok(new {status = "success",data = new {user = user}});
    }

    [HttpPost("change-user-password")]
    public async Task<IActionResult> ChangeUserPassword([FromBody] UserTokenAndUserPassword userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewPassword))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User? user = await _context.Users
        .FirstOrDefaultAsync(u => u.SessionToken == userInfo.SessionToken);

      if (user == null) {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return Forbid();
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
      //if(!userInfo.NewPassword.Any(c => char.IsSymbol(c))){
      //  return BadRequest(new {status ="error", error = "The password should contain at least one symbol."});
      //}


      user.Password = userInfo.NewPassword;
      int HowMuchIsWritten = await _context.SaveChangesAsync();
      if(HowMuchIsWritten == 0){
        return StatusCode(500,new{status="error",error="The server was unable to change your password."});
      }

      return Ok(new {status = "success",data = new {user = user}});

    }
    [HttpPost("change-user-email")]
    public async Task<IActionResult> ChangeUserEmail([FromBody] UserTokenAndUserEmail userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewEmail))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User? user = await _context.Users
        .FirstOrDefaultAsync(u => u.SessionToken == userInfo.SessionToken);

      if (user == null) {
        return Unauthorized(new { status = "error", error = "user not found" });
      }

      if(user.IsAccountDeleted){
        return Forbid();
      }

      if(!new EmailAddressAttribute().IsValid(userInfo.NewEmail)){
        return BadRequest(new {status = "error", error = "The provided email is invalid."});
      }

      user.Email = userInfo.NewEmail;
      int HowMuchIsWritten = await _context.SaveChangesAsync();
      if(HowMuchIsWritten == 0){
        return StatusCode(500,new{status="error",error="The server was unable to change your email."});
      }

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
