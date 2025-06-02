using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TestDatabase.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ChatController : ControllerBase
  {
    private readonly Context _context;
    public ChatController(Context context)
    {
      _context = context;
    }

    [HttpPost("new-chat")]
    public async Task<IActionResult> CreateNewChat([FromBody] ChatInfoType1 chatInfoType1){
      if(string.IsNullOrWhiteSpace(chatInfoType1.SessionToken) ||
          string.IsNullOrWhiteSpace(chatInfoType1.ChatName)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = FindUser(chatInfoType1.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }

      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});//Forbid();
      }

      var existingChat = FindChat(chatInfoType1.ChatName);
      if (existingChat != null){
        return Conflict(new { status = "error", error = "chat with this name already exists"});
      }
      Chat chat = new Chat{
        ChatName = chatInfoType1.ChatName,
      };
      _context.Chat.Add(chat);
      _context.SaveChanges();
      chat = FindChat(chatInfoType1.ChatName);
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
        string.IsNullOrWhiteSpace(chatInfoType2.ChatName) ||
        string.IsNullOrWhiteSpace(chatInfoType2.Username)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = FindUser(chatInfoType2.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});//Forbid();
      }
      Chat chat = FindChat(chatInfoType2.ChatName);
      var userExistInChat = await _context.UsersInChat.FirstOrDefaultAsync(uc => uc.ChatID == chat.ChatID &&
                                                                           uc.UserID == user.UserID);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
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
      User user = FindUser(chatInfoType3.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      if(user.IsAccountDeleted){
        return StatusCode(403, new {status = "error", error = "The account has been deleted, you can not further alter or use it."});//Forbid();
      }
      var usersChats = _context.UsersInChat.Include(us => us.Chat)
        .Where(us => us.UserID == user.UserID)
        .Select(uc => new {
          ChatId = uc.Chat.ChatID,
          ChatName = uc.Chat.ChatName
        });
      return Ok( new {status = "success", data = new { chats = usersChats }});
    }
    public class ChatInfoType1{
      public string SessionToken {get; set;}
      public string ChatName {get; set;}
    }
    public class ChatInfoType2{
      public string SessionToken {get; set;}
      public string ChatName {get; set;}
      public string Username {get; set;}
    }

    public class ChatInfoType3{
      public string SessionToken {get; set;}
    }
    public User FindUser(string token){
      return _context.Users.FirstOrDefault(u => u.SessionToken == token);
    }
    public Chat FindChat(string chatName){
      return _context.Chat.FirstOrDefault(c => c.ChatName == chatName);
    }
  }
}
