using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MythicQuestScripts;

public class MythicWolfScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicWolfScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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
        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 20
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "wolf_initial":
            {
                if (hasWolf && (wolf == MythicWolf.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.";
                    Subject.NextDialogKey = "Close";
                }
                
                if ((main == MythicQuestMain.MythicStarted) && !hasWolf)

                {
                    Subject.Text = "Ears to you, traveler. I am the leader of this warren of bunnies, and I carrot thank you enough for coming to our aid. The neigh-sayers may think we're just cute and fluffy, but we're tougher than we look.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (wolf == MythicWolf.Lower)
                {
                    Subject.Text = "Well, well, well, look who's back! It's our favorite rabbit-loving adventurer! Have you come to tell us that you've completed the task we gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_lower2",
                        OptionText = "Yes Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (wolf == MythicWolf.LowerComplete)
                {
                    Subject.Text = "Warren Wanderer, we are in need of your assistance once again. It seems that another group of horses has invaded our territory and is causing chaos and destruction. We need your help to remove them from our fields, just as you did with the previous group.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_start3",
                        OptionText = "No problem Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (wolf == MythicWolf.Higher)
                {
                    Subject.Text = " ";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_higher2",
                        OptionText = "Yeah, it is done."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (wolf == MythicWolf.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_item",
                        OptionText = "I can get that."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (wolf == MythicWolf.Item)
                {
                    Subject.Text = " ";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_item2",
                        OptionText = "I have them here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (wolf == MythicWolf.ItemComplete)
                {
                    Subject.Text = "\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_ally",
                        OptionText = "Ally with Bunny"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "wolf_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (wolf == MythicWolf.Allied)
                {
                    Subject.Text =
                        " ";
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_start5",
                        OptionText = "Anything for you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (wolf == MythicWolf.BossStarted)
                {
                    Subject.Text =
                        " ";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (wolf == MythicWolf.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "wolf_lower":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Kill 15 Mythic Frogs for the Wolf Pack Leader");
                source.Enums.Set(MythicWolf.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "wolf_lower2":
            {

                if (!source.Counters.TryGetValue("mythicfrog", out var wolflower) || (wolflower < 15))
                {
                    Subject.Text = "You haven't killed enough Mythic Frogs";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicWolf.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("mythicfrog", out _);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "wolf_initial";

                break;
            }

            case "wolf_higher":
            {
                Subject.Text = "Great, clear 10 Blue Frogs and 10 Red Frogs.";
                source.SendOrangeBarMessage("Kill 10 Blue and 10 Red Frogs for Wolf Pack Leader.");
                source.Enums.Set(MythicWolf.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "wolf_higher2":
            {
                source.Counters.TryGetValue("bluefrog", out var bluefrog);
                source.Counters.TryGetValue("redfrog", out var redfrog);

                if ((bluefrog < 10) && (redfrog < 10))
                {
                    Subject.Text = "You haven't killed enough Blue and Red Frogs.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = " ";
                Subject.NextDialogKey = "wolf_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicWolf.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("redfrog", out _);
                source.Counters.Remove("bluefrog", out _);

                var option = new DialogOption
                {
                    DialogKey = "wolf_item",
                    OptionText = "I can get that."
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Add(option);

                break;
            }

            case "wolf_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 Frog Meat for Wolf Pack Leader");
                source.Enums.Set(MythicWolf.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "wolf_item2":
            {

                if (!source.Inventory.RemoveQuantity("Frog Meat", 25))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicWolf.ItemComplete);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "wolf_initial";

                break;
            }

            case "wolf_ally":
            {
                if (hasFrog
                    && (hasFrog == frog is MythicFrog.Allied or MythicFrog.BossStarted or MythicFrog.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    source.Enums.Set(MythicWolf.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicWolf.Allied);
                source.SendOrangeBarMessage("You are now allied with the Wolves!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "wolf_initial";

                break;

            }

            case "wolf_boss":
            {
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicWolf.BossStarted);
                source.SendOrangeBarMessage("Kill Frogger three times.");
            }

                break;

            case "wolf_boss2":
            {
                if (!source.Counters.TryGetValue("Frogger", out var wolfboss1) || (wolfboss1 < 3))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Frogger.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };
                
                Subject.Text = " ";
                source.Animate(ani2, source.Id);
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Counters.Remove("frogger", out _);
                source.Enums.Set(MythicWolf.BossDefeated);
                source.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                {
                    source.Enums.Set(MythicQuestMain.CompletedAll);
                }
            }

                break;
        }
    }
}