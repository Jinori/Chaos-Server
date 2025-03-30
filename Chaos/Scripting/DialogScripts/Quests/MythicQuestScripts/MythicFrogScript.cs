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

public class MythicFrogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicFrogScript(Dialog subject, IItemFactory itemFactory)
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
            case "frog_initial":
            {
                if (hasFrog && (frog == MythicFrog.EnemyFrogAllied))
                {
                    Subject.Reply(
                        source,
                        "You have allied yourself with our enemies. You have no honor and have betrayed our trust. Please leave our army at once.");

                    return;
                }

                if (hasMain && !hasFrog)
                {
                    Subject.Reply(source, "Skip", "frog_start1start");

                    return;
                }

                if (frog == MythicFrog.LowerFrog)
                {
                    Subject.Reply(source, "Skip", "frog_lower2start");

                    return;
                }

                if (frog == MythicFrog.LowerFrogComplete)
                {
                    Subject.Reply(source, "Skip", "frog_start3start");

                    return;
                }

                if (frog == MythicFrog.HigherFrog)
                {
                    Subject.Reply(source, "Skip", "frog_higher2start");

                    return;
                }

                if (frog == MythicFrog.HigherFrogComplete)
                {
                    Subject.Reply(source, "Skip", "frog_itemstart");

                    return;
                }

                if (frog == MythicFrog.ItemFrog)
                {
                    Subject.Reply(source, "Skip", "frog_item2start");

                    return;
                }

                if (frog == MythicFrog.ItemFrogComplete)
                {
                    Subject.Reply(source, "Skip", "Frog_allystart");

                    return;
                }

                if (frog == MythicFrog.AlliedFrog)
                {
                    Subject.Reply(source, "Skip", "frog_start5start");

                    return;
                }

                if (frog == MythicFrog.BossFrogStarted)
                {
                    Subject.Reply(source, "Skip", "frog_boss2start");

                    return;
                }

                if (frog == MythicFrog.BossFrogDefeated)
                    Subject.Reply(source, "Thank you again Aisling for your help. You will always be part of our army.");

                break;
            }

            case "frog_lower":
            {
                Subject.Reply(source, "Thank you, adventurer. We appreciate your assistance. Good luck on your quest.");
                source.SendOrangeBarMessage("Kill 15 Peculiar Wolves for the Frog King.");
                source.Trackers.Enums.Set(MythicFrog.LowerFrog);

                return;
            }

            case "frog_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicFrog1", out var mythicwolf) || (mythicwolf < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Peculiar Wolves.");

                    return;
                }

                source.Trackers.Enums.Set(MythicFrog.LowerFrogComplete);
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

                source.Trackers.Counters.Remove("MythicFrog1", out _);

                Subject.Reply(
                    source,
                    "Excellent work, adventurer! You have proven yourself to our army. Now, it is time to deal with the stronger wolves that have been causing us even more trouble.",
                    "frog_initial");

                break;
            }

            case "frog_higher":
            {
                Subject.Reply(
                    source,
                    "Remember to slay 10 of each of the stronger wolves, the Menacing Wolf and the Ominous Wolf. They are more powerful and cunning than the ones you faced before, so be careful.");

                source.SendOrangeBarMessage("Kill 10 Menacing Wolves and 10 Ominous Wolves.");
                source.Trackers.Enums.Set(MythicFrog.HigherFrog);

                return;
            }

            case "frog_higher2":
            {
                source.Trackers.Counters.TryGetValue("MythicFrog2", out var whitewolf);
                source.Trackers.Counters.TryGetValue("MythicFrog3", out var beardedwolf);

                if ((whitewolf < 10) || (beardedwolf < 10))
                {
                    Subject.Reply(source, "You haven't killed enough Menacing Wolves and Ominous Wolves.");

                    return;
                }

                Subject.Reply(source, "Croak! You've really proven you could be a powerful ally.", "frog_initial");

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

                source.Trackers.Enums.Set(MythicFrog.HigherFrogComplete);
                source.Trackers.Counters.Remove("MythicFrog2", out _);
                source.Trackers.Counters.Remove("MythicFrog3", out _);

                var option = new DialogOption
                {
                    DialogKey = "frog_item",
                    OptionText = "I can get that."
                };

                var option1 = new DialogOption
                {
                    DialogKey = "frog_no",
                    OptionText = "Not a chance, good luck."
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Add(option);

                if (!Subject.HasOption(option1.OptionText))
                    Subject.Options.Add(option1);

                break;
            }

            case "frog_item":
            {
                Subject.Reply(
                    source,
                    "Please be careful, we don't want you to ribbit the wrong way into danger. They may be more aggressive now that their pack has been weakened. Return to us once you have collected the wolf skins.");

                source.SendOrangeBarMessage("Collect 25 Wolf Skin for the Frog King");
                source.Trackers.Enums.Set(MythicFrog.ItemFrog);

                return;
            }

            case "frog_item2":
            {
                if (!source.Inventory.RemoveQuantity("Wolf Skin", 25))
                {
                    Subject.Reply(
                        source,
                        "We need 25 wolf skins to make some protective clothing for our frog army. Croak! We understand that this may be a difficult task, but we believe that you are more than capable of accomplishing it.");

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

                source.Trackers.Enums.Set(MythicFrog.ItemFrogComplete);

                Subject.Reply(
                    source,
                    "Excellent work, adventurer. You continue to prove yourself as a valuable friend to our army. These skins will be very useful for our army's survival.",
                    "frog_initial");

                break;
            }

            case "frog_ally":
            {
                if (hasWolf && (hasWolf == wolf is MythicWolf.AlliedWolf or MythicWolf.BossWolfStarted or MythicWolf.BossWolfDefeated))
                {
                    Subject.Reply(source, "Ribbit! It seems you already allied with the Wolves! Go away!");
                    source.Trackers.Enums.Set(MythicFrog.EnemyFrogAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicFrog.AlliedFrog);
                source.SendOrangeBarMessage("You are now allied with the Frogs!");

                Subject.Reply(
                    source,
                    $"Remember, {source.Name}, no matter where your journeys take you, you will always have a home within our army!",
                    "frog_initial");

                break;
            }

            case "frog_boss":
            {
                Subject.Reply(
                    source,
                    "Please be careful, Nymeria is a fierce beast and is not to be taken lightly. We need your help to finally be rid of these beast . We'll be waiting for your return, may the frog be with you!");

                Subject.NextDialogKey = "Close";
                source.Trackers.Enums.Set(MythicFrog.BossFrogStarted);
                source.SendOrangeBarMessage("Kill Nymeria at least three times.");
            }

                break;

            case "frog_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicFrog", out var frogboss1) || (frogboss1 < 3))
                {
                    Subject.Reply(source, "Please rest and recover your strength. We're all cheering for you!");

                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't killed Nymeria enough times.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "Your skill in battle have truly croaked over our expectations. You have proven yourself to our army, and we will forever be grateful for you. Croak!",
                    "frog_initial");

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

                source.Trackers.Counters.Remove("MythicFrog", out _);
                source.Trackers.Enums.Set(MythicFrog.BossFrogDefeated);
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