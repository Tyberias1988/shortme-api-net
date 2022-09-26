using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using shortme_api_net.Configuration;
using shortme_api_net.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace shortme_api_net;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        Log.Debug("Configuring services (startup)");

        var appConfiguration = new ApplicationConfiguration();
        Configuration.GetSection("Application").Bind(appConfiguration);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddCors(o =>
        {
            o.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddHttpContextAccessor();

        services.AddHealthChecks();

        services.AddTransient<Services.IShortLinkService, Services.ShortLinkService>();

        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.AllowTrailingCommas = true;
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
                options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                options.JsonSerializerOptions.IncludeFields = false;
                options.JsonSerializerOptions.MaxDepth = 10;
                options.JsonSerializerOptions.WriteIndented = true; // TODO If dev

                // Wait for .NET 6 (This options prevents loops by using $references... Not suitable)
                // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler ReferenceHandler.Preserve;
                // options.JsonSerializerOptions.Converters.Add(new NonLoopingJsonConverter());
            });
        services.AddSwaggerGen();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        Log.Debug("Configuring (startup)");

        var appConfiguration = new ApplicationConfiguration();
        Configuration.GetSection("Application").Bind(appConfiguration);

        app.UseSerilogRequestLogging();
        if (env.IsDevelopment())
        {
            app.UseExceptionHandler("/developmenterror");
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseCors();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "shortme-api-net");
        });

        app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
    }
}