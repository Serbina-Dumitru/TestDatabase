//using Models;
//litecli, sqlitebrowser
using Microsoft.EntityFrameworkCore;
using TestDatabase;
internal class Program
{
  private static void FillWithData(){
    using(var _context = new Context()){
      _context.Database.EnsureCreated();
      _context.Users.Add(new User{
          Username = "test",
          Email    = "a@a.a",
          IsOnline = false,
          SessionToken = "",
          SessionTokenExpirationDate = DateTime.Now,
          UserProfilePicturePath = "./test"
          });
      _context.Chat.Add(new Chat{
          ChatName = "First Chat",
          });
      _context.Messages.Add(new Message{
          UserID = 1,
          ChatID = 1,
          Content = "hello",
          TimeStamp = DateTime.Now,
          IsSeen = false,
          IsFile = false,
          });
      _context.UsersInChat.Add(new UsersInChat{
          ChatID = 1,
          UserID = 1, 
          });

      _context.Notifications.Add(new Notification{
          UserID = 1,
          Content = "new notification",
          Timestamp = DateTime.Now,
          });

      _context.Contact.Add(new Contact{
          UserID = 1,
          ContactUserID = 1
          });
      _context.SaveChanges();
    }
  }
  private static void QuerryAllDbNames(){
    using(var _context = new Context()){
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
  private static void PrintAllTheDb(){
    Console.WriteLine();
    using(var _context = new Context()){
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
  private static void Main(string[] args){
    PrintAllTheDb();
  }
  
}
