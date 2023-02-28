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

public class MythicZombieScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicZombieScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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
            case "zombie_initial":
            {
                if (hasZombie && (zombie == MythicZombie.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasZombie)

                {
                    Subject.Text = "Ears to you, traveler. I am the leader of this warren of bunnies, and I carrot thank you enough for coming to our aid. The neigh-sayers may think we're just cute and fluffy, but we're tougher than we look.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "zombie_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (zombie == MythicZombie.Lower)
                {
                    Subject.Text = "Well, well, well, look who's back! It's our favorite rabbit-loving adventurer! Have you come to tell us that you've completed the task we gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_lower2",
                        OptionText = "Yes Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (zombie == MythicZombie.LowerComplete)
                {
                    Subject.Text = "Warren Wanderer, we are in need of your assistance once again. It seems that another group of horses has invaded our territory and is causing chaos and destruction. We need your help to remove them from our fields, just as you did with the previous group.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "zombie_start3",
                        OptionText = "No problem Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.Higher)
                {
                    Subject.Text = " ";

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_higher2",
                        OptionText = "Yeah, it is done."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "zombie_item",
                        OptionText = "I can get that."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (zombie == MythicZombie.Item)
                {
                    Subject.Text = " ";

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_item2",
                        OptionText = "I have them here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.ItemComplete)
                {
                    Subject.Text = "\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_ally",
                        OptionText = "Ally with Bunny"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "zombie_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (zombie == MythicZombie.Allied)
                {
                    Subject.Text =
                        " ";
                    var option = new DialogOption
                    {
                        DialogKey = "zombie_start5",
                        OptionText = "Anything for you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (zombie == MythicZombie.BossStarted)
                {
                    Subject.Text =
                        " ";

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (zombie == MythicZombie.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "zombie_lower":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Kill 15 Mythic Dunans for the Superior Zombie");
                source.Enums.Set(MythicZombie.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "zombie_lower2":
            {

                if (!source.Counters.TryGetValue("mythicdunan", out var zombielower) || (zombielower < 15))
                {
                    Subject.Text = "You haven't killed enough Mythic Dunans";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicZombie.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("mythicdunan", out _);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "zombie_initial";

                break;
            }

            case "zombie_higher":
            {
                Subject.Text = "Great, clear 10 Gargoyle Servants and 10 Guards";
                source.SendOrangeBarMessage("Kill 10 Gargoyle Servants and 10 Guards");
                source.Enums.Set(MythicZombie.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "zombie_higher2":
            {
                source.Counters.TryGetValue("gargoyleservant", out var gargoyleservant);
                source.Counters.TryGetValue("gargoyleguard", out var gargoyleguard);

                if ((gargoyleservant < 10) && (gargoyleguard < 10))
                {
                    Subject.Text = "You haven't killed enough Gargoyle Servants and Guards.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = " ";
                Subject.NextDialogKey = "zombie_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicZombie.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("gargoyleservant", out _);
                source.Counters.Remove("gargoyleguard", out _);

                var option = new DialogOption
                {
                    DialogKey = "zombie_item",
                    OptionText = "I can get that."
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Add(option);

                break;
            }

            case "zombie_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 Dark Flames for Superior Zombie");
                source.Enums.Set(MythicZombie.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "zombie_item2":
            {

                if (!source.Inventory.RemoveQuantity("Dark Flame", 25))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicZombie.ItemComplete);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "zombie_initial";

                break;
            }

            case "zombie_ally":
            {
                if (hasFrog
                    && (hasFrog == frog is MythicFrog.Allied or MythicFrog.BossStarted or MythicFrog.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    source.Enums.Set(MythicZombie.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicZombie.Allied);
                source.SendOrangeBarMessage("You are now allied with the Zombies!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "zombie_initial";

                break;

            }

            case "zombie_boss":
            {
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicZombie.BossStarted);
                source.SendOrangeBarMessage("Kill Gargoyle Fiend three times.");
            }

                break;

            case "zombie_boss2":
            {
                if (!source.Counters.TryGetValue("gargoylefiend", out var zombieboss1) || (zombieboss1 < 3))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Gargoyle Fiend.");

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
                source.Counters.Remove("gargoylefiend", out _);
                source.Enums.Set(MythicZombie.BossDefeated);
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