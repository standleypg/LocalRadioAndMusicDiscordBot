using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using radio_discord_bot.Commands;
using radio_discord_bot.Services;
using radio_discord_bot.Services.Interfaces;
using radio_discord_bot.Store;
using YoutubeExplode;

namespace radio_discord_bot;

public static class DependencyInjection
{
    public static IServiceProvider CreateProvider()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMembers | GatewayIntents.MessageContent | GatewayIntents.DirectMessages
        };

        var collection = new ServiceCollection()
        .AddSingleton(config)
        .AddSingleton<DiscordSocketClient>()
        .AddSingleton<CommandService>()
        .AddSingleton<Startup>()
        .AddSingleton<RadioCommand>()
        .AddSingleton<GlobalStore>()
        .AddScoped<YoutubeClient>()
        .AddTransient<IAudioService, AudioService>()
        .AddSingleton<IJokeService, JokeService>()
        .AddSingleton<IQuoteService, QuoteService>()
        .AddSingleton<IHttpRequestService, HttpRequestService>()
        .AddSingleton<IInteractionService, InteractionService>();

        return collection.BuildServiceProvider();
    }
}
