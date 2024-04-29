using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusBugReportScript(Dialog subject) : DialogScriptBase(subject)
{
    //Place Bot Token Here When Live
    private const string BOT_TOKEN = @"";
    private const ulong CHANNEL_ID = 1083522838817939566;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_sendreportaccepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!TryFetchArgs<string>(out var description))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Task.Run(
            async () =>
            {
                var client = new DiscordSocketClient();
                await client.LoginAsync(TokenType.Bot, BOT_TOKEN);
                await client.StartAsync();
                var channel = await client.GetChannelAsync(CHANNEL_ID) as IMessageChannel;

                await channel!.SendMessageAsync(
                    "```"
                    + "User Description: "
                    + description
                    + Environment.NewLine
                    + "Character Name: "
                    + source.Name
                    + Environment.NewLine
                    + "Level: "
                    + source.UserStatSheet.Level
                    + Environment.NewLine
                    + "Map Info: "
                    + source.MapInstance.Name
                    + " X:"
                    + source.X
                    + " Y:"
                    + source.Y
                    + "```");

                await client.StopAsync();
            });
    }
}