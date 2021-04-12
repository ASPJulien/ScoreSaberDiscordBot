using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace ScoreSaberDiscordBot
{
  internal class Program
  {
    private DiscordSocketClient client;
    private CommandService commands;

    public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    public async Task RunBotAsync()
    {
      client = new DiscordSocketClient(new DiscordSocketConfig
      {
        LogLevel = LogSeverity.Info
      });

      commands = new CommandService();
      
      client.Log += Log;
      client.Ready += () =>
      {
        return Task.CompletedTask;
      };

      await InstallCommandsAsync();
      PlayersDB.ReadDB();

      
      
      await client.LoginAsync(TokenType.Bot, "");
      await client.StartAsync();
      
      await Task.Delay(-1);
    }

    public async Task InstallCommandsAsync()
    {
      client.MessageReceived += HandleCommandAsync;
      await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
    }

    private async Task HandleCommandAsync(SocketMessage pMessage)
    {
      var message = pMessage as SocketUserMessage;
      if(message == null) return;

      int argPos = 0;
      if(!message.HasCharPrefix('!', ref argPos)) return;

      var context = new SocketCommandContext(client, message);
      var result = await commands.ExecuteAsync(context, argPos, null);
      
      //error
      if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
    }

    private Task Log(LogMessage arg)
    {
      Console.WriteLine(arg.ToString());
      return Task.CompletedTask;
    }
  }
}