using DbFunctionality;

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

    DbFunctionalityClass.PrintAllTheDb();
    app.Run();
  }
}
