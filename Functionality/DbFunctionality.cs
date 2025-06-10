using Microsoft.EntityFrameworkCore;
using TestDatabase;

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
  


    public static void QuerryAllDbNames()
    {
      using (var _context = new Context())
      {
        _context.Users.ToList();
        _context.Messages.ToList();
        _context.Chat.ToList();
        _context.UsersInChat.ToList();
        _context.Notifications.ToList();
        _context.Notifications.ToList();
        var allData = _context.ChangeTracker.Entries().Select(e => e.Entity).ToList();
        allData.ForEach(e => Console.WriteLine(e));
      }
    }
    public static void PrintAllTheDb()
    {
      Console.WriteLine();
      using (var _context = new Context())
      {
        Console.WriteLine("Users:");
        var users = _context.Users
          .Include(u => u.SentMessages)
          .Include(u => u.Notifications)
          .Include(u => u.UsersInChats).ThenInclude(uc => uc.Chat)
          .Include(u => u.Contacts).ThenInclude(c => c.ContactUser)
          .Include(u => u.ContactedBy)
          .ToList();
        foreach (var user in users)
        {
          Console.WriteLine($"User: {user.UserID}, {user.Username}, {user.Email}");
          Console.WriteLine($"  Sent Messages: {user.SentMessages?.Count}");
          Console.WriteLine($"  Notifications: {user.Notifications?.Count}");
          Console.WriteLine($"  Chats: {string.Join(", ", user.UsersInChats?.Select(uc => uc.Chat.ChatName) ?? Enumerable.Empty<string>())}");
          Console.WriteLine($"  Contacts: {string.Join(", ", user.Contacts?.Select(c => c.ContactUser.Username) ?? Enumerable.Empty<string>())}");
        }

        // Print Chats
        Console.WriteLine("\nChats:");
        var chats = _context.Chat
          .Include(c => c.Messages).ThenInclude(m => m.Sender)
          .Include(c => c.ChatMembers).ThenInclude(cm => cm.User)
          .ToList();
        foreach (var chat in chats)
        {
          Console.WriteLine($"Chat: {chat.ChatID}, {chat.ChatName}");
          Console.WriteLine($"  Messages: {chat.Messages?.Count}");
          Console.WriteLine($"  Members: {string.Join(", ", chat.ChatMembers?.Select(cm => cm.User.Username) ?? Enumerable.Empty<string>())}");
        }

        // Print Messages
        Console.WriteLine("\nMessages:");
        var messages = _context.Messages
          .Include(m => m.Sender)
          .Include(m => m.Chat)
          .ToList();
        foreach (var message in messages)
        {
          Console.WriteLine($"Message: {message.MessageID}, Chat: {message.Chat.ChatName}, Sender: {message.Sender.Username}, Content: {message.Content}");
        }

        // Print Contacts
        Console.WriteLine("\nContacts:");
        var contacts = _context.Contact
          .Include(c => c.User)
          .Include(c => c.ContactUser)
          .ToList();
        foreach (var contact in contacts)
        {
          Console.WriteLine($"Contact: {contact.User.Username} -> {contact.ContactUser.Username}");
        }

        // Print Notifications
        Console.WriteLine("\nNotifications:");
        var notifications = _context.Notifications
          .Include(n => n.User)
          .ToList();
        foreach (var notification in notifications)
        {
          Console.WriteLine($"Notification: {notification.NotificationID}, User: {notification.User.Username}, Content: {notification.Content}");
        }

        // Print UsersInChats
        Console.WriteLine("\nUsersInChats:");
        var usersInChats = _context.UsersInChat
          .Include(uc => uc.User)
          .Include(uc => uc.Chat)
          .ToList();
        foreach (var uc in usersInChats)
        {
          Console.WriteLine($"UsersInChat: User: {uc.User.Username}, Chat: {uc.Chat.ChatName}");
        }
      }
    }
  }
}
