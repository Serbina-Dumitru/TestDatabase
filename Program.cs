using DbFunctionality;
using Microsoft.EntityFrameworkCore;
using TestDatabase;
using Newtonsoft.Json;

public class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    DbFunctionalityClass.FillWithData();
    var app = builder.Build();
    app.MapControllers();
    app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapGet("/SayHi", () => {
        return "Hello";
    })
    .WithName("SayHiName");

    app.MapGet("/GetAllUsers", Program.GetAllUsersAsync).WithName("GetAllUsers");

    app.MapPost("/login", Program.LogIn);

    DbFunctionalityClass.FillWithData();
    app.Run();
  }
  
  public static async Task<string> LogIn(string username, string password){
    using(var _context = new Context()){
      User? user = await _context.Users.FirstOrDefaultAsync<User>(u => u.Username == username);//.ToList();

      if(user == null) return "Such an user does not exists";
      if(user.Password != password) return "The password is incorect";

      string generatingToken = "SomeRandomTokenThatShouldBeGenerated";
      user.SessionToken = generatingToken;
      user.SessionTokenExpirationDate = DateTime.Now.AddDays(1);
      try{ await _context.SaveChangesAsync(); }
      catch(DbUpdateException){
        return "Something went wrong, unable to update the database, contact the developer.";
      }

      return generatingToken;
    }
    return "Something wen really wrong on server side whether you see this.";
  } 

  public static async Task<string> GetAllUsersAsync()
  {
    using(var _context = new Context()){
      var users =  await _context.Users
        .Include(u => u.SentMessages)
        .Include(u => u.Notifications)
        .Include(u => u.UsersInChats).ThenInclude(uc => uc.Chat)
        .Include(u => u.Contacts).ThenInclude(c => c.ContactUser)
        .Include(u => u.ContactedBy)
        .ToListAsync();
      return JsonConvert.SerializeObject(users, Formatting.Indented,
          new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
          );
    }
  }
}
