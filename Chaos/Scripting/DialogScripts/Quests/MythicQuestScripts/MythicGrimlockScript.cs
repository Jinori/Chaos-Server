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

public class MythicGrimlockScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicGrimlockScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasKobold = source.Trackers.Enums.TryGetValue(out MythicKobold kobold);
        var hasGrimlock = source.Trackers.Enums.TryGetValue(out MythicGrimlock grimlock);
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
            case "grimlock_initial":
            {
                if (hasGrimlock && (grimlock == MythicGrimlock.EnemyAllied))
                    Subject.Reply(
                        source,
                        "Your actions have brought shame upon yourself, and you will be remembered as a traitor to my people. May the spirits have mercy on your soul, for we will not. Now go, and do not let us catch you lurking in our tunnels again.");

                if (hasMain && !hasGrimlock)

                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_start1start");

                    return;
                }

                if (grimlock == MythicGrimlock.Lower)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_lower2start");

                    return;
                }

                if (grimlock == MythicGrimlock.LowerComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_start3start");

                    return;
                }

                if (grimlock == MythicGrimlock.Higher)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_higher2start");

                    return;
                }

                if (grimlock == MythicGrimlock.HigherComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_itemstart");

                    return;
                }

                if (grimlock == MythicGrimlock.Item)
                {
                    Subject.Reply(source, "Skip", "grimlock_item2start");

                    return;
                }

                if (grimlock == MythicGrimlock.ItemComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_allystart");

                    return;
                }

                if (grimlock == MythicGrimlock.Allied)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_start5start");

                    return;
                }

                if (grimlock == MythicGrimlock.BossStarted)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "grimlock_boss2start");

                    return;
                }

                if (grimlock == MythicGrimlock.BossDefeated)
                    Subject.Reply(
                        source,
                        "My people will never forget your heroic deeds, I must not ask you another favor for now. You have done more than enough for my people, thank you again.");

                break;
            }

            case "grimlock_lower":
            {
                Subject.Reply(
                    source,
                    "Please go kill 15 Kobold Workers, that'll stop them from damaging our land any further and supplying any more wars between us. Your willingness to fight on our behalf is appreciated.");

                source.SendOrangeBarMessage("Kill 15 Kobold Workers.");
                source.Trackers.Enums.Set(MythicGrimlock.Lower);

                return;
            }

            case "grimlock_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("koboldworker", out var koboldworker) || (koboldworker < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Kobold Workers");

                    return;
                }

                source.Trackers.Enums.Set(MythicGrimlock.LowerComplete);
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

                source.Trackers.Counters.Remove("koboldworker", out _);

                Subject.Reply(
                    source,
                    "While the elimination of any living beings is never something to be taken lightly, the Kobold workers posed a genuine threat to our land. Your actions have helped ensure the safety and security of our lands, and for that, we are grateful.",
                    "grimlock_initial");

                break;
            }

            case "grimlock_higher":
            {
                Subject.Reply(source, "Thank you Aisling, I take it I will hear from you soon.");
                source.SendOrangeBarMessage("Kill 10 Kobold Soldiers and 10 Kobold Warriors.");
                source.Trackers.Enums.Set(MythicGrimlock.Higher);

                return;
            }

            case "grimlock_higher2":
            {
                source.Trackers.Counters.TryGetValue("koboldworker", out var koboldsoldier);
                source.Trackers.Counters.TryGetValue("koboldsoldier", out var koboldwarrior);

                if ((koboldsoldier < 10) || (koboldwarrior < 10))
                {
                    Subject.Reply(source, "You haven't killed enough Kobold Soldiers and Warriors.");

                    return;
                }

                Subject.Reply(
                    source,
                    "With the loss of these fighters, the Kobolds will surely be weakened, and their chances of winning this conflict have been significantly reduced. The Grimlocks are a proud people, and we do not forget those who fight alongside us. You have earned our respect and gratitude, and we will remember your deeds for generations to come.",
                    "grimlock_initial");

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

                source.Trackers.Enums.Set(MythicGrimlock.HigherComplete);
                source.Trackers.Counters.Remove("koboldworker", out _);
                source.Trackers.Counters.Remove("koboldsoldier", out _);

                break;
            }

            case "grimlock_item":
            {
                Subject.Reply(
                    source,
                    "I am counting on you to bring back those Kobold tails. Your success will send a strong message to our enemies that we will not tolerate their attacks on our land. We also really need those tails.");

                source.SendOrangeBarMessage("Collect 25 Kobold Tails for the Grimlock Queen");
                source.Trackers.Enums.Set(MythicGrimlock.Item);

                return;
            }

            case "grimlock_item2":
            {
                if (!source.Inventory.RemoveQuantity("Kobold Tail", 25))
                {
                    Subject.Reply(
                        source,
                        "This is not enough! My people will blow through that many very quickly, please try harder. We are counting on you.");

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

                source.Trackers.Enums.Set(MythicGrimlock.ItemComplete);

                Subject.Reply(
                    source,
                    "This is so much! We will be satisfied for weeks! Thank you so much. We use the tails to heal our wounds and they really calm my people down.",
                    "grimlock_initial");

                break;
            }

            case "grimlock_ally":
            {
                if (hasKobold
                    && (hasKobold == kobold is MythicKobold.Allied or MythicKobold.BossStarted or MythicKobold.BossDefeated))
                {
                    Subject.Reply(
                        source,
                        "I am outraged to discover that you have allied yourself with our sworn enemies, the Kobolds! How dare you betray us in such a manner! I trusted you to be an honorable and loyal ally, but you have proven yourself to be nothing more than a treacherous liar. I have no choice but to order you to leave our territory at once. You are no longer welcome among our people, and we will not hesitate to defend ourselves against you should you attempt to harm us in any way.");

                    source.Trackers.Enums.Set(MythicGrimlock.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicGrimlock.Allied);
                source.SendOrangeBarMessage("You are now allied with the Grimlocks!");

                Subject.Reply(
                    source,
                    "I must say that I am impressed with your commitment to our cause. Your dedication to our alliance fills me with pride, and I have no doubt that you will be a valuable addition to our underground family.",
                    "grimlock_initial");

                break;
            }

            case "grimlock_boss":
            {
                Subject.Reply(
                    source,
                    "Oh, Aisling. Remember you must defeat him 3 times! Thank you again for handling this, I don't know what we would do without you.");

                source.Trackers.Enums.Set(MythicGrimlock.BossStarted);
                source.SendOrangeBarMessage("Kill Shank three times.");
            }

                break;

            case "grimlock_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Shank", out var shank) || (shank < 3))
                {
                    Subject.Reply(
                        source,
                        "Shank is still out there? This is not good. Please take care of him as soon as possible, every second he is on the move, my people are in danger.");

                    source.SendOrangeBarMessage("You haven't completely defeated Shank.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "Shank's Defeated!? That's such great news. I will pass along the word to my scouts, they will rest easy. This should really help us in our conflict, without Shank, the kobold's will fall. Thank you Adventurer, the Grimlock people are in your debt. Come back anytime.");

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

                source.Trackers.Counters.Remove("shank", out _);
                source.Trackers.Enums.Set(MythicGrimlock.BossDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}