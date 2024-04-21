using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class MonkFormDedicationScript(Dialog subject) : DialogScriptBase(subject)
{
    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass != BaseClass.Monk)
        {
            Subject.Reply(source, "How did you get here?");
            return;
        }

        if (source.Trackers.Enums.TryGetValue(out MonkElementForm _))
        {
            Subject.Reply(source, "You have already dedicated yourself to an elemental path, Aisling.");
            return;
        }

        HandleElementDedication(source);
    }

    private void HandleElementDedication(Aisling source)
    {
        var elementalType = Subject.Template.TemplateKey.Replace("fei_chose", "").ToLower();
        var chosenForm = Enum.Parse<MonkElementForm>(elementalType, true);

        source.Trackers.Enums.Set(chosenForm);
        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You've dedicated your spark to {elementalType.Humanize().Titleize()}!");

        source.Legend.AddUnique(
            new LegendMark(
                $"{elementalType.Humanize().Titleize()} Elemental Dedication",
                $"{elementalType}Monk",
                MarkIcon.Monk,
                MarkColor.Blue,
                1,
                GameTime.Now));
    }
}