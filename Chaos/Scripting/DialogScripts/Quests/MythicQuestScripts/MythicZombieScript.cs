using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
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
                    Subject.Reply(
                        source,
                        "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.");

                    return;
                }

                if (hasMain && !hasZombie)
                {
                    Subject.Reply(source, "skip", "zombie_start1start");

                    return;
                }

                if (zombie == MythicZombie.Lower)
                {
                    Subject.Reply(source, "skip", "Zombie_lower2start");

                    return;
                }

                if (zombie == MythicZombie.LowerComplete)
                {
                    Subject.Reply(source, "skip", "Zombie_start3start");

                    return;
                }

                if (zombie == MythicZombie.Higher)
                {
                    Subject.Reply(source, "skip", "Zombie_higher2start");

                    return;
                }

                if (zombie == MythicZombie.HigherComplete)
                {
                    Subject.Reply(source, "skip", "zombie_itemstart");

                    return;
                }

                if (zombie == MythicZombie.Item)
                {
                    Subject.Reply(source, "skip", "zombie_item2start");

                    return;
                }

                if (zombie == MythicZombie.ItemComplete)
                {
                    Subject.Reply(source, "skip", "zombie_allystart");

                    return;
                }

                if (zombie == MythicZombie.Allied)
                {
                    Subject.Reply(source, "skip", "zombie_start5start");

                    return;
                }

                if (zombie == MythicZombie.BossStarted)
                {
                    Subject.Reply(source, "skip", "zombie_boss2start");

                    return;
                }

                if (zombie == MythicZombie.BossDefeated)
                    Subject.Reply(source, "Thank you again Aisling for your help. We are winning our fight.");

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
                    source.SendOrangeBarMessage("You received 10000000 experience!");
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
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicZombie.HigherComplete);
                source.Trackers.Counters.Remove("gargoyleservant", out _);
                source.Trackers.Counters.Remove("gargoyleguard", out _);

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
                    Subject.Reply(source, "Thank you again for your help.");
                    source.Trackers.Enums.Set(MythicZombie.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicZombie.Allied);
                source.SendOrangeBarMessage("You are now allied with the Zombies!");
                Subject.Reply(source, " ");

                break;
            }

            case "zombie_boss":
            {
                Subject.Reply(source, "Please return safely.");
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
                    source.SendOrangeBarMessage("You received 25000000 experience!");
                }

                source.Trackers.Counters.Remove("gargoylefiend", out _);
                source.Trackers.Enums.Set(MythicZombie.BossDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}