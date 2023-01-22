using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts
{
    public class MonkFormDedicationScript : DialogScriptBase
    {
        public MonkFormDedicationScript(Dialog subject)
            : base(subject) { }

        public override void OnDisplaying(Aisling source)
        {
            var monkForm = source.Enums.TryGetValue(out MonkElementForm form);

            if (monkForm || source.UserStatSheet.BaseClass is not BaseClass.Monk)
            {
                Subject.Options.Clear();
                Subject.Text = "Go, be on your way. The path to learning is endless.";
                Subject.Type = MenuOrDialogType.Normal;
            }
            if (!monkForm)
                switch (Subject.Template.TemplateKey.ToLower())
                {
                    case "fei_chosefire":
                    {
                        source.Enums.Set(MonkElementForm.Fire);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Fire!");

                        source.Legend.AddOrAccumulate(
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
                        source.Enums.Set(MonkElementForm.Water);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Water!");

                        source.Legend.AddOrAccumulate(
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
                        source.Enums.Set(MonkElementForm.Earth);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Earth!");

                        source.Legend.AddOrAccumulate(
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
                        source.Enums.Set(MonkElementForm.Air);
                        source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You've dedicated your spark to Air!");

                        source.Legend.AddOrAccumulate(
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
        }
    }
}