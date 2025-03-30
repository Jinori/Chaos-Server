using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusGroupLootScript : DialogScriptBase
{
    /// <inheritdoc />
    public TerminusGroupLootScript(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "terminus_groupoptions",
                    OptionText = "Group Loot Options"
                };

                if (!Subject.HasOption(option.OptionText) && (source.Group != null) && source.Group.Leader.Equals(source))
                    Subject.Options.Insert(0, option);

                return;
            }

            case "terminus_lootdefault":
            {
                if (source.Group is null)
                {
                    Subject.Reply(source, "You are not in a group.");

                    return;
                }

                if (source.Group.Leader.Equals(source))
                    source.Group.LootOption = Group.GroupLootOption.Default;

                foreach (var member in source.Group)
                    member.SendServerMessage(ServerMessageType.ActiveMessage, $"{source.Name} set loot options to defaults.");

                break;
            }

            case "terminus_lootrandom":
            {
                if (source.Group is null)
                {
                    Subject.Reply(source, "You are not in a group.");

                    return;
                }

                if (source.Group.Leader.Equals(source))
                    source.Group.LootOption = Group.GroupLootOption.Random;

                foreach (var member in source.Group)
                    member.SendServerMessage(ServerMessageType.ActiveMessage, $"{source.Name} set loot options to random mode.");

                break;
            }

            case "terminus_lootmaster":
            {
                if (source.Group is null)
                {
                    Subject.Reply(source, "You are not in a group.");

                    return;
                }

                if (source.Group.Leader.Equals(source))
                    source.Group.LootOption = Group.GroupLootOption.MasterLooter;

                foreach (var member in source.Group)
                    member.SendServerMessage(ServerMessageType.ActiveMessage, $"{source.Name} set loot options to Master Looter.");

                break;
            }
        }
    }
}