using Microsoft.EntityFrameworkCore;
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

    public User CreateUser(string username, string password, string email){
      User user = new User(){
          Username = username,
          Password = password,
          Email = email,
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
    public User ChangeUserEmail(User user, string newEmail){
      user.Email = newEmail;
      _context.Users.Update(user);
      _context.SaveChanges();
      return user;
    }


    //Message


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

    public Chat CreateChat(string chatName){
      Chat chat = new Chat{
        ChatName = chatName,
      };
      _context.Chat.Add(chat);
      _context.SaveChanges();
      return chat;
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
