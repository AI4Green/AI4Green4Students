namespace AI4Green4Students.Startup.Web;

public static class WebEntrypoint
{
    public static async Task Run(string[] args)
    {
        var b = WebApplication.CreateBuilder(args);

        // Configure DI Services
        b.ConfigureServices();
        
        // Build the app
        var app = b.Build();
    
        // Initialisation before the app runs
        await app.Initialise();

        // Configure the HTTP Request Pipeline
        app.UseWebPipeline();

        // Run the app!
        await app.RunAsync();
    }
}
