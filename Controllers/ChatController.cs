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
    private VereficationFunctionality _verefication;
    public ChatController(Context context)
    {
      _context = context;
      _dbFunctionality = new DbFunctionality(_context);
      _verefication = new VereficationFunctionality();
    }

    [HttpPost("new-chat")]
    public async Task<IActionResult> CreateNewChat([FromBody] ChatInfoType1 chatInfoType1){
      if(string.IsNullOrWhiteSpace(chatInfoType1.SessionToken) ||
          string.IsNullOrWhiteSpace(chatInfoType1.ChatName)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(chatInfoType1.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.CreateChat(chatInfoType1.ChatName);
      UsersInChat usersInChat = _dbFunctionality.AddUserToChat(chat, user);
      return Ok( new {status = "success", data = chat});
    }

    [HttpPost("add-user-in-chat")]
    public async Task<IActionResult> AddUserInChat([FromBody] ChatInfoType2 chatInfoType2){
      if(string.IsNullOrWhiteSpace(chatInfoType2.SessionToken) ||
        string.IsNullOrWhiteSpace(chatInfoType2.ChatID) ||
        string.IsNullOrWhiteSpace(chatInfoType2.Username)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = _dbFunctionality.FindUserByToken(chatInfoType2.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.FindChatById(chatInfoType2.ChatID);
      if(chat == null){
        return Unauthorized(new {status = "error", error = "chat not found"});
      }
      if(chat.IsChatDeleted){
        return StatusCode(403, new {status = "error", error = "The chat has been deleted, you can not further alter or use it."});
      }

      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }

      user = _dbFunctionality.FindUserByUsername(chatInfoType2.Username);
      verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      var newUserExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if(newUserExistInChat != null){
        return Conflict(new {status = "error", error = "user already exist iin this chat"});
      }

      UsersInChat usersInChat = _dbFunctionality.AddUserToChat(chat, user);
      return Ok( new {status = "success", data = "user added successfully"});
    }

    [HttpPost("all-users-chats")]
    public async Task<IActionResult> AllUsersChats([FromBody] ChatInfoType3 chatInfoType3){
      if(string.IsNullOrWhiteSpace(chatInfoType3.SessionToken)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user =  _dbFunctionality.FindUserByToken(chatInfoType3.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      var usersChats = _dbFunctionality.FindAllChatsWithThisUser(user);
      return Ok(new { status = "success", data = usersChats });
    }

    [HttpDelete("delete-chat")]
    public async Task<IActionResult> DeleteChat([FromBody] ChatInfoType5 chatInfo){
      if(string.IsNullOrWhiteSpace(chatInfo.SessionToken) ||
         string.IsNullOrWhiteSpace(chatInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user =  _dbFunctionality.FindUserByToken(chatInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.FindChatById(chatInfo.ChatID);
      if(chat == null){
        return Unauthorized(new {status = "error", error = "chat not found"});
      }
      if(chat.IsChatDeleted){
        return StatusCode(403, new {status = "error", error = "The chat has been deleted, you can not further alter or use it."});
      }
      
      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }

      chat = _dbFunctionality.DeleteChat(chat);
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
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;
        
      Chat chat = _dbFunctionality.FindChatById(chatInfo.ChatID);
      if(chat == null){
        return Unauthorized(new {status = "error", error = "chat not found"});
      }
      if(chat.IsChatDeleted){
        return StatusCode(403, new {status = "error", error = "The chat has been deleted, you can not further alter or use it."});
      }

      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }

      chat = _dbFunctionality.UpdateChatName(chat, chatInfo.ChatName);
      return Ok(new {status = "success",data = chat});
    }

    public class ChatInfoType1{
      public string SessionToken {get; set;}
      public string ChatName {get; set;}
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
        public class ChatInfoType5{
      public string SessionToken {get; set;}
      public string ChatID {get; set;}
    }
  }
}
