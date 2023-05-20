using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Professions;

public class AcceptFishingProfessionScript : DialogScriptBase
{
    public AcceptFishingProfessionScript(Dialog subject)
        : base(subject) { }

 public override void OnDisplaying(Aisling source)
{
    var profCount = source.Trackers.Enums.TryGetValue(out ProfessionCount profession);
    var hasFishing = source.Trackers.Enums.TryGetValue(out Definitions.Professions job);
    switch (Subject.Template.TemplateKey.ToLower())
    {
        case "kamel_initial":
        {
            if (hasFishing)
            {
                var s = Subject.GetOptionIndex("Fish Market")!.Value;
                Subject.Options.RemoveAt(s);
            }
        }
        break;
        case "kamel_acceptprofession":
        {
            switch (profession)
            {
                case ProfessionCount.Two:
                    Subject.Reply(source, "Go, be on your way. The path to learning is endless. You cannot learn more.");
                    source.SendOrangeBarMessage("You already have two professions.");
                    break;
                case ProfessionCount.One when job is not Definitions.Professions.Fishing:
                    source.Trackers.Enums.Set(Definitions.Professions.Fishing);
                    source.Trackers.Enums.Set(ProfessionCount.Two);
                    source.Titles.Add("Fisherman");
                    source.SendOrangeBarMessage("You've selected Fishing as your second profession!");
                    break;
                default:
                {
                    if (profession is ProfessionCount.None || !hasFishing)
                    {
                        source.Trackers.Enums.Set(Definitions.Professions.Fishing);
                        source.Trackers.Enums.Set(ProfessionCount.One);
                        source.Titles.Add("Fisherman");
                        source.SendOrangeBarMessage("You've selected Fishing as your first profession!");
                    }
                    break;
                }
            }
        }
        break;
    }
}
}