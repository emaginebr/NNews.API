using NNews.Application;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

// Configuração do Serilog ANTES de criar o builder
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()  // Nível mais baixo do Serilog (equivalente a Trace)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "NNews.API")
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        restrictedToMinimumLevel: LogEventLevel.Verbose,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] [{ThreadId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting NNews API application");

    var builder = WebApplication.CreateBuilder(args);

    // Adiciona Serilog ao ASP.NET Core
    builder.Host.UseSerilog();

    // Carrega appsettings baseado no ambiente
    var environment = builder.Environment.EnvironmentName;
    Log.Information("Running in {Environment} environment", environment);
    
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    // Configuração de CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure Kestrel for HTTPS if certificate is available
    var certPath = builder.Configuration["ASPNETCORE_Kestrel__Certificates__Default__Path"];
    var certPassword = builder.Configuration["ASPNETCORE_Kestrel__Certificates__Default__Password"];
    
    if (!string.IsNullOrEmpty(certPath) && System.IO.File.Exists(certPath))
    {
        Log.Information("Configuring HTTPS with certificate: {CertPath}", certPath);
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(certPath, certPassword);
            });
        });
    }
    else
    {
        Log.Warning("HTTPS certificate not configured or file not found at: {CertPath}", certPath ?? "NOT SET");
    }

    Initializer.Configure(builder.Services, builder.Configuration.GetConnectionString("NNewsContext"), builder.Configuration);

    var app = builder.Build();

    // Adiciona middleware do Serilog para logging de requisições HTTP
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;  // Log todas requisições como Debug
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RequestBody", httpContext.Request.ContentLength);
            diagnosticContext.Set("ResponseBody", httpContext.Response.ContentLength);
        };
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection();

    // Habilitar CORS - DEVE vir antes de UseAuthentication e UseAuthorization
    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("NNews API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
