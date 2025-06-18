using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestDatabase.Dtos.RequestDtos;
using TestDatabase.Dtos.ResponseDtos;
using TestDatabase.Functionality;

namespace TestDatabase.Controllers{
  [ApiController]
  [Route("[controller]")]
  public class MessageController : ControllerBase{
    private readonly Context _context;
    private DbFunctionality _dbFunctionality;
    private VereficationFunctionality _verefication;
    public MessageController(Context context){
        _context = context;
        _dbFunctionality = new DbFunctionality(_context);
        _verefication = new VereficationFunctionality();
    }

    [HttpPost("save")]
    public async Task<IActionResult> ResieveMessageFromUser([FromBody] MessageSaveRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(messageInfo.Content) ||
          string.IsNullOrWhiteSpace(messageInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Message message = _dbFunctionality.CreateMessage(user.UserID, messageInfo.Content, messageInfo.ChatID);
      MessageToClientDto messageToClientDto = _dbFunctionality.ConvertMessageToMessageToClientDto(message);
      return Ok(new { status = "success", data = new { message = messageToClientDto } });
    }

    [HttpPost("get-chat-messages")]
    public async Task<IActionResult> SendToUserMessagesFromChat([FromBody] GetMessagesInChatRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(messageInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.FindChatById(messageInfo.ChatID);
      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if(userExistInChat == null){
        return Unauthorized(new {status = "error", error = "user dosen't have access to this chat"});
      }

      _dbFunctionality.UpdateUserOnlineStatus(user);

      List<MessageToClientDto> messages = _dbFunctionality.FindLastNMessagesInChat(messageInfo.ChatID);
      return Ok(new {status = "success", data = new {messages = messages}});
    }

    [HttpPost("get-new-chat-messages")]
    public async Task<IActionResult> SendToUserNewMessagesFromChat([FromBody] GetNewMessagesInChatRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
          string.IsNullOrWhiteSpace(messageInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.FindChatById(messageInfo.ChatID);
      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if (userExistInChat == null){
        return Unauthorized(new { status = "error", error = "user dosen't have access to this chat" });
      }

      List<MessageToClientDto> messages = _dbFunctionality.FindNewMessagesInChat(user);
      return Ok(new {status = "success", data = new {messages = messages}});
    }

    [HttpPost("get-chats-with-offline-messages")]
    public async Task<IActionResult> SendToUserOfflineMessages([FromBody] GetChatsWithOfflineMessagesRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken)){
        return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      List<ChatDto> chats = _dbFunctionality.FindAllChatsWithOfflineMessages(user); 
      return Ok(new {status = "success", data = new {chats = chats}});
    }

    [HttpDelete("message-delete")]
    public async Task<IActionResult> DeleteMessage([FromBody] MessageDeleteRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
         string.IsNullOrWhiteSpace(messageInfo.MessageID)){
          return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      _dbFunctionality.DeleteMessage(messageInfo.MessageID);
      return Ok(new {status = "success", data="message deleted successfully"});
    }

    [HttpPut("message-update")]
    public async Task<IActionResult> UpdateMessage([FromBody] MessageUpdateRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
         string.IsNullOrWhiteSpace(messageInfo.MessageID) || 
         string.IsNullOrWhiteSpace(messageInfo.Content)){
          return BadRequest(new { status = "error", error = "empty data" });
      }

      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      _dbFunctionality.EditMessage(messageInfo.MessageID, messageInfo.Content);
      return Ok(new {status = "success", data="message updated successfully"});
    }

    [HttpPost("get-deleted-messages")]
    public async Task<IActionResult> SendToUserDeletedMessage([FromBody] GetDeletedMessagesRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
         string.IsNullOrWhiteSpace(messageInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.FindChatById(messageInfo.ChatID);
      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if (userExistInChat == null){
        return Unauthorized(new { status = "error", error = "user dosen't have access to this chat" });
      }

      List<int> deletedMessages = _dbFunctionality.FindDeletedMessages(messageInfo.ChatID);
      return Ok( new { status = "success", data = new { messages = deletedMessages } });
    }

    [HttpPost("get-updated-messages")]
    public async Task<IActionResult> SendToUserUpdatedMessages([FromBody] GetUpdatedMessagesRequestDto messageInfo){
      if(string.IsNullOrWhiteSpace(messageInfo.SessionToken) ||
         string.IsNullOrWhiteSpace(messageInfo.ChatID)){
        return BadRequest(new { status = "error", error = "empty data" });
      }
      User user = _dbFunctionality.FindUserByToken(messageInfo.SessionToken);
      var verificationResult = await _verefication.UserVerefication(user);
      if (verificationResult != null)
        return verificationResult;

      Chat chat = _dbFunctionality.FindChatById(messageInfo.ChatID);
      var userExistInChat = _dbFunctionality.FindUserInChat(chat, user);
      if (userExistInChat == null){
        return Unauthorized(new { status = "error", error = "user dosen't have access to this chat" });
      }

      List<MessageToClientDto> messages = _dbFunctionality.FindEditedMessages(messageInfo.ChatID);
      return Ok( new { status = "success", data = new { messages = messages } });
    }
  }
}
