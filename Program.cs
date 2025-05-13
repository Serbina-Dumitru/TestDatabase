using DbFunctionality;
using Microsoft.EntityFrameworkCore;
using TestDatabase;
using Newtonsoft.Json;

public class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    var app = builder.Build();

    app.UseHttpsRedirection();
    app.MapGet("/SayHi", () => {
        return "Hello";
    })
    .WithName("SayHiName");

    app.MapGet("/GetAllUsers", () => {
      using(var _context = new Context()){
        var users =  _context.Users
          .Include(u => u.SentMessages)
          .Include(u => u.Notifications)
          .Include(u => u.UsersInChats).ThenInclude(uc => uc.Chat)
          .Include(u => u.Contacts).ThenInclude(c => c.ContactUser)
          .Include(u => u.ContactedBy)
          .ToList();
        //var options = new JsonSerializerOptions{ ReferenceHandler = ReferenceHandler.IgnoreCycles};
        //return Results.Json(users,options);
        return JsonConvert.SerializeObject(users, Formatting.Indented,
            new JsonSerializerSettings()
            { 
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
        })
    .WithName("GetAllUsers");

    //app.MapPost("/login", async (string token) => {
    //  using(var _context = new Context()){
    //    var user = _context.Users.Select(u => u.SessionToken == token);
    //    if(user.Count == 0) return 0;
    //  }
    //});

    //DbFunctionalityClass.PrintAllTheDb();
    app.Run();
  }
}
