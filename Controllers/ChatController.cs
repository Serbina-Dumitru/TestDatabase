using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestDatabase.Functionality;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ChatController : ControllerBase
  {
    private readonly Context _context;
    private DbFunctionality _dbFunctionality;
    public ChatController(Context context)
    {
      _context = context;
      _dbFunctionality = new DbFunctionality(_context);
    }

    [HttpPost("new-chat")]
    public async Task<IActionResult> CreateNewChat([FromBody] ChatInfoType1 chatInfoType1){
      if(string.IsNullOrWhiteSpace(chatInfoType1.SessionToken) ||
          string.IsNullOrWhiteSpace(chatInfoType1.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = _dbFunctionality.FindUserByToken(chatInfoType1.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }

      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }

      Chat chat = new Chat{
        ChatName = chatInfoType1.ChatID,
      };
      _context.Chat.Add(chat);
      _context.SaveChanges();
      chat = FindChat(chatInfoType1.ChatID);
      UsersInChat usersInChat = new UsersInChat{
        ChatID = chat.ChatID,
        UserID = user.UserID
      };
      _context.Add(usersInChat);
      _context.SaveChanges();
      return Ok( new {status = "success", data = "chat created successfylly"});
    }

    [HttpPost("add-user-in-chat")]
    public async Task<IActionResult> AddUserInChat([FromBody] ChatInfoType2 chatInfoType2){
      if(string.IsNullOrWhiteSpace(chatInfoType2.SessionToken) ||
        string.IsNullOrWhiteSpace(chatInfoType2.ChatID) ||
        string.IsNullOrWhiteSpace(chatInfoType2.Username)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = _dbFunctionality.FindUserByToken(chatInfoType2.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }

      Chat chat = FindChat(chatInfoType2.ChatID);
      if(chat == null){
        return Unauthorized(new {status = "error", error = "chat not found"});
      }
      var userExistInChat = await _context.UsersInChat.FirstOrDefaultAsync(uc => uc.ChatID == chat.ChatID &&
                                                                           uc.UserID == user.UserID);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }
      if(chat.IsChatDeleted){
        return StatusCode(403, new {status = "error", error = "The chat has been deleted, you can not further alter or use it."});
      }

      User newUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == chatInfoType2.Username);
      if(newUser == null){
        return Unauthorized(new {status = "error", error = "new user not found"});
      }
      UsersInChat usersInChat = new UsersInChat{
        ChatID = chat.ChatID,
        UserID = newUser.UserID
      };
      _context.UsersInChat.Add(usersInChat);
      _context.SaveChanges();
      return Ok( new {status = "success", data = "user added successfylly"});
    }

    [HttpPost("all-users-chats")]
    public async Task<IActionResult> AllUsersChats([FromBody] ChatInfoType3 chatInfoType3){
      if(string.IsNullOrWhiteSpace(chatInfoType3.SessionToken)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user =  _dbFunctionality.FindUserByToken(chatInfoType3.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }
      var usersChats = _context.UsersInChat.Include(us => us.Chat)
        .Where(us => us.UserID == user.UserID && !us.Chat.IsChatDeleted)
        .Select(uc => new {
          ChatId = uc.Chat.ChatID,
          ChatName = uc.Chat.ChatName
        });
      return Ok( new {status = "success", data = new { chats = usersChats }});
    }

    [HttpDelete("delete-chat")]
    public async Task<IActionResult> DeleteChat([FromBody] ChatInfoType1 chatInfo){
      if(string.IsNullOrWhiteSpace(chatInfo.SessionToken)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user =  _dbFunctionality.FindUserByToken(chatInfo.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }
      Chat chat = FindChat(chatInfo.ChatID);
      if(chat == null){
        return Unauthorized(new {status = "error", error = "chat not found"});
      }
      if(chat.IsChatDeleted){
        return StatusCode(403, new {status = "error", error = "The chat has been deleted, you can not further alter or use it."});
      }

      chat.IsChatDeleted = true;
      int HowMuchIsWritten = await _context.SaveChangesAsync();
      if(HowMuchIsWritten == 0){
        return StatusCode(500,new{status="error",error="The server was unable to delete the chat."});
      }

      return Ok(new {status = "success",data = chat});
    }

    [HttpPut("update-chat")]
    public async Task<IActionResult> UpdateChat([FromBody] ChatInfoType4 chatInfo){
      if(string.IsNullOrWhiteSpace(chatInfo.SessionToken) ||
         string.IsNullOrWhiteSpace(chatInfo.ChatName) ||
         string.IsNullOrWhiteSpace(chatInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = _dbFunctionality.FindUserByToken(chatInfo.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});
      }
      Chat chat = FindChat(chatInfo.ChatID);
      if(chat == null){
        return Unauthorized(new {status = "error", error = "chat not found"});
      }
      if(chat.IsChatDeleted){
        return StatusCode(403, new {status = "error", error = "The chat has been deleted, you can not further alter or use it."});
      }

      chat.ChatName = chatInfo.ChatName;
      int HowMuchIsWritten = await _context.SaveChangesAsync();
      if(HowMuchIsWritten == 0){
        return StatusCode(500,new{status="error",error="The server was unable to change chat name."});
      }

      return Ok(new {status = "success",data = chat});
    }

    public class ChatInfoType1{
      public string SessionToken {get; set;}
      public string ChatID {get; set;}
    }
    public class ChatInfoType2{
      public string SessionToken {get; set;}
      public string ChatID {get; set;}
      public string Username {get; set;}
    }

    public class ChatInfoType3{
      public string SessionToken {get; set;}
    }

    public class ChatInfoType4{
      public string SessionToken {get; set;}
      public string ChatID {get; set;}
      public string ChatName {get; set;}
    }
    public Chat FindChat(string ChatID){
      return _context.Chat.FirstOrDefault(c => c.ChatID == int.Parse(ChatID));
    }
  }
}
