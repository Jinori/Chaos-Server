using System.Text;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
using Discord;
using Discord.WebSocket;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusBugReportScript : DialogScriptBase
{
    public TerminusBugReportScript(Dialog subject) : base(subject)
    {
        InputCollector = new InputCollectorBuilder().RequestTextInput(DialogString.From(RequestDescriptionInput)).HandleInput(HandleInput)
            .RequestOptionSelection(DialogString.From(() => $"Are you sure you want to submit this report?"),
            DialogString.Yes,
            DialogString.No).HandleInput(HandleConfirmation)
            .Build();
    }

    private InputCollector InputCollector { get; }
    private string? BugDescription { get; set; }
    private string? OptionSelected { get; set; }
    
    const string BotToken = @"MTA4Mzg2MzMyNDc3MDQzOTM1MA.GUhESy.QbvKjNnh8wI_C_eagfjG550HjEFNSWZR2conHM";
    readonly ulong _channelId = 1083522838817939566;
    
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (OptionSelected.IsNullOrEmpty())
        {
            if (optionIndex.HasValue)
            {
                OptionSelected = optionIndex switch
                {
                    1 => "Skill/Spell",
                    2 => "Item",
                    3 => "Map or Warp",
                    4 => "Mundane",
                    5 => "Other",
                    _ => OptionSelected
                };
            }
            

            if (OptionSelected.IsNullOrEmpty())
            {
                Subject.Reply(source, DialogString.UnknownInput.Value);
                return;
            }
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }

    private bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;
        
        Task.Run(
            async () =>
            {
                var client = new DiscordSocketClient();
                await client.LoginAsync(TokenType.Bot, BotToken);
                await client.StartAsync();
                var channel = await client.GetChannelAsync(_channelId) as IMessageChannel;
                await channel!.SendMessageAsync($"```" + "Bug Topic: " + OptionSelected + Environment.NewLine
                                                + "User Description: " + BugDescription + Environment.NewLine +
                                                "Character Name: " + aisling.Name + Environment.NewLine +
                                                "Level: " + aisling.UserStatSheet.Level + Environment.NewLine +
                                                "Map Info: " + aisling.MapInstance.Name + " X:" + aisling.X + " Y:" + aisling.Y

                                                + "```");
                await client.StopAsync();
            });

        return true;
    }

    private string RequestDescriptionInput()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Please briefly describe your bug or suggestion!");
        return builder.ToString();
    }

    private bool HandleInput(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (!dialog.MenuArgs.TryGet<string>(0, out var description))
        {
            dialog.Reply(aisling, DialogString.UnknownInput.Value);
            return false;
        }

        BugDescription = description;
        
        return true;
    }
    
}