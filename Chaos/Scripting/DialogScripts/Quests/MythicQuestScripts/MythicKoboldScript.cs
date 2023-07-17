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

public class MythicKoboldScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicKoboldScript(Dialog subject, IItemFactory itemFactory)
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
            case "kobold_initial":
            {
                if (hasKobold && (kobold == MythicKobold.EnemyKoboldAllied))
                {
                    Subject.Reply(
                        source,
                        "I told you to get lost, do not make me use these claws. I am still so angry at you. They steal our land and now our allies. (Kobold Leader growls in anger)");

                    return;
                }

                if (hasMain && !hasKobold)
                {
                    Subject.Reply(source, "Skip", "kobold_start1start");

                    return;
                }

                if (kobold == MythicKobold.LowerKobold)
                {
                    Subject.Reply(source, "Skip", "kobold_lower2start");

                    return;
                }

                if (kobold == MythicKobold.LowerKoboldComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "kobold_start3start");

                    return;
                }

                if (kobold == MythicKobold.HigherKobold)
                {
                    Subject.Reply(source, "Skip", "kobold_higher2start");

                    return;
                }

                if (kobold == MythicKobold.HigherKoboldComplete)
                {
                    Subject.Reply(source, "Skip", "kobold_itemstart");

                    return;
                }

                if (kobold == MythicKobold.ItemKobold)
                {
                    Subject.Reply(source, "Skip", "kobold_item2start");

                    return;
                }

                if (kobold == MythicKobold.ItemKoboldComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "kobold_allystart");

                    return;
                }

                if (kobold == MythicKobold.AlliedKobold)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "kobold_start5start");

                    return;
                }

                if (kobold == MythicKobold.BossKoboldStarted)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "kobold_boss2start");

                    return;
                }

                if (kobold == MythicKobold.BossKoboldDefeated)
                    Subject.Reply(source, "Thank you again Aisling for your help. We are winning our fight.");

                break;
            }

            case "kobold_lower":
            {
                Subject.Reply(
                    source,
                    "My, thank you Aisling. You don't know what it'll mean for us, but we really need this to happen. Kill 15 Grimlock Workers and come back, I'll let my people know the plan.");

                source.SendOrangeBarMessage("Kill 15 Grimlock Workers for Kobold Leader");
                source.Trackers.Enums.Set(MythicKobold.LowerKobold);

                return;
            }

            case "kobold_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicKobold1", out var grimlockworker) || (grimlockworker < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Grimlock Workers, they are still in the area. It isn't enough.");

                    return;
                }

                source.Trackers.Enums.Set(MythicKobold.LowerKoboldComplete);
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

                source.Trackers.Counters.Remove("MythicKobold1", out _);

                Subject.Reply(
                    source,
                    "Your actions have caused a major distraction for the Grimlocks, giving us the opportunity to safely farm resources on our old land, which is crucial for our survival. Thank you.",
                    "kobold_initial");

                break;
            }

            case "kobold_higher":
            {
                Subject.Reply(
                    source,
                    "You will? That is great to hear, the grimlocks won't know what's coming. My people will be safe with your help. You need to kill 10 Grimlock Guards and 10 Grimlock Rogues.");

                source.SendOrangeBarMessage("Kill 10 Grimlock Guards and 10 Grimlock Rogues");
                source.Trackers.Enums.Set(MythicKobold.HigherKobold);

                return;
            }

            case "kobold_higher2":
            {
                source.Trackers.Counters.TryGetValue("MythicKobold2", out var grimlockguard);
                source.Trackers.Counters.TryGetValue("MythicKobold3", out var grimlockrogue);

                if ((grimlockguard < 10) || (grimlockrogue < 10))
                {
                    Subject.Reply(source, "You haven't killed enough Grimlock Guards and Grimlock Rogues.");

                    return;
                }

                Subject.Reply(
                    source,
                    "That's great news. They are pretty riled up about it and will probably retreat for now. Thank you Aisling, my people will hear about your impressive abilities for many moons.",
                    "kobold_initial");

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

                source.Trackers.Enums.Set(MythicKobold.HigherKoboldComplete);
                source.Trackers.Counters.Remove("MythicKobold2", out _);
                source.Trackers.Counters.Remove("MythicKobold3", out _);

                break;
            }

            case "kobold_item":
            {
                Subject.Reply(
                    source,
                    "That will be fantastic Aisling, I can't wait to have it once again. You should've seen me in my prime, my hair would shine like the sun with just a few drops of this potion. Please, be sure to grab 25! For all of us.");

                source.SendOrangeBarMessage("Collect 25 Strange Potion");
                source.Trackers.Enums.Set(MythicKobold.ItemKobold);

                return;
            }

            case "kobold_item2":
            {
                if (!source.Inventory.RemoveQuantity("Strange Potion", 25))
                {
                    Subject.Reply(source, "This won't be enough. Please get us some more.");

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

                source.Trackers.Enums.Set(MythicKobold.ItemKoboldComplete);

                Subject.Reply(
                    source,
                    "Perfect! I cannot wait to use this stuff! I missed it so much. My people will be thrilled. Thank you Aisling, this is just short of a miracle.",
                    "kobold_initial");

                break;
            }

            case "kobold_ally":
            {
                if (hasGrimlock
                    && (hasGrimlock == grimlock is MythicGrimlock.AlliedGrimlock or MythicGrimlock.BossGrimlockStarted or MythicGrimlock.BossGrimlockDefeated))
                {
                    Subject.Reply(
                        source,
                        "No way! You have been allied to the Grimlocks this entire time!? I was so blind in need that I didn't see the traitor before me. I will gouge your eyes out with my new sharp claws. Go far away from me.");

                    source.Trackers.Enums.Set(MythicKobold.EnemyKoboldAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicKobold.AlliedKobold);
                source.SendOrangeBarMessage("You are now allied with the Kobolds!");

                Subject.Reply(
                    source,
                    "We Kobolds are not always trusted by other races, but your actions have shown us that there are still those who are willing to stand with us.",
                    "kobold_initial");

                break;
            }

            case "kobold_boss":
            {
                Subject.Reply(
                    source,
                    "We owe you for everything but this is the utmost important. My people shall be safe once you defeat the Grimlock Princess, remember you need to defeat her three times.");

                source.Trackers.Enums.Set(MythicKobold.BossKoboldStarted);
                source.SendOrangeBarMessage("Kill Grimlock Princess three times.");
            }

                break;

            case "kobold_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicKobold", out var koboldboss1) || (koboldboss1 < 3))
                {
                    Subject.Reply(source, "She is still out there, I can smell her.");

                    source.SendOrangeBarMessage("You haven't completely defeated the Grimlock Princess");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "We Kobolds are grateful to you for your bravery and skill in defeating the Grimlock princess. You have shown great honor and courage, and we will always remember your deeds. Our people are now safe thanks to your actions, and we are forever in your debt.");

                source.Animate(ani2, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                }
                else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                }

                source.Trackers.Counters.Remove("MythicKobold", out _);
                source.Trackers.Enums.Set(MythicKobold.BossKoboldDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}