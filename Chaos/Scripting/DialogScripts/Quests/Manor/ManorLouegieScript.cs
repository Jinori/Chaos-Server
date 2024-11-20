using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.Manor;

public class ManorLouegieScript : DialogScriptBase
{
    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    /// <inheritdoc />
    public ManorLouegieScript(Dialog subject)
        : base(subject)
    {
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    public override void OnDisplaying(Aisling source)
    {
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentypercent = Convert.ToInt32(0.20 * tnl);

        if (twentypercent > 320000)
        {
            twentypercent = 320000;
        }
        
        var hasStage = source.Trackers.Enums.TryGetValue(out ManorLouegieStage stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "louegie_initial":
            {
                if (source.UserStatSheet.Level < 41)
                {
                    Subject.Reply(source, "You're a little weak to be handling that second floor for me. Come back when you're stronger.");
                    return;
                }

                if (source.Trackers.TimedEvents.HasActiveEvent("Louegie2ndFloor", out _))
                {
                    Subject.Reply(source, $"Those banshees are gone for now, thank you again {source.Name}.");
                    return;
                }
                
                switch (stage)
                {
                    case ManorLouegieStage.AcceptedQuestBanshee:
                        Subject.Reply(source, "Come back when you've dispatched of the ghosts.");

                        return;
                    case ManorLouegieStage.CompletedQuest:
                    {
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
                        
                        source.Trackers.Enums.Set(ManorLouegieStage.None);
                        ExperienceDistributionScript.GiveExp(source, twentypercent);
                        source.TryGiveGamePoints(10);
                        source.Trackers.TimedEvents.AddEvent("Louegie2ndFloor", TimeSpan.FromHours(22), true);
                        return;
                    }
                }
            }

                break;

            case "louegie_acceptedquest":
            {
                if (!hasStage || (stage == ManorLouegieStage.None))
                    source.Trackers.Enums.Set(ManorLouegieStage.AcceptedQuestBanshee);

                source.SendOrangeBarMessage("Go kill one hundred ghosts for Louegie on the second floor.");

                break;
            }
        }
    }
}