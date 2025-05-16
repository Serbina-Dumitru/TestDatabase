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
    app.MapControllers();
    app.UseHttpsRedirection();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");


    DbFunctionalityClass.FillWithData();
    app.Run();
  }

}
