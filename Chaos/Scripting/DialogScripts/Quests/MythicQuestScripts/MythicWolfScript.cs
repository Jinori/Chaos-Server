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

public class MythicWolfScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicWolfScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasFrog = source.Trackers.Enums.TryGetValue(out MythicFrog frog);
        var hasWolf = source.Trackers.Enums.TryGetValue(out MythicWolf wolf);
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
                if (hasWolf && (wolf == MythicWolf.EnemyWolfAllied))
                {
                    Subject.Reply(source, "You have allied yourself with the frogs. You can never be one with the pack. Get lost.");

                    return;
                }

                if (hasMain && !hasWolf)
                {
                    Subject.Reply(source, "skip", "wolf_start1start");

                    return;
                }

                if (wolf == MythicWolf.LowerWolf)
                {
                    Subject.Reply(source, "skip", "wolf_lower2start");

                    return;
                }

                if (wolf == MythicWolf.LowerWolfComplete)
                {
                    Subject.Reply(source, "skip", "wolf_start3start");

                    return;
                }

                if (wolf == MythicWolf.HigherWolf)
                {
                    Subject.Reply(source, "skip", "wolf_higher2start");

                    return;
                }

                if (wolf == MythicWolf.HigherWolfComplete)
                {
                    Subject.Reply(source, "skip", "wolf_itemstart");

                    return;
                }

                if (wolf == MythicWolf.ItemWolf)
                {
                    Subject.Reply(source, "skip", "wolf_item2start");

                    return;
                }

                if (wolf == MythicWolf.ItemWolfComplete)
                {
                    Subject.Reply(source, "skip", "wolf_allystart");

                    return;
                }

                if (wolf == MythicWolf.AlliedWolf)
                {
                    Subject.Reply(source, "skip", "wolf_start5start");

                    return;
                }

                if (wolf == MythicWolf.BossWolfStarted)
                {
                    Subject.Reply(source, "skip", "wolf_boss2start");

                    return;
                }

                if (wolf == MythicWolf.BossWolfDefeated)
                    Subject.Reply(source, "Hello fearless one! Thank you for all you have done. I have no other task for you.");

                break;
            }

            case "wolf_lower":
            {
                Subject.Reply(source, "Stay safe, friend.");
                source.SendOrangeBarMessage("Kill 15 Slimy Frogs for the Wolf Pack Leader");
                source.Trackers.Enums.Set(MythicWolf.LowerWolf);

                return;
            }

            case "wolf_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicWolf1", out var wolflower) || (wolflower < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Slimy Frogs");

                    return;
                }

                source.Trackers.Enums.Set(MythicWolf.LowerWolfComplete);
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

                source.Trackers.Counters.Remove("MythicWolf1", out _);
                Subject.Reply(source, "Ha! You seem brave. Please come back when you can, I have another task for you.");

                break;
            }

            case "wolf_higher":
            {
                Subject.Reply(
                    source,
                    "You seem to have no fear. You are either very brave or just plain out stupid. Either way you have earned my respect. Please come back to me once the task is complete.");

                source.SendOrangeBarMessage("Kill 10 Poisonous and 10 Fierce Frogs for Wolf Pack Leader.");
                source.Trackers.Enums.Set(MythicWolf.HigherWolf);

                return;
            }

            case "wolf_higher2":
            {
                source.Trackers.Counters.TryGetValue("MythicWolf2", out var bluefrog);
                source.Trackers.Counters.TryGetValue("MythicWolf3", out var redfrog);

                if ((bluefrog < 10) || (redfrog < 10))
                {
                    Subject.Reply(source, "You haven't killed enough Poisonous and Fierce Frogs.");

                    return;
                }

                Subject.Reply(
                    source,
                    "Thanks for the help once again. You have earned our respect. I have another task for you when you are ready.");

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

                source.Trackers.Enums.Set(MythicWolf.HigherWolfComplete);
                source.Trackers.Counters.Remove("MythicWolf2", out _);
                source.Trackers.Counters.Remove("MythicWolf3", out _);

                break;
            }

            case "wolf_item":
            {
                Subject.Reply(source, "Hurry back, the other wolves are starting to get very hangry.");
                source.SendOrangeBarMessage("Collect 25 Frog Meat for Wolf Pack Leader");
                source.Trackers.Enums.Set(MythicWolf.ItemWolf);

                return;
            }

            case "wolf_item2":
            {
                if (!source.Inventory.RemoveQuantity("Frog Meat", 25))
                {
                    Subject.Reply(source, "You do not have enough Frog Meat.");

                    return;
                }

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

                source.Trackers.Enums.Set(MythicWolf.ItemWolfComplete);

                Subject.Reply(
                    source,
                    "Thank you. I was starting to get a headache from all the howling. This should be enough to feed us for awhile. Please come back to see me when you can. ");

                break;
            }

            case "wolf_ally":
            {
                if (hasFrog && (hasFrog == frog is MythicFrog.AlliedFrog or MythicFrog.BossFrogStarted or MythicFrog.BossFrogDefeated))
                {
                    Subject.Reply(source, "You are already allied with the Frogs? *The wolf pack leader begins to growl* Begone!");
                    source.Trackers.Enums.Set(MythicWolf.EnemyWolfAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicWolf.AlliedWolf);
                source.SendOrangeBarMessage("You are now allied with the Wolves!");
                Subject.Reply(source, "Wise choice friend! Welcome to the wolf pack!");

                break;
            }

            case "wolf_boss":
            {
                Subject.Reply(source, "That's the spirit! We will be here awaiting your return.");
                source.Trackers.Enums.Set(MythicWolf.BossWolfStarted);
                source.SendOrangeBarMessage("Kill Frogger three times.");
            }

                break;

            case "wolf_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicWolf", out var wolfboss1) || (wolfboss1 < 3))
                {
                    Subject.Reply(source, "Frogger's army is still there!");
                    source.SendOrangeBarMessage("You haven't completely defeated Frogger.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(source, "What a victory! Frogger and his army are no more. Thank you for all the help fearless one.");
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

                source.Trackers.Counters.Remove("MythicWolf", out _);
                source.Trackers.Enums.Set(MythicWolf.BossWolfDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss)
                    && (mythicboss >= 5)
                    && !source.Trackers.Enums.HasValue(MythicQuestMain.CompletedMythic))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}