using Microsoft.EntityFrameworkCore;

namespace TestDatabase;

public class Context : DbContext
{
  public DbSet<User>         Users         {get;set;}
  public DbSet<Message>      Messages      {get;set;}
  public DbSet<UsersInChat>  UsersInChat   {get;set;}
  public DbSet<Contact>      Contact       {get;set;}
  public DbSet<Chat>         Chat          {get;set;}
  public DbSet<Notification> Notifications {get;set;}

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
}
/*
   DbContext is Not thread-safe. Don't share contexts between threads. Make sure to await all async calls before continuing to use the context instance.
   */
