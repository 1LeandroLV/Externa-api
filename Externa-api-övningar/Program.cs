using Externa_api_övningar.Services;


namespace Externa_api_övningar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient < ExternalApiClient> (client =>
            {
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.MapGet("/posts", async (ExternalApiClient externa) =>
            {
                try
                {
                    var posts = await externa.GetPostsAsync(10);
                    return Results.Ok(posts);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
