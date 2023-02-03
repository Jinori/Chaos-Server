using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts
{
    public class AcceptFishingProfessionScript : DialogScriptBase
    {
        public AcceptFishingProfessionScript(Dialog subject)
            : base(subject) { }

        public override void OnDisplaying(Aisling source)
        {
            var profCount = source.Enums.TryGetValue(out ProfessionCount profession);
            var hasFishing = source.Enums.TryGetValue(out Professions job);

            switch (Subject.Template.TemplateKey.ToLower())
            {
                case "kamel_initial":
                {
                    if (hasFishing)
                    {
                        if (Subject.GetOptionIndex("Fish Market").HasValue)
                        {
                            var s = Subject.GetOptionIndex("Fish Market")!.Value;
                            Subject.Options.RemoveAt(s);
                        }   
                    }
                }

                    break;

                case "kamel_acceptProfession":
                {
                    if (profession.Equals(ProfessionCount.Two))
                    {
                        Subject.Text = "Go, be on your way. The path to learning is endless. You cannot learn more.";
                        Subject.Type = MenuOrDialogType.Normal;
                        source.SendOrangeBarMessage("You already have two professions.");
                    } else if (profession.Equals(ProfessionCount.One) && job is not Professions.Fishing)
                    {
                        source.Enums.Set(Professions.Fishing);
                        source.Enums.Set((ProfessionCount.Two));
                        source.Titles.Add("Fisherman");
                        source.SendOrangeBarMessage("You've selected Fishing as your second profession!");
                    } else if (profession.Equals(ProfessionCount.None))
                    {
                        source.Enums.Set(Professions.Fishing);
                        source.Enums.Set(ProfessionCount.One);
                        source.Titles.Add("Fisherman");
                        source.SendOrangeBarMessage("You've selected Fishing as your first profession!");
                    }
                }

                    break;
            }
        }
    }
}