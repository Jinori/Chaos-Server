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

namespace Chaos.Scripting.DialogScripts.Quests.MythicQuestScripts;

public class MythicZombieScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicZombieScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasZombie = source.Trackers.Enums.TryGetValue(out MythicZombie zombie);
        var hasFrog = source.Trackers.Enums.TryGetValue(out MythicFrog frog);
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
                    

                    Subject.Reply(source,
                        "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.");

                   
                }

                if (hasMain && !hasZombie)

                {
                    Subject.Reply(source,
                        "Grrrrreetings, aisling. We have a task that needs your attention. Those Gargoyles have been mocking us for tooooo loooong. They think we're just braaaaaaaainless corpses who wander around aimlessly. We need to show them that we're mmmoooooore than that. Uuuurgh!");

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
                    Subject.Reply(source,
                        "welcome baaaaacck aisling. Have you taken care of those Dunans yet? Uuuuuurgh! They need to be punished for looking downnnnnn on us.");

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_lower2",
                        OptionText = "Yes, 10 less dunans to worry about."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.LowerComplete)
                {
                    Subject.Reply(source,
                        "Uuuurgh! You have done well. We are immmmmpressed with your skills. However, those Gargoyles still won't leeeeave us alone. They keep yapping about how we're nothing but braaaaaaainless zombies. It's time we put an end to their mooooockery. Uuuurgh! We need you to slay 10 of their servants and 10 of their guards. Uuuurgh!");

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_start3",
                        OptionText = "No problem. I wont let you down."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.Higher)
                {
                    Subject.Reply(source, "Have you taken care of the seeerrrrvants and guards yet?");

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_higher2",
                        OptionText = "Yes, they won't be making fun of you anymore."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.HigherComplete)
                {
                    Subject.Reply(source, "Uuuurgh. The Gargoyles like to carry around Dark Flames with themmmmm, We need you to collect sommme for us. Maybe if we have these flammmmmes, they will think we are one of themmmmmm and leave us alone.");

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_item",
                        OptionText = "Uhh, sure..."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (zombie == MythicZombie.Item)
                {
                    Subject.Reply(source, "Weeeeerrrrre you able to colleeect the Dark Flames? Uuuurgh!");

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_item2",
                        OptionText = "I have them here... I don't think this will work the way you think it will..."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (zombie == MythicZombie.ItemComplete)
                {
                    Subject.Reply(source, "Thank you for all you have done. We could use you braaaaaaain to keep them from mocking us. Would you like to foorrrrrm an alliance to end their mockery? \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))");

                    var option = new DialogOption
                    {
                        DialogKey = "zombie_ally",
                        OptionText = "Ally with Zombies."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "zombie_no",
                        OptionText = "No, I'm good. I think I'm starting to see why they mock y'all all the time..."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (zombie == MythicZombie.Allied)
                {
                    Subject.Reply(source,
                        " ");

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
                    Subject.Reply(source,
                        " ");

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

                    Subject.Reply(source, "Thank you again Aisling for your help. We are winning our fight.");
                }

                break;
            }

            case "zombie_lower":
            {
                Subject.Reply(source, "Get moooooving. Now where are my braaaaaaainss at... Uuuuurgh.");
                source.SendOrangeBarMessage("Kill 15 Mythic Dunans for the Superior Zombie");
                source.Trackers.Enums.Set(MythicZombie.Lower);
                

                return;
            }

            case "zombie_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("mythicdunan", out var zombielower) || (zombielower < 15))
                {
                    Subject.Reply(source, "Uuuurgh! I can still hear them mmooooocking us!");
                    

                    return;
                }

                source.Trackers.Enums.Set(MythicZombie.LowerComplete);
                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Counters.Remove("mythicdunan", out _);
                Subject.Reply(source, " ");
                
                

                break;
            }

            case "zombie_higher":
            {
                Subject.Reply(source, "Great, clear 10 Gargoyle Servants and 10 Guards");
                source.SendOrangeBarMessage("Kill 10 Gargoyle Servants and 10 Guards");
                source.Trackers.Enums.Set(MythicZombie.Higher);
                

                return;
            }

            case "zombie_higher2":
            {
                source.Trackers.Counters.TryGetValue("gargoyleservant", out var gargoyleservant);
                source.Trackers.Counters.TryGetValue("gargoyleguard", out var gargoyleguard);

                if ((gargoyleservant < 10) || (gargoyleguard < 10))
                {
                    Subject.Reply(source, "I can still hear themmmmmm.");
                    

                    return;
                }

                Subject.Reply(source, "Gooooood job. Soon we will be able to roooooooammm in peace. Uuuurgh!");
                
                
                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicZombie.HigherComplete);
                source.Trackers.Counters.Remove("gargoyleservant", out _);
                source.Trackers.Counters.Remove("gargoyleguard", out _);

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
                Subject.Reply(source, " ");
                source.SendOrangeBarMessage("Collect 25 Dark Flames for Superior Zombie");
                source.Trackers.Enums.Set(MythicZombie.Item);
                

                return;
            }

            case "zombie_item2":
            {

                if (!source.Inventory.RemoveQuantity("Dark Flame", 25))
                {
                    Subject.Reply(source, " ");
                    

                    return;
                }

                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicZombie.ItemComplete);
                Subject.Reply(source, " ");
                
                

                break;
            }

            case "zombie_ally":
            {
                if (hasFrog
                    && (hasFrog == frog is MythicFrog.Allied or MythicFrog.BossStarted or MythicFrog.BossDefeated))
                {
                    
                    Subject.Reply(source, " ");
                    source.Trackers.Enums.Set(MythicZombie.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicZombie.Allied);
                source.SendOrangeBarMessage("You are now allied with the Zombies!");
                Subject.Reply(source, $" ");
                
                

                break;

            }

            case "zombie_boss":
            {
                Subject.Reply(source, " ");
                
               
                source.Trackers.Enums.Set(MythicZombie.BossStarted);
                source.SendOrangeBarMessage("Kill Gargoyle Fiend three times.");
            }

                break;

            case "zombie_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("gargoylefiend", out var zombieboss1) || (zombieboss1 < 3))
                {
                    Subject.Reply(source, " ");
                    
                   
                    source.SendOrangeBarMessage("You haven't completely defeated Gargoyle Fiend.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(source, " ");
                source.Animate(ani2, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage($"You received 25000000 experience!");
                }

                source.Trackers.Counters.Remove("gargoylefiend", out _);
                source.Trackers.Enums.Set(MythicZombie.BossDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                {
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
                }
            }

                break;
        }
    }
}