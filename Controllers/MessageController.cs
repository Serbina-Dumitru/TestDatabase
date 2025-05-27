using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace TestDatabase.Controllers{
  [ApiController]
  [Route("[controller]")]
  public class MessageController : ControllerBase{
    private readonly Context _context;
    public MessageController(Context context){
        _context = context;
    }

    [HttpPost("save")]
    public async Task<IActionResult> ResieveMessageFromUser([FromBody] MessageInfo messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(messageInfo.Content) ||
          string.IsNullOrWhiteSpace(messageInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = await _context.Users.FirstOrDefaultAsync(u => u.SessionToken == messageInfo.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      Message message = new Message{
        UserID = user.UserID,
        Content = messageInfo.Content,
        TimeStamp = DateTime.Now,
        ChatID = int.Parse(messageInfo.ChatID),
      };
      _context.Messages.AddAsync(message);
      _context.SaveChanges();
      return Created();
    }
    [HttpPost("get-chat-messages")]
    public async Task<IActionResult> SendToUserMessagesFromChat([FromBody] MessageType2 messageType2){
      if(string.IsNullOrWhiteSpace(messageType2.SessionToken) ||
          string.IsNullOrWhiteSpace(messageType2.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = await _context.Users.FirstOrDefaultAsync(u => u.SessionToken == messageType2.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      var userExistInChat = await _context.UsersInChat.FirstOrDefaultAsync(c => c.ChatID == int.Parse(messageType2.ChatID) &&
                                                                           c.UserID == user.UserID);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }
      user.LastTimeOnline = DateTime.Now;
      user.IsOnline = true;
      _context.Users.Update(user);
      _context.SaveChanges();
      var messages = _context.Messages
        .Where(m => m.ChatID == int.Parse(messageType2.ChatID))
        .OrderByDescending(m => m.TimeStamp)
        .Take(50)
        .OrderBy(m => m.TimeStamp)
        .Select(m => new {
            MessageID = m.MessageID,
            UserID = m.UserID,
            Content = m.Content,
            TimeStamp = m.TimeStamp,
            ChatID = m.ChatID,
            IsSeen = m.IsSeen,
            IsFile = m.IsFile,
            Sender = new {
                UserID = m.Sender.UserID,
                Username = m.Sender.Username,
                UserProfilePicturePath = m.Sender.UserProfilePicturePath
            }
        })
        .ToList();
      return Ok(new {status = "success", data = new {messages = messages}});
    }
    [HttpPost("get-new-chat-messages")]
    public async Task<IActionResult> SendToUserNewMessagesFromChat([FromBody] MessageType3 messageType3){
      if(string.IsNullOrWhiteSpace(messageType3.SessionToken) ||
          string.IsNullOrWhiteSpace(messageType3.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = await _context.Users.FirstOrDefaultAsync(u => u.SessionToken == messageType3.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      var userExistInChat = await _context.UsersInChat.FirstOrDefaultAsync(c => c.ChatID == int.Parse(messageType3.ChatID) &&
                                                                           c.UserID == user.UserID);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }
      var messages = _context.Messages
        .Where(m => m.TimeStamp > DateTime.Now.AddSeconds(-0.5) && m.Sender.UserID != user.UserID)
        .Select(m => new {
          MessageID = m.MessageID,
          UserID = m.UserID,
          Content = m.Content,
          TimeStamp = m.TimeStamp,
          ChatID = m.ChatID,
          IsSeen = m.IsSeen,
          IsFile = m.IsFile,
          Sender = new {
              UserID = m.Sender.UserID,
              Username = m.Sender.Username,
              UserProfilePicturePath = m.Sender.UserProfilePicturePath
          }
        })
        .ToList();
        System.Console.WriteLine(messages);
      return Ok(new {status = "success", data = new {messages = messages}});
    }
    [HttpPost("get-new-messages")]
    public async Task<IActionResult> SendToUserNewMessages([FromBody] MessageType4 messageType4){
      if(string.IsNullOrWhiteSpace(messageType4.SessionToken)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = await _context.Users.FirstOrDefaultAsync(u => u.SessionToken == messageType4.SessionToken);
      if(user == null){
        return Unauthorized(new {status = "error", error = "user not found"});
      }
      var chats = _context.Chat.Include(c => c.ChatMembers).Include(c => c.Messages)
        .Where(c =>
          c.ChatMembers.Any(cm => cm.UserID == user.UserID) &&
          c.Messages.Any(m => m.TimeStamp > user.LastTimeOnline))
        .Select(c => new {
          ChatId = c.ChatID,
          ChatName = c.ChatName,
          LastMessage = c.Messages
            .OrderByDescending(m => m.TimeStamp)
            .Select(m => new {
              UserID = m.UserID,
              Content = m.Content,
              TimeStamp = m.TimeStamp,
              ChatID = m.ChatID,
              IsSeen = m.IsSeen,
              IsFile = m.IsFile,
              Sender = new {
                  UserID = m.Sender.UserID,
                  Username = m.Sender.Username,
                  UserProfilePicturePath = m.Sender.UserProfilePicturePath
          }
        })
          .FirstOrDefault()
        })
        .ToList();
      return Ok(new {status = "success", data = new {chats = chats}});
    }
    public class MessageInfo{
      public string SessionToken {get; set;}
      public string Content {get; set;}
      public string ChatID {get; set;}
    }
    public class MessageType2{
      public string SessionToken {get; set;}
      public string ChatID {get;set;}
    }
    public class MessageType3{
      public string SessionToken {get; set;}
      public string ChatID {get;set;}
    }
    public class MessageType4{
      public string SessionToken {get; set;}
    }
  }
}
