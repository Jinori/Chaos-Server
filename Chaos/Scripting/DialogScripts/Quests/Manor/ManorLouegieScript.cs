using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class ManorLouegieScript : DialogScriptBase
{
    /// <inheritdoc />
    public ManorLouegieScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out ManorLouegieStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "louegie_initial":
            {
                switch (stage)
                {
                    case ManorLouegieStage.AcceptedQuest:
                        Subject.Reply(source, "Come back when you've dispatched of the ghosts.");

                        return;
                    case ManorLouegieStage.CompletedQuest:
                        Subject.Reply(source, "Thanks for sending those ghosts onward!");

                        if (!source.Legend.ContainsKey("manorPlumber"))
                            source.Legend.AddUnique(
                                new LegendMark(
                                    "Helped the Eingren Manor's Plumber",
                                    "manorPlumber",
                                    MarkIcon.Heart,
                                    MarkColor.Blue,
                                    1,
                                    GameTime.Now));

                        return;
                }
            }

                break;

            case "louegie_acceptedquest":
            {
                if (!hasStage || (stage == ManorLouegieStage.None))
                    source.Trackers.Enums.Set(ManorLouegieStage.AcceptedQuest);

                source.SendOrangeBarMessage("Go kill one hundred ghosts for Louegie on the second floor.");

                break;
            }
        }
    }
}