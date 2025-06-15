using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TestDatabase.Dtos.RequestDtos;
using TestDatabase.Dtos.ResponseDtos;
using TestDatabase.Functionality;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    private readonly Context _context;
    private DbFunctionality _dbFunctionality;
    private VereficationFunctionality _verefication;
    public UserController(Context context)
    {
      _context = context;
      _dbFunctionality = new DbFunctionality(_context);
      _verefication = new VereficationFunctionality();
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] UserDeleteRequestDto userToken){
      if (string.IsNullOrWhiteSpace(userToken.SessionToken)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(userToken.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      user = _dbFunctionality.DeleteUser(user);
      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });
    }

    [HttpPut("change-user-name")]
    public async Task<IActionResult> ChangeUserName([FromBody] UserChangeUsernameRequestDto userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewUserName)   ){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      User? existingUser = _dbFunctionality.FindUserByUsername(userInfo.NewUserName);
      if(existingUser != null){
        return Conflict(new{status = "error",error="User with that username already exists."});
      }

      user = _dbFunctionality.ChangeUsername(user, userInfo.NewUserName);
      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });
    }

    [HttpPut("change-user-password")]
    public async Task<IActionResult> ChangeUserPassword([FromBody] UserChangePasswordRequestDto userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewPassword)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      if (userInfo.NewPassword.Count() < 8){
        return BadRequest(new {status = "error", error = "The password should be at least 8 characters."});
      }
      if(!userInfo.NewPassword.Any(c => char.IsUpper(c))){
        return BadRequest(new {status = "error", error = "The password should contain at least one upercase letter."});
      }
      if(!userInfo.NewPassword.Any(c => char.IsDigit(c))){
        return BadRequest(new {status ="error", error = "The password should contain at least one digit."});
      }

      user = _dbFunctionality.ChangeUserPassword(user, userInfo.NewPassword);

      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });

    }
    [HttpPut("change-user-email")]
    public async Task<IActionResult> ChangeUserEmail([FromBody] UserChangeEmailRequestDto userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(userInfo.NewEmail)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      if (!new EmailAddressAttribute().IsValid(userInfo.NewEmail)){
        return BadRequest(new {status = "error", error = "The provided email is invalid."});
      }

      user = _dbFunctionality.ChangeUserEmail(user, userInfo.NewEmail);

      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });
    }
  }
}
