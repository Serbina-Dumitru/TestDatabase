using Microsoft.EntityFrameworkCore;

namespace TestDatabase;

public class Context : DbContext
{
  public DbSet<User> Users { get; set; }
  public DbSet<Message> Messages { get; set; }
  public DbSet<UsersInChat> UsersInChat { get; set; }
  public DbSet<Contact> Contact { get; set; }
  public DbSet<Chat> Chat { get; set; }
  public DbSet<Notification> Notifications { get; set; }

  public Context(DbContextOptions options) : base(options)
  {

  }
  public Context() : base()
  {
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
    optionsBuilder.UseSqlite("Data Source = Messenger.db");
  }



  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>()
      .HasMany(u => u.SentMessages)
      .WithOne(m => m.Sender)
      .HasForeignKey(m => m.UserID)
      .IsRequired();

    modelBuilder.Entity<User>()
      .HasMany(u => u.Notifications)
      .WithOne(n => n.User)
      .HasForeignKey(n => n.UserID)
      .IsRequired();

    modelBuilder.Entity<User>()
      .HasMany(u => u.UsersInChats)
      .WithOne(uc => uc.User)
      .HasForeignKey(uc => uc.UserID)
      .IsRequired();

    modelBuilder.Entity<User>()
      .HasMany(u => u.Contacts)
      .WithOne(c => c.User)
      .HasForeignKey(uc => uc.UserID)
      .IsRequired();

    //modelBuilder.Entity<User>()
    //  .HasMany(u => u.ContactedBy)
    //  .WithOne(c => c.User)
    //  .HasForeignKey(uc => uc.UserID)
    //  .IsRequired();

    modelBuilder.Entity<Chat>()
      .HasMany(c => c.Messages)
      .WithOne(m => m.Chat)
      .HasForeignKey(m => m.ChatID)
      .IsRequired();

    modelBuilder.Entity<Chat>()
      .HasMany(c => c.ChatMembers)
      .WithOne(m => m.Chat)
      .HasForeignKey(m => m.ChatID)
      .IsRequired();

    modelBuilder.Entity<User>()
      .Property(u => u.IsOnline)
      .HasDefaultValue(false);
    modelBuilder.Entity<Message>()
      .Property(u => u.IsSeen)
      .HasDefaultValue(false);
    modelBuilder.Entity<Message>()
      .Property(u => u.IsFile)
      .HasDefaultValue(false);
  }

  public static void Seed(Context _context)
  {
    _context.Database.EnsureCreated();

    List<Chat> chats = new List<Chat>();
    if (!_context.Chat.Any())
    {
      chats.AddRange(
          new() { ChatID = 1, ChatName = "Weekend Plans" },
          new() { ChatID = 2, ChatName = "Project Alpha Team" },
          new() { ChatID = 3, ChatName = "Family Group" },
          new() { ChatID = 4, ChatName = "Fitness Buddies" },
          new() { ChatID = 5, ChatName = "Book Club" },
          new() { ChatID = 6, ChatName = "Movie Night" },
          new() { ChatID = 7, ChatName = "Work Updates" },
          new() { ChatID = 8, ChatName = "Gaming Crew" },
          new() { ChatID = 9, ChatName = "Writers Circle" },
          new() { ChatID = 10, ChatName = "Travel Squad" }
          );
      _context.Chat.AddRange(chats);
      _context.SaveChanges();
    }





    List<User> users = new List<User>();
    if (!_context.Users.Any())
    {
      users.AddRange(
          new() {UserID = 1, Username = "alice", Password = "AlicePass123", Email = "alice@example.com", IsOnline = true, LastTimeOnline = DateTime.UtcNow.AddMinutes(-120), SessionToken = "token-alice", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/alice.jpg" },
          new() { UserID = 2, Username = "bob", Password = "BobPass123", Email = "bob@example.com", IsOnline = false,  LastTimeOnline = DateTime.UtcNow,SessionToken = "token-bob", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/bob.jpg" },
          new() { UserID = 3, Username = "carla", Password = "CarlaPass123", Email = "carla@example.com", IsOnline = true,  LastTimeOnline = DateTime.UtcNow,SessionToken = "token-carla", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/carla.jpg" },
          new() { UserID = 4, Username = "daniel", Password = "DanielPass123", Email = "daniel@example.com", IsOnline = true,  LastTimeOnline = DateTime.UtcNow,SessionToken = "token-daniel", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/daniel.jpg" },
          new() { UserID = 5, Username = "emily", Password = "EmilyPass123", Email = "emily@example.com", IsOnline = false, LastTimeOnline = DateTime.UtcNow, SessionToken = "token-emily", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/emily.jpg" },
          new() { UserID = 6, Username = "frank", Password = "FrankPass123", Email = "frank@example.com", IsOnline = true, LastTimeOnline = DateTime.UtcNow, SessionToken = "token-frank", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/frank.jpg" },
          new() { UserID = 7, Username = "grace", Password = "GracePass123", Email = "grace@example.com", IsOnline = true, LastTimeOnline = DateTime.UtcNow, SessionToken = "token-grace", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/grace.jpg" },
          new() { UserID = 8, Username = "henry", Password = "HenryPass123", Email = "henry@example.com", IsOnline = false, LastTimeOnline = DateTime.UtcNow, SessionToken = "token-henry", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/henry.jpg" },
          new() { UserID = 9, Username = "isabel", Password = "IsabelPass123", Email = "isabel@example.com", IsOnline = true, LastTimeOnline = DateTime.UtcNow, SessionToken = "token-isabel", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/isabel.jpg" },
          new() { UserID = 10, Username = "jack", Password = "JackPass123", Email = "jack@example.com", IsOnline = true, LastTimeOnline = DateTime.UtcNow, SessionToken = "token-jack", SessionTokenExpirationDate = DateTime.UtcNow.AddDays(1), UserProfilePicturePath = "images/profiles/jack.jpg" }
          );
      _context.Users.AddRange(users);
      _context.SaveChanges();
    }

    List<Notification> notifications = new List<Notification>();
    if (!_context.Notifications.Any())
    {
      notifications.AddRange(
          new() { NotificationID = 1, UserID = 1, Content = "New message in Weekend Plans", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
          new() { NotificationID = 2, UserID = 2, Content = "Daniel mentioned you in Project Alpha Team", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
          new() { NotificationID = 3, UserID = 3, Content = "Reminder: Book Club at 7PM", Timestamp = DateTime.UtcNow.AddMinutes(-15) },
          new() { NotificationID = 4, UserID = 4, Content = "Isabel added a new file", Timestamp = DateTime.UtcNow.AddMinutes(-20) },
          new() { NotificationID = 5, UserID = 5, Content = "Jack started a video call", Timestamp = DateTime.UtcNow.AddMinutes(-25) },
          new() { NotificationID = 6, UserID = 6, Content = "Grace sent a photo", Timestamp = DateTime.UtcNow.AddMinutes(-30) },
          new() { NotificationID = 7, UserID = 7, Content = "Bob reacted to your message", Timestamp = DateTime.UtcNow.AddMinutes(-35) },
          new() { NotificationID = 8, UserID = 8, Content = "Meeting starts in 15 minutes", Timestamp = DateTime.UtcNow.AddMinutes(-40) },
          new() { NotificationID = 9, UserID = 9, Content = "Frank updated the group description", Timestamp = DateTime.UtcNow.AddMinutes(-45) },
          new() { NotificationID = 10, UserID = 10, Content = "New contact request from Carla", Timestamp = DateTime.UtcNow.AddMinutes(-50) }
          );
      _context.Notifications.AddRange(notifications);
      _context.SaveChanges();
    }

    List<Contact> contacts = new List<Contact>();
    if (!_context.Contact.Any())
    {
      contacts.AddRange(
          new() { ContactID = 1, UserID = 1, ContactUserID = 2 },
          new() { ContactID = 2, UserID = 2, ContactUserID = 3 },
          new() { ContactID = 3, UserID = 3, ContactUserID = 4 },
          new() { ContactID = 4, UserID = 4, ContactUserID = 5 },
          new() { ContactID = 5, UserID = 5, ContactUserID = 6 },
          new() { ContactID = 6, UserID = 6, ContactUserID = 7 },
          new() { ContactID = 7, UserID = 7, ContactUserID = 8 },
          new() { ContactID = 8, UserID = 8, ContactUserID = 9 },
          new() { ContactID = 9, UserID = 9, ContactUserID = 10 },
          new() { ContactID = 10, UserID = 10, ContactUserID = 1 }
          );
      _context.Contact.AddRange(contacts);
      _context.SaveChanges();
    }

    List<Message> messages = new List<Message>();
    if (!_context.Messages.Any())
    {
      messages.AddRange(
          new() { MessageID = 1, ChatID = 1, UserID = 1, Content = "Anyone up for hiking this weekend?", TimeStamp = DateTime.UtcNow.AddMinutes(-60), IsSeen = false, IsFile = false },
          new() { MessageID = 2, ChatID = 2, UserID = 2, Content = "Updated the document on our shared drive.", TimeStamp = DateTime.UtcNow.AddMinutes(-55), IsSeen = true, IsFile = false },
          new() { MessageID = 3, ChatID = 3, UserID = 3, Content = "Dinner at 7 sounds good!", TimeStamp = DateTime.UtcNow.AddMinutes(-50), IsSeen = true, IsFile = false },
          new() { MessageID = 4, ChatID = 4, UserID = 4, Content = "Don't forget leg day!", TimeStamp = DateTime.UtcNow.AddMinutes(-45), IsSeen = false, IsFile = false },
          new() { MessageID = 5, ChatID = 5, UserID = 5, Content = "Has everyone finished chapter 3?", TimeStamp = DateTime.UtcNow.AddMinutes(-40), IsSeen = false, IsFile = false },
          new() { MessageID = 6, ChatID = 6, UserID = 6, Content = "Let's watch Inception tonight!", TimeStamp = DateTime.UtcNow.AddMinutes(-35), IsSeen = true, IsFile = false },
          new() { MessageID = 7, ChatID = 7, UserID = 7, Content = "Meeting moved to 3pm.", TimeStamp = DateTime.UtcNow.AddMinutes(-30), IsSeen = true, IsFile = false },
          new() { MessageID = 8, ChatID = 8, UserID = 8, Content = "Game night at my place?", TimeStamp = DateTime.UtcNow.AddMinutes(-25), IsSeen = false, IsFile = false },
          new() { MessageID = 9, ChatID = 9, UserID = 9, Content = "Final draft submitted!", TimeStamp = DateTime.UtcNow.AddMinutes(-20), IsSeen = true, IsFile = true },
          new() { MessageID = 10, ChatID = 10, UserID = 10, Content = "Flight's at 6AM on Friday!", TimeStamp = DateTime.UtcNow.AddMinutes(-15), IsSeen = false, IsFile = false }
          );
      _context.Messages.AddRange(messages);
      _context.SaveChanges();
    }

    List<UsersInChat> usersInChats = new List<UsersInChat>();
    if (!_context.UsersInChat.Any())
    {
      int usersInChatId = 1;
      for (int i = 0; i < chats.Count; i++)
      {
        int userId1 = (i % 10) + 1;
        int userId2 = ((i + 1) % 10) + 1;
        usersInChats.Add(new UsersInChat { UsersInChatID = usersInChatId++, ChatID = chats[i].ChatID, UserID = userId1 });
        usersInChats.Add(new UsersInChat { UsersInChatID = usersInChatId++, ChatID = chats[i].ChatID, UserID = userId2 });
      }
      _context.UsersInChat.AddRange(usersInChats);
      _context.SaveChanges();
    }
  }
}
/*
   DbContext is Not thread-safe. Don't share contexts between threads. Make sure to await all async calls before continuing to use the context instance.
   */
