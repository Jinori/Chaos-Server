using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class MonkFormDedicationScript : DialogScriptBase
{
    public MonkFormDedicationScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        var monkForm = source.Trackers.Enums.TryGetValue(out MonkElementForm form);

        if (source.UserStatSheet.BaseClass is not BaseClass.Monk)
        {
            Subject.Reply(source, "How did you get here?.");

            return;
        }

        switch (monkForm)
        {
            case true:
                Subject.Reply(source, "You have already dedicated yourself to a elemental path, Aisling.");

                return;
            case false:
                switch (Subject.Template.TemplateKey.ToLower())
                {
                    case "fei_chosefire":
                    {
                        source.Trackers.Enums.Set(MonkElementForm.Fire);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Fire!");

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Fire Elemental Dedication",
                                "fireMonk",
                                MarkIcon.Monk,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }

                        break;

                    case "fei_chosewater":
                    {
                        source.Trackers.Enums.Set(MonkElementForm.Water);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Water!");

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Water Elemental Dedication",
                                "waterMonk",
                                MarkIcon.Monk,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }

                        break;

                    case "fei_choseearth":
                    {
                        source.Trackers.Enums.Set(MonkElementForm.Earth);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Earth!");

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Earth Elemental Dedication",
                                "earthMonk",
                                MarkIcon.Monk,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }

                        break;

                    case "fei_choseair":
                    {
                        source.Trackers.Enums.Set(MonkElementForm.Air);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Air!");

                        source.Legend.AddUnique(
                            new LegendMark(
                                "Air Elemental Dedication",
                                "airMonk",
                                MarkIcon.Monk,
                                MarkColor.Blue,
                                1,
                                GameTime.Now));
                    }

                        break;
                }

                break;
        }
    }
}