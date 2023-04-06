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

public class MythicMantisScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicMantisScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasMantis = source.Trackers.Enums.TryGetValue(out MythicMantis mantis);
        var hasBee = source.Trackers.Enums.TryGetValue(out MythicBee bee);
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
            case "mantis_initial":
            {
                if (hasMantis && (mantis == MythicMantis.EnemyAllied))
                {
                    
                    Subject.Reply(source, "Oh lucky us. You're allied with the bees. We'll devour you like the rest of them.");
                    
                }

                if (hasMain && !hasMantis)

                {
                    Subject.Reply(source, "skip", "mantis_start1start");
                    return;
                }

                if (mantis == MythicMantis.Lower)
                {
                    Subject.Reply(source, "skip", "mantis_lower2start");
                    return;
                }

                if (mantis == MythicMantis.LowerComplete)
                {
                    Subject.Reply(source, "skip", "mantis_start3start");
                    return;
                }

                if (mantis == MythicMantis.Higher)
                {
                    Subject.Reply(source, "skip", "mantis_higher2start");
                    return;

                }

                if (mantis == MythicMantis.HigherComplete)
                {
                    Subject.Reply(source, "skip", "mantis_itemstart");
                    return;
                }

                if (mantis == MythicMantis.Item)
                {
                    Subject.Reply(source, "skip", "mantis_item2start");
                    return;
                }

                if (mantis == MythicMantis.ItemComplete)
                {
                    Subject.Reply(source, "skip", "mantis_allystart");
                    return;
                }

                if (mantis == MythicMantis.Allied)
                {
                    Subject.Reply(source,
                        "skip", "mantis_start5start");
                    return;
                }

                if (mantis == MythicMantis.BossStarted)
                {
                    Subject.Reply(source, "skip", "mantis_boss2start");
                    return;
                }

                if (mantis == MythicMantis.BossDefeated)
                {
                    Subject.Reply(source, "My Colony is eating well, nothing to worry about. Thank you again Aisling for taking care of that wasp. We have no troubles these days.");
                }

                break;
            }

            case "mantis_lower":
            {
                Subject.Reply(source, "Good, good. I will see you when you return. Remember, go kill 15 Mythic Bees. Don't leave any alive.");
                source.SendOrangeBarMessage("Kill 15 Mythic Bees for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Lower);
                return;
            }

            case "mantis_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("mythicbee", out var mantislower) || (mantislower < 15))
                {
                    Subject.Reply(source, "Disappointing, loyalty is everything and if you aren't willing, don't come back.");
                    return;
                }

                source.Trackers.Enums.Set(MythicMantis.LowerComplete);
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

                source.Trackers.Counters.Remove("mythicbee", out _);
                Subject.Reply(source, "Impressive. 15 Mythic bees without blinking an eye. I admire your loyalty.", "mantis_initial");
                
                break;
            }

            case "mantis_higher":
            {
                Subject.Reply(source, "Show me how you do it. Good luck Aisling, they can be awefully tricky to kill. Make sure you slay 20 of the Green Bees.");
                source.SendOrangeBarMessage("Kill 20 Green Bees for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Higher);
                return;
            }

            case "mantis_higher2":
            {

                if (!source.Trackers.Counters.TryGetValue("greenbee", out var mantishigher) || (mantishigher < 20))
                {
                    Subject.Reply(source, "Didn't quite finish them all did you? Go back and make sure they're dead.");
                    
                    return;
                }

                Subject.Reply(source, "It's quite exciting to come out on top. They're also very delicious if you're a mantis. Good work.", "mantis_initial");
                
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

                source.Trackers.Enums.Set(MythicMantis.HigherComplete);
                source.Trackers.Counters.Remove("greenbee", out _);

                break;
            }

            case "mantis_item":
            {
                Subject.Reply(source, "Does that make us bad parents? Ah, who cares. Grab us 25 Mythic Honey.");
                source.SendOrangeBarMessage("Collect 25 Mythic Honey for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Item);
                return;
            }

            case "mantis_item2":
            {

                if (!source.Inventory.RemoveQuantity("Mythic Honey", 25))
                {
                    Subject.Reply(source, "Nah, this won't be enough. Go get us some more, there's plenty in there.");
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
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicMantis.ItemComplete);
                Subject.Reply(source, "Ah, just enough. We could always use more but this will keep us satisfied for some time. Thank you Aisling, you really are becoming part of the colony.", "mantis_initial");

                break;
            }

            case "mantis_ally":
            {
                if (hasBee
                    && (hasBee == bee is MythicBee.Allied or MythicBee.BossStarted or MythicBee.BossDefeated))
                {
                    
                    Subject.Reply(source, $"Welcome to the Colony {source.Name}. I always knew you were strong enough to be one of us, you will fit in well.");
                    source.Trackers.Enums.Set(MythicMantis.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicMantis.Allied);
                source.SendOrangeBarMessage("You are now allied with the Mantis!");
                Subject.Reply(source, $" ");

                break;

            }

            case "mantis_boss":
            {
                Subject.Reply(source, "That would be fantastic. Good luck Adventurer, and if I don't see you again, the colony appreciates your loyalty.");
                
                
                source.Trackers.Enums.Set(MythicMantis.BossStarted);
                source.SendOrangeBarMessage("Kill Carolina three times.");
            }

                break;

            case "mantis_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Carolina", out var mantisboss1) || (mantisboss1 < 3))
                {
                    Subject.Reply(source, "Carolina is still out there, please find her and defeat her three times.");
                    
                    
                    source.SendOrangeBarMessage("You haven't completely defeated Carolina.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(source, $"I can't believe you did it. That was a miracle! The whole Mantis Colony is talking about your adventures and bravery. You took her down, no problem. We are relieved in the revenge you applied today, thank you {source.Name}!");
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
                source.Trackers.Counters.Remove("carolina", out _);
                source.Trackers.Enums.Set(MythicMantis.BossDefeated);
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