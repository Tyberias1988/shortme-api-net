using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;
using shortme_api_net.Configuration;
using shortme_api_net.Models;

namespace shortme_api_net;

public class Program
{

    
    public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateBootstrapLogger();

            try
            {
                Log.Debug("Starting");

                //TODO: Timezone-Thema kl�ren und hier raus
                Log.Debug("Enabling temporary postgres datetime handling fallback (.net5 => .net6");
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                Log.Debug("Configure Host");
                var host = CreateHostBuilder(args).Build();
                
                using (var scope = host.Services.CreateScope())
                {
                    Log.Information("Migrating Database");
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    db.Database.Migrate();
                }
                
                Log.Debug("Running Host");
                host.Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    
    public static IHostBuilder CreateHostBuilder(string[] args) => Host
           .CreateDefaultBuilder(args)
           .ConfigureHostConfiguration((configuration =>
           {

           }))
           .ConfigureAppConfiguration((context, configuration) =>
           {
               Log.Debug("Configuring appConfiguration");

               configuration
                   .SetBasePath(AppContext.BaseDirectory)
                   .AddEnvironmentVariables("ASP_")
                   .AddCommandLine(args);
           })
           .ConfigureLogging((hostContext, logging) =>
           {
               // Not used for serilog!
           })
           .UseSerilog()
           .ConfigureServices((hostContext, services) =>
           {
               Log.Debug("Configuring services");

               var appConfiguration = new ApplicationConfiguration();
               hostContext.Configuration.GetSection("App").Bind(appConfiguration);
           })
           .ConfigureWebHostDefaults(webBuilder =>
           {
               Log.Debug("Configuring webHostDefaults");
               webBuilder 
                   .UseKestrel()
                   .UseStartup<Startup>();
           })
            .UseSerilog((context, services, loggingConfig) =>
            {

                //// var hubContext = services.GetService<IHubContext<Hubs.LogHub>>();
                loggingConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("Version", "todo")
                    .Enrich.WithCorrelationId()
                    .WriteTo.Console(LogEventLevel.Verbose,
                        "[{Timestamp:HH:mm:ss} {CorrelationId} {SourceContext} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                        theme: AnsiConsoleTheme.Code)
                    .WriteTo.Debug()
                    // .Enrich.WithHttpContext(services)
                    .Enrich.WithCorrelationId();
            }, true, false);
    
}