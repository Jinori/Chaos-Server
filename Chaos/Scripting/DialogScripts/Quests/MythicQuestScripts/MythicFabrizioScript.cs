using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Quests.MythicQuestScripts;

public class MythicFabrizioScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicFabrizioScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Enums.TryGetValue(out MythicQuestMain main);
        var hasBunny = source.Enums.TryGetValue(out MythicBunny bunny);
        var hasHorse = source.Enums.TryGetValue(out MythicHorse horse);
        var hasGargoyle = source.Enums.TryGetValue(out MythicGargoyle gargoyle);
        var hasZombie = source.Enums.TryGetValue(out MythicZombie zombie);
        var hasFrog = source.Enums.TryGetValue(out MythicFrog frog);
        var hasWolf = source.Enums.TryGetValue(out MythicWolf wolf);
        var hasMantis = source.Enums.TryGetValue(out MythicMantis mantis);
        var hasBee = source.Enums.TryGetValue(out MythicBee bee);
        var hasKobold = source.Enums.TryGetValue(out MythicKobold kobold);
        var hasGrimlock = source.Enums.TryGetValue(out MythicGrimlock grimlock);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = MathEx.GetPercentOf<int>(tnl, 20);
        var fiftyPercent = MathEx.GetPercentOf<int>(tnl, 50);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "fabrizio_initial":
            {
                if (source.UserStatSheet.Level < 9)
                {
                    Subject.Text = "Greetings Young Aisling, you are too weak to be here. Please get out of here.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                if (main == MythicQuestMain.CompletedMythic)
                {
                    Subject.Text = "Thank you for all your hard work Aisling. These lands are forever in your debt.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                if (!hasMain)
                {
                    var option = new DialogOption
                    {
                        DialogKey = "fabrizio_start1",
                        OptionText = "What have you tried so far?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Insert(0, option);

                    return;
                }

                if (main != MythicQuestMain.CompletedAll)
                {
                    Subject.Text =
                        "Welcome back, adventurer. I sense that you haven't made enough alliances yet.";
                    source.SendOrangeBarMessage("Assist five leaders to the fullest.");
                    Subject.Type = MenuOrDialogType.Normal;
                    
                    return;
                }

                if (source.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                {
                    Subject.Text =
                        "You really did it! You've made alliances with some of the leaders. They have been feuding forever! Whichever alliances you made will be victorious. The others will perish and these lands shall be peaceful once again. Unfortunately, it was the only way.";
                    Subject.Type = MenuOrDialogType.Normal;
                    source.Enums.Set(MythicQuestMain.CompletedMythic);
                    ExperienceDistributionScript.GiveExp(source, source.UserStatSheet.Level <= 98 ? tnl : 50000000);

                    source.Legend.AddOrAccumulate(
                        new LegendMark(
                            "Ended the Feud of Mythic",
                            "MythicComplete",
                            MarkIcon.Yay,
                            MarkColor.Yellow,
                            1,
                            GameTime.Now));

                    return;
                }

                break;
            }

            case "mythicquest_initial":
            {
                source.Enums.Set(MythicQuestMain.MythicStarted);

                Subject.Text =
                    "Farewell for now, adventurer. Remember, our land's fate rests in your hands. Please return to me once you've made alliances and finished helping the leaders.. The more allies we have, the stronger our position will be. I trust in your abilities and look forward to hearing about your progress. Good luck.";

                Subject.Type = MenuOrDialogType.Normal;
                source.SendOrangeBarMessage("Talk to any of the leaders.");

                break;
            }
        }
    }
}