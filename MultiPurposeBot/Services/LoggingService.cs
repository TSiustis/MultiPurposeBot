using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MultiPurposeBot.Services;
public class LoggingService
{
    private readonly ILogger _logger;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;

    public LoggingService(IServiceProvider services)
    {
        _discord = services.GetRequiredService<DiscordSocketClient>();
        _commands = services.GetRequiredService<CommandService>();
        _logger = services.GetRequiredService<ILogger<LoggingService>>();
        
        _discord.Ready += OnReadyAsync;
        _discord.Log += OnLogAsync;
        _commands.Log += OnLogAsync;
    }
    
    public Task OnReadyAsync()
    {
        _logger.LogInformation($"Connected as -> [] :)");
        return Task.CompletedTask;
    }
    
    public Task OnLogAsync(LogMessage msg)
    {
        var logText = $": {msg.Exception?.ToString() ?? msg.Message}";
        switch (msg.Severity.ToString())
        {
            case "Critical":
                {
                    _logger.LogCritical(logText);
                    break;
                }
            case "Warning":
                {
                    _logger.LogWarning(logText);
                    break;
                }
            case "Info":
                {
                    _logger.LogInformation(logText);
                    break;
                }
            case "Verbose":
                {
                    _logger.LogInformation(logText);
                    break;
                }
            case "Debug":
                {
                    _logger.LogDebug(logText);
                    break;
                }
            case "Error":
                {
                    _logger.LogError(logText);
                    break;
                }
        }

        return Task.CompletedTask;
    }
}
