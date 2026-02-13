using Serilog;
using dotenv.net;
using joker_api.Services.Interfaces;
using joker_api.Services.Services;

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
    builder.Services.AddScoped<IJokeService, JokeService>();

    // Add services to the container.
    var app = builder.Build();



    // Configure the HTTP request pipeline.
    app.UseHttpsRedirection();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();

        app.MapOpenApi();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Joker API V1");
            options.RoutePrefix = "swagger";
        });
    }

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    // Log the exception using Serilog
    Log.Fatal(ex, "An unhandled exception occurred.");
}

#endregion

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}