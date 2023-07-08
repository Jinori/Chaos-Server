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

public class MythicBeeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicBeeScript(Dialog subject, IItemFactory itemFactory)
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
            case "bee_initial":
            {
                if (hasBee && (bee == MythicBee.EnemyAllied))
                    Subject.Reply(
                        source,
                        "I cannot allow a member of the hive to be allied with our enemies. It puts our entire colony at risk, and I simply cannot tolerate it. So, I must ask you to leave our hive and never return.");

                if (hasMain && !hasBee)
                    Subject.Reply(source, "Skip", "bee_start1start");

                if (bee == MythicBee.Lower)
                    Subject.Reply(source, "Skip", "bee_lower2start");

                if (bee == MythicBee.LowerComplete)
                    Subject.Reply(source, "Skip", "bee_start3start");

                if (bee == MythicBee.Higher)
                    Subject.Reply(source, "Skip", "bee_higher2start");

                if (bee == MythicBee.HigherComplete)
                    Subject.Reply(source, "Skip", "bee_itemstart");

                if (bee == MythicBee.Item)
                    Subject.Reply(source, "Skip", "bee_item2start");

                if (bee == MythicBee.ItemComplete)
                    Subject.Reply(source, "Skip", "bee_allystart");

                if (bee == MythicBee.Allied)
                    Subject.Reply(source, "Skip", "bee_start5start");

                if (bee == MythicBee.BossStarted)
                    Subject.Reply(source, "Skip", "bee_boss2");

                if (bee == MythicBee.BossDefeated)
                    Subject.Reply(
                        source,
                        "The whole hive is buzzing about your efforts, you're an honorary bee around here. Thank you again!");

                break;
            }

            case "bee_lower":
            {
                Subject.Reply(
                    source,
                    "I wish you the best of luck on your mission. May your stinger be swift and true, and may you return to our hive victorious. Buzz on, my friend!");

                source.SendOrangeBarMessage("Kill 15 Mythic Mantis for Queen Bee");
                source.Trackers.Enums.Set(MythicBee.Lower);

                return;
            }

            case "bee_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicMantis", out var mythicMantis) || (mythicMantis < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Mythic Mantis.");

                    return;
                }

                source.Trackers.Enums.Set(MythicBee.LowerComplete);
                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                }
                else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Counters.Remove("MythicMantis", out _);

                Subject.Reply(
                    source,
                    "Your bravery and dedication have not gone unnoticed. By eliminating some of the mythic mantis, we can now send our workers to gather pollen and nectar without fear of attack.",
                    "bee_initial");

                Subject.NextDialogKey = "bee_initial";

                break;
            }

            case "bee_higher":
            {
                Subject.Reply(
                    source,
                    "Please eliminate 20 Brown Mantis. The nectar from the flowers that they are guarding is among the sweetest we have ever tasted.");

                source.SendOrangeBarMessage("Kill 20 Brown Mantis for the Bee Queen.");
                source.Trackers.Enums.Set(MythicBee.Higher);

                return;
            }

            case "bee_higher2":
            {
                if (!source.Trackers.Counters.TryGetValue("brownmantis", out var brownmantis) || (brownmantis < 20))
                {
                    Subject.Reply(source, "You haven't killed enough Brown Mantis.");

                    return;
                }

                Subject.Reply(
                    source,
                    "Thank you from the bottom of my queenly heart. You truly are the pollen to our flower. Buzz on, my dear bee protector!",
                    "bee_initial");

                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                }
                else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicBee.HigherComplete);
                source.Trackers.Counters.Remove("brownmantis", out _);
            }

                break;

            case "bee_item":
            {
                Subject.Reply(source, "Thank you mighty bee! Fight the mantis colony and collect 25 dendron flowers from them.");
                source.SendOrangeBarMessage("Collect 25 Dendron Flower for the Queen Bee");
                source.Trackers.Enums.Set(MythicBee.Item);

                return;
            }

            case "bee_item2":
            {
                if (!source.Inventory.RemoveQuantity("Dendron Flower", 25))
                {
                    Subject.Reply(source, "This isn't enough Dendron Flowers to feed the hive, we need more of its sweet nectar.");

                    return;
                }

                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                }
                else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicBee.ItemComplete);

                Subject.Reply(
                    source,
                    "With these flowers, we will be able to produce the most delicious and nutritious nectar that will keep our hive buzzing with joy and energy. You have truly outdone yourself this time, my dear ally.",
                    "bee_initial");

                Subject.NextDialogKey = "bee_initial";

                break;
            }

            case "bee_ally":
            {
                if (hasMantis
                    && (hasMantis == mantis is MythicMantis.Allied or MythicMantis.BossStarted or MythicMantis.BossDefeated))
                {
                    Subject.Reply(source, "No way! You have been allied to the Mantis Colony this entire time traitor! Buzz off!");
                    source.Trackers.Enums.Set(MythicBee.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicBee.Allied);
                source.SendOrangeBarMessage("You are now allied with the Bees!");

                Subject.Reply(
                    source,
                    $"So let me just say, we're pollen for you {source.Name
                    }! It's bee-n a long time since we've had such a dedicated ally in our fight against the mantis colony. With you on our side, we can bee unstoppable!",
                    "bee_initial");

                break;
            }

            case "bee_boss":
            {
                Subject.Reply(source, "So fly out, my buzz-worthy ally, and bring us victory over Fire Tree!");
                source.Trackers.Enums.Set(MythicBee.BossStarted);
                source.SendOrangeBarMessage("Kill Fire Tree three times.");
            }

                break;

            case "bee_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("FireTree", out var firetree) || (firetree < 3))
                {
                    Subject.Reply(
                        source,
                        "Oh dear, it seems Fire Tree is still at large. This is troubling news for my hive and my workers. Please be careful, we cannot afford to let the mantis leader continue to harm us. I urge you to finish the task at hand and take down Fire Tree once and for all. The safety and well-being of my hive and all its inhabitants depend on it.");

                    source.SendOrangeBarMessage("You haven't completely defeated Fire Tree.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "Buzzing fantastic! You've done it! You've defeated Fire Tree, the notorious mantis leader. Thank you so much for your dedication and bravery in protecting my hive and my fellow bees. Your efforts will not go unnoticed. You truly are the bee's knees!");

                source.Animate(ani2, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                }
                else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage("You received 25000000 experience!");
                }

                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Trackers.Counters.Remove("FireTree", out _);
                source.Trackers.Enums.Set(MythicBee.BossDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}