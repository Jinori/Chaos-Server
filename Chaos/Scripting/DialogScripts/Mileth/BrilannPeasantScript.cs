using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class BrilannPeasantScript : DialogScriptBase
{
    public BrilannPeasantScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass.Equals(BaseClass.Peasant) && source.Trackers.Flags.HasFlag(QuestFlag1.ChosenClass))
        {
            Subject.Reply(source, "Hello, young traveler. What do you need help with?");
            Subject.Type = ChaosDialogType.Menu;

            var BrilannOptions = new List<DialogOption>
            {
                new()
                {
                    DialogKey = "brilann_learnSpells",
                    OptionText = "Learn Spells"
                },
                new()
                {
                    DialogKey = "brilann_learnSkills",
                    OptionText = "Learn Skills"
                }
            };

            Subject.Options.AddRange(BrilannOptions);
        }
    }
}