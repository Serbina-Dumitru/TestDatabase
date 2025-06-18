using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestDatabase.Dtos.RequestDtos;
using TestDatabase.Dtos.ResponseDtos;
using TestDatabase.Functionality;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AutentificationController : ControllerBase
  {
    private readonly Context _context;
    private DbFunctionality _dbFunctionality;
    private VereficationFunctionality _verefication;
    public AutentificationController(Context context)
    {
      _context = context;
      _dbFunctionality  = new DbFunctionality(_context);
      _verefication = new VereficationFunctionality();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userInfo)
    {
      if (string.IsNullOrWhiteSpace(userInfo.Username) || string.IsNullOrWhiteSpace(userInfo.Password))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }

      var user = _dbFunctionality.FindUserByUsernameAndPassword(userInfo.Username, userInfo.Password);

      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromBody] UserTokenLoginRequestDto userInfo)
    {
      if (string.IsNullOrWhiteSpace(userInfo.SessionToken))
      {
        return BadRequest(new { status = "error", error = "empty data" });
      }

      var user = _dbFunctionality.FindUserByToken(userInfo.SessionToken);

      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      if (user.SessionTokenExpirationDate < DateTime.Now){
        _dbFunctionality.CreateNewSessionToken(user);
      }
      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userInfo){
      if (string.IsNullOrWhiteSpace(userInfo.Username) ||
          string.IsNullOrWhiteSpace(userInfo.Password))
      {
        return BadRequest(new { status = "error",
            error = "Username, password, and email must not be empty." });
      }
      if(_dbFunctionality.FindUserByUsername(userInfo.Username) != null){
        return Conflict(new {status = "error",
            error = "A user with this username already exists."});
      }

      User user = _dbFunctionality.CreateUser(userInfo.Username, userInfo.Password);
      UserDto userDto = _dbFunctionality.ConvertUserToUserDto(user);
      return Ok(new { status = "success", data = new { user = userDto } });
    }
  }
}
