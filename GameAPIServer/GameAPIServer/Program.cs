using GameAPIServer.DatabaseServices.GameDb;
using GameAPIServer.Filter;
using GameAPIServer.MiddleWare;
using ZLogger;

// delay for DB containers
await Task.Delay(10000);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<EmailFormatCheckFilter>();
    options.Filters.Add<NicknameFormatCheckFilter>();
    options.Filters.Add<PasswordFormatCheckFilter>();
});
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddZLoggerFile("logs/mainLog.log", options => { options.EnableStructuredLogging = true; });
    logging.AddZLoggerRollingFile((dt, x) => $"logs/{dt.ToLocalTime():yyyy-MM-dd}_{x:000}.log", x => x.ToLocalTime().Date, 1024);
    logging.AddZLoggerConsole(options => { options.EnableStructuredLogging = true; });
});
builder.Services.AddTransient<IGameDbService, MysqlGameDbService>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<BodyParsingMiddleware>();
app.Run();
