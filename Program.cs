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
    builder.Services.AddDbContext<Context>(options => options.UseSqlite("Data Source = Messenger.db"),ServiceLifetime.Scoped);

    var app = builder.Build();
#if DEBUG
    using (var scope = app.Services.CreateScope())
    {
      var services = scope.ServiceProvider;
      var dbContext = services.GetRequiredService<Context>();
      Context.Seed(dbContext);
    }
#endif
    app.MapControllers();
    app.UseHttpsRedirection();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapGet("/", ()=>{
        return "Test database is working :3";
        });

    app.Run();
  }
}
