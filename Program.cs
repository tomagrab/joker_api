using Serilog;
using dotenv.net;
using joker_api.Services.Interfaces;
using joker_api.Services.Services;
using joker_api.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

#region - App-wide try-catch block to catch unhandled exceptions and log them

try
{
    #region - Load environment variables from .env file

    DotEnv.Load();

    #endregion

    #region - Configure Serilog for logging to console, file, and MSSQL database

    // Get the `DB_CONNECTION_STRING` environment variable
    var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");


    // Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("./logs/log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.MSSqlServer(
            connectionString: dbConnectionString,
            sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = true
            })
        .CreateLogger();

    #endregion

    // Log that the application is starting
    Log.Information("Starting Joker API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(dbConnectionString);
    });
    builder.Services.AddScoped<IJokeService, JokeService>();

    // Add services to the container.
    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
        app.MapOpenApi();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Joker API V1");
            options.RoutePrefix = "swagger";
        });
    }

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();

        context.Database.EnsureCreated();

        // Migrate the database if there are any pending migrations
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    // Log the exception using Serilog
    Log.Fatal(ex, "An unhandled exception occurred.");
}

#endregion