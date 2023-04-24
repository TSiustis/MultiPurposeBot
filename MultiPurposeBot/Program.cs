using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiPurposeBot.Services;

namespace MultiPurposeBot
{
    class Program
    {
        private DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private InteractionService _commands;
        private ulong _testGuilldId;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync(string[] args)
        {

        }

        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            _config = _builder.Build();
            _testGuilldId = ulong.Parse(_config["TestGuildId"]);
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var commands = services.GetRequiredService<InteractionService>();
                _client = client;
                _commands = commands;

                _client.Log += Log;
                _commands.Log += Log;
                _client.Ready += ReadyAsync;

                await _client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                await Task.Delay(Timeout.Infinite);
            }
            
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            if(IsDebug())
            {
                System.Console.WriteLine("Debug mode enabled.");
                await _commands.RegisterCommandsToGuildAsync(_testGuilldId);
            }
            else{
                await _commands.RegisterCommandsGloballyAsync(true);
            }

            Console.WriteLine($"Connected to Discord as {_client.CurrentUser.Username}");
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        static bool IsDebug ( )
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }
}