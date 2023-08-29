using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.PentagramQuest;

public sealed class PentaPactScript : ReactorTileScriptBase
{

    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public PentaPactScript(ReactorTile subject, IDialogFactory dialogFactory, IItemFactory itemFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        ItemFactory = itemFactory;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage);
        if (hasStage)
        {
            source.SendOrangeBarMessage("You have already signed the pact.");
            return;
        }

        if (source.Trackers.TimedEvents.HasActiveEvent("PentagramQuestTimer", out var timer))
        {
            source.SendOrangeBarMessage("You must wait before signing the pact again.");
            return;
        }

        var requiredClasses = new HashSet<BaseClass>
        {
            BaseClass.Warrior,
            BaseClass.Rogue,
            BaseClass.Wizard,
            BaseClass.Priest,
            BaseClass.Monk
        };

        var encounteredClasses = new HashSet<BaseClass>();

        if (source.UserStatSheet.Level < 60)
        {
           source.SendOrangeBarMessage("That book looks frightening, better not touch it.");

            return;
        }

        if (source.Group == null)
        {
            source.SendOrangeBarMessage("You glance at a book about a pact... but are afraid to look alone.");

            return;
        }

        if (source.Group != null)
            foreach (var groupMember in source.Group)
            {
                var memberClass = groupMember.UserStatSheet.BaseClass;

                switch (source.Group.Count)
                {
                    case < 5:
                        source.SendOrangeBarMessage("You'll need more people to sign the pact.");

                        return;
                    case > 5:
                        source.SendOrangeBarMessage("There's too many people in your group to sign the pact.");

                        return;
                }

                if (requiredClasses.Contains(memberClass))
                {
                    if (encounteredClasses.Contains(memberClass))
                    {
                        // Handle the case where there's more than one of a required class
                        source.Client.SendServerMessage(
                            ServerMessageType.OrangeBar1,
                            "You can only have one member of each required class in your group.");

                        return;
                    } else
                    {
                        encounteredClasses.Add(memberClass);
                    }
                } else
                {
                    // Handle the case where a class that isn't required is in the group
                    source.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "Your group contains a class that is not required.");

                    return;
                }
            }

        // Check if all required classes have been encountered
        if (encounteredClasses.Count == requiredClasses.Count)
        {
            // Create a dialog based on the class of the character
            var pact = ItemFactory.Create("pentapactbook");
            var classDialog = DialogFactory.Create("pentapact", pact);

            classDialog.Display(source);
        }
    }
}