﻿using System.Text.Json;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using radio_discord_bot;
using radio_discord_bot.Handlers;
using radio_discord_bot.Services;
using YoutubeExplode;

public class Program
{
    private readonly string _token;

    private readonly IServiceProvider _serviceProvider;
    public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    public Program()
    {
        _serviceProvider = CreateProvider();

        IConfiguration configuration = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .Build();

        _token = configuration.GetSection("BotToken").Value!;
    }

    static IServiceProvider CreateProvider()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMembers | GatewayIntents.MessageContent | GatewayIntents.DirectMessages
        };

        var collection = new ServiceCollection()
        .AddSingleton(config)
        .AddSingleton<DiscordSocketClient>()
        .AddTransient<AudioService>()
        .AddTransient<YoutubeClient>()
        .AddSingleton<PlaylistService>()
        .AddTransient<CommandHandler>()
        .AddTransient<MessageService>()
        .AddTransient<InteractionsService>();

        //...
        return collection.BuildServiceProvider();
    }

    public async Task RunBotAsync()
    {
        var _messageService = _serviceProvider.GetRequiredService<MessageService>();
        var _interactionsService = _serviceProvider.GetRequiredService<InteractionsService>();
        var _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += _messageService.TextChannelMessageReceivedAsync;
        _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;

        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();

        _client.InteractionCreated += _interactionsService.OnInteractionCreated;

        await Task.Delay(-1);
    }

    public async Task ReadyAsync()
    {
        var _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

        await Task.CompletedTask;
        Console.WriteLine($"Logged in as {_client.CurrentUser.Username}");
    }

    private async Task LogAsync(LogMessage log)
    {
        await Task.CompletedTask;
        Console.WriteLine(log);
    }

    private async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
    {
        //TODO - disconnect bot if only bot is in voice channel
    }
}
