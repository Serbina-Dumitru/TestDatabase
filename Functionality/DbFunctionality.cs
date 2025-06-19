using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using TestDatabase;
using TestDatabase.Dtos;
using TestDatabase.Dtos.ResponseDtos;

namespace TestDatabase.Functionality
{
  class DbFunctionality
  {
    public Random random;
    private readonly Context _context;
    public DbFunctionality(Context context)
    {
      _context = context;
      random = new Random();
    }

    //User
    public User FindUserByToken(string token){
        return _context.Users.FirstOrDefault(u => u.SessionToken == token);
    }
    public User FindUserByUsernameAndPassword(string username, string password){
      return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }
    public User FindUserByUsername(string username){
      return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    public User CreateUser(string username, string password){
      User user = new User(){
          Username = username,
          Password = password,
          IsOnline = false,
          SessionToken = username+random.Next(100000, 999999),
          SessionTokenExpirationDate = DateTime.Now.AddDays(30),
          UserProfilePicturePath = "./test"
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User CreateNewSessionToken(User user){
      user.SessionToken = user.Username+random.Next(1000, 9999);
      user.SessionTokenExpirationDate = DateTime.Now.AddDays(30);
      _context.Users.Update(user);
      _context.SaveChanges();
      return user;
    }

    public User DeleteUser(User user){
      user.IsAccountDeleted = true;
      user.Username = $"Deleted-Account-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
      _context.Users.Update(user);
      _context.SaveChanges();
      return user;
    }

    public User ChangeUsername(User user, string newUserName){
      user.Username = newUserName;
      _context.Users.Update(user);
      _context.SaveChanges();
      return user;
    }
    public User ChangeUserPassword(User user, string newPassword){
      user.Password = newPassword;
      _context.Users.Update(user);
      _context.SaveChanges();
      return user;
    }

    public UserDto ConvertUserToUserDto(User user)
    {
      return new UserDto()
      {
        UserID = user.UserID,
        Username = user.Username,
        SessionToken = user.SessionToken,
        UserProfilePicturePath = user.UserProfilePicturePath
      };
    }

    public void UpdateUserOnlineStatus(User user)
    {
      user.LastTimeOnline = DateTime.Now;
      user.IsOnline = true;
      _context.Users.Update(user);
      _context.SaveChanges();
    }

    //Message
    public List<MessageToClientDto> FindLastNMessagesInChat(string ChatID)
    {
      return _context.Messages
        .Where(m => m.ChatID == int.Parse(ChatID) && !m.IsDeleted)
        .OrderByDescending(m => m.TimeStamp)
        .Take(50)
        .OrderBy(m => m.TimeStamp)
         .Select(m => new MessageToClientDto
         {
           MessageID = m.MessageID,
           Content = m.Content,
           TimeStamp = m.TimeStamp,
           Sender = new SenderDto
           {
             UserID = m.Sender.UserID,
             Username = m.Sender.Username,
             UserProfilePicturePath = m.Sender.UserProfilePicturePath
           }
         })
        .ToList();
    }
    public List<MessageToClientDto> FindNewMessagesInChat(User user)
    {
      return _context.Messages
        .Where(m => m.TimeStamp > DateTime.Now.AddSeconds(-2) && m.Sender.UserID != user.UserID && !m.IsDeleted)
        .Select(m => new MessageToClientDto
        {
          MessageID = m.MessageID,
          Content = m.Content,
          TimeStamp = m.TimeStamp,
          Sender = new SenderDto
          {
            UserID = m.Sender.UserID,
            Username = m.Sender.Username,
            UserProfilePicturePath = m.Sender.UserProfilePicturePath
          }
        })
        .ToList();
    }

    public Message FindMessageById(string messageID)
    {
      return _context.Messages.FirstOrDefault(m => m.MessageID == int.Parse(messageID));
    }

    public Message CreateMessage(int UserID, string Content, string ChatID)
    {
      Message message = new Message
      {
        UserID = UserID,
        Content = Content,
        TimeStamp = DateTime.Now,
        ChatID = int.Parse(ChatID)
      };
      _context.Messages.AddAsync(message);
      _context.SaveChanges();
      return message;
    }

    public void DeleteMessage(string messageID)
    {
      Message message = FindMessageById(messageID);
      message.IsDeleted = true;
      _context.Messages.Update(message);
      _context.SaveChanges();
    }

    public void EditMessage(string messageId, string content)
    {
      Message message = FindMessageById(messageId);
      message.Content = content;
      message.IsModified = true;
      _context.Messages.Update(message);
      _context.SaveChanges();
    }

    public List<int> FindDeletedMessages(string chatId)
    {
      return _context.Messages.Where(m => m.ChatID == int.Parse(chatId) && m.IsDeleted).Select(m => m.MessageID).ToList();
    }

    public List<MessageToClientDto> FindEditedMessages(string chatId)
    {
      return _context.Messages.Where(m => m.ChatID == int.Parse(chatId) && m.IsModified)
        .Select(m => new MessageToClientDto
        {
          MessageID = m.MessageID,
          Content = m.Content,
          TimeStamp = m.TimeStamp,
          Sender = new SenderDto
          {
            UserID = m.Sender.UserID,
            Username = m.Sender.Username,
            UserProfilePicturePath = m.Sender.UserProfilePicturePath
          }
        })
          .ToList();
    }

    public MessageToClientDto ConvertMessageToMessageToClientDto(Message message)
    {
      return new MessageToClientDto()
      {
        MessageID = message.MessageID,
        Content = message.Content,
        TimeStamp = message.TimeStamp,
        Sender = ConvertToSenderDto(message.Sender)
      };
    }


    public SenderDto ConvertToSenderDto(User sender)
    {
      return new SenderDto()
      {
        UserID = sender.UserID,
        Username = sender.Username,
        UserProfilePicturePath = sender.UserProfilePicturePath
      };
    }

    //Chat
    public Chat FindChatById(string id){
      return _context.Chat.FirstOrDefault(c => c.ChatID == int.Parse(id));
    }
    public UsersInChat FindUserInChat(Chat chat, User user){
      return _context.UsersInChat.FirstOrDefault(uc => uc.ChatID == chat.ChatID && uc.UserID == user.UserID);
    }
    public List<ChatDto> FindAllChatsWithThisUser(User user){
      return _context.UsersInChat.Include(us => us.Chat).Include(c => c.Chat.Messages)
        .Where(us => us.UserID == user.UserID && !us.Chat.IsChatDeleted)
        .Select(c => new ChatDto
        {
          ChatID = c.ChatID,
          ChatName = c.Chat.ChatName,
          LastMessage = c.Chat.Messages
          .OrderByDescending(m => m.TimeStamp)
            .Select(m => new MessageToClientDto
            {
              Content = m.Content,
              TimeStamp = m.TimeStamp,
              Sender = new SenderDto
              {
                UserID = m.Sender.UserID,
                Username = m.Sender.Username,
                UserProfilePicturePath = m.Sender.UserProfilePicturePath
              }
            })
          .FirstOrDefault()
        })
        .ToList();
    }

    public List<ChatDto> FindAllChatsWithOfflineMessages(User user)
    {
      return _context.Chat.Include(c => c.ChatMembers).Include(c => c.Messages)
        .Where(c =>
          c.ChatMembers.Any(cm => cm.UserID == user.UserID) &&
          c.Messages.Any(m => m.TimeStamp > user.LastTimeOnline))
        .Select(c => new ChatDto
        {
          ChatID = c.ChatID,
          ChatName = c.ChatName,
          LastMessage = c.Messages
            .OrderByDescending(m => m.TimeStamp)
            .Select(m => new MessageToClientDto
            {
              Content = m.Content,
              TimeStamp = m.TimeStamp,
              Sender = new SenderDto
              {
                UserID = m.Sender.UserID,
                Username = m.Sender.Username,
                UserProfilePicturePath = m.Sender.UserProfilePicturePath
              }
            })
          .FirstOrDefault()
        })
        .ToList();
    }

    public Chat CreateChat(string chatName){
      Chat chat = new Chat{
        ChatName = chatName,
      };
      _context.Chat.Add(chat);
      _context.SaveChanges();

      return chat;
    }
    public ChatDto ChatToDto(Chat chat)
    {
      return _context.Chat.Include(c => c.ChatMembers).Include(c => c.Messages)
        .Where(c => c.ChatID == chat.ChatID)
        .Select(c => new ChatDto
        {
          ChatID = c.ChatID,
          ChatName = c.ChatName,
          LastMessage = c.Messages
            .OrderByDescending(m => m.TimeStamp)
            .Select(m => new MessageToClientDto
            {
              Content = m.Content,
              TimeStamp = m.TimeStamp,
              Sender = new SenderDto
              {
                UserID = m.Sender.UserID,
                Username = m.Sender.Username,
                UserProfilePicturePath = m.Sender.UserProfilePicturePath
              }
            })
          .FirstOrDefault()
        }).FirstOrDefault();
    }
    public UsersInChat AddUserToChat(Chat chat, User user){
      UsersInChat usersInChat = new UsersInChat{
        ChatID = chat.ChatID,
        UserID = user.UserID
      };
      _context.UsersInChat.Add(usersInChat);
      _context.SaveChanges();
      return usersInChat;
    }

    public Chat DeleteChat(Chat chat){
      chat.IsChatDeleted = true;
      _context.Chat.Update(chat);
      _context.SaveChanges();
      return chat;
    }

    public Chat UpdateChatName(Chat chat, string newChatName){
      chat.ChatName = newChatName;
      _context.Chat.Update(chat);
      _context.SaveChanges();
      return chat;
    }
  }
}
