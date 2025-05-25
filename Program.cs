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
    if(app.Environment.IsDevelopment()){
      using (var scope = app.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<Context>();
        Context.Seed(dbContext);
      }
      app.UseDeveloperExceptionPage();
      //app.Use(async (context, next) =>
      //    {
      //    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
      //    logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
      //    await next.Invoke();
      //    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
      //    });
    }

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
