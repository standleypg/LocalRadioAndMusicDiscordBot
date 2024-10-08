using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace radio_discord_bot;

public class Startup(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider, ILogger<Startup> logger)
{
    public async Task SetupLoggingAndReadyEvents()
    {
        await Task.CompletedTask;
        client.Log += async log =>
        {
            await Task.CompletedTask;
            logger.LogInformation(log.ToString());
            // Console.WriteLine(log);
        };
        client.Ready += async () =>
        {
            await Task.CompletedTask;
            logger.LogInformation($"Logged in as {client.CurrentUser.Username}");
        };
    }

    public async Task SetupCommandHandling()
    {
        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

        client.MessageReceived += MessageReceived;
    }

    private async Task MessageReceived(SocketMessage arg)
    {
        await Task.CompletedTask;
        _ = Task.Run(async () =>
        {
            if (arg is not SocketUserMessage msg) return;
            var context = new SocketCommandContext(client, msg);
            var argPos = 0;

            var isHelpDm = context.IsPrivate && msg.ToString().Equals("help");

            if (msg.HasStringPrefix("/", ref argPos) || isHelpDm)
            {
                var result = await commands.ExecuteAsync(context, isHelpDm ? 0 : argPos, serviceProvider);

                if (!result.IsSuccess)
                {
                    logger.LogError($"OnMesssageReceived: {result.ErrorReason}");
                }
            }
        });
    }
}