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

public class MythicBunnyScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicBunnyScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasBunny = source.Trackers.Enums.TryGetValue(out MythicBunny bunny);
        var hasHorse = source.Trackers.Enums.TryGetValue(out MythicHorse horse);
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
            case "bunny_initial":
            {
                if (hasBunny && (bunny == MythicBunny.EnemyAllied))
                {
                    Subject.Reply(
                        source,
                        "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.");

                    return;
                }

                if (hasMain && !hasBunny)
                {
                    Subject.Reply(source, "Skip", "bunny_start1start");

                    return;
                }

                if (bunny == MythicBunny.Lower)
                {
                    Subject.Reply(source, "Skip", "bunny_lower2start");

                    return;
                }

                if (bunny == MythicBunny.LowerComplete)
                {
                    Subject.Reply(source, "Skip", "bunny_start3start");

                    return;
                }

                if (bunny == MythicBunny.Higher)
                {
                    Subject.Reply(source, "Skip", "bunny_higher2start");

                    return;
                }

                if (bunny == MythicBunny.HigherComplete)
                {
                    Subject.Reply(source, "skip", "bunny_itemstart");

                    return;
                }

                if (bunny == MythicBunny.Item)
                {
                    Subject.Reply(source, "Skip", "bunny_item2start");

                    return;
                }

                if (bunny == MythicBunny.ItemComplete)
                {
                    Subject.Reply(source, "Skip", "bunny_allystart");

                    return;
                }

                if (bunny == MythicBunny.Allied)
                {
                    Subject.Reply(source, "Skip", "bunny_start5start");

                    return;
                }

                if (bunny == MythicBunny.BossStarted)
                {
                    Subject.Reply(source, "Skip", "bunny_boss2start");

                    return;
                }

                if (bunny == MythicBunny.BossDefeated)
                    Subject.Reply(
                        source,
                        $"Every bunny knows your name {source.Name
                        }! It's all around the warren, we really appreciate your hare-oic efforts.");

                break;
            }

            case "bunny_lower":
            {
                Subject.Reply(
                    source,
                    "You have our paws-tounding gratitude. Don't let the horses get your goat, though - they're quick and nimble, and they can kick like mules. But we believe in you, and we know you'll do us proud. May the bunny luck be with you!");

                source.SendOrangeBarMessage("Kill 15 Purple Horses for Big Bunny");
                source.Trackers.Enums.Set(MythicBunny.Lower);

                return;
            }

            case "bunny_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicBunny", out var purplehorse) || (purplehorse < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Purple Horses.");

                    return;
                }

                source.Trackers.Enums.Set(MythicBunny.LowerComplete);
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

                source.Trackers.Counters.Remove("MythicBunny", out _);

                Subject.Reply(
                    source,
                    "As you can imagine, horses stomping around all day can really cramp a bunny's style. We've got carrots to grow and holes to dig, and we can't do any of that with a bunch of hooves stomping all over the place. Thank you.",
                    "bunny_initial");

                break;
            }

            case "bunny_higher":
            {
                Subject.Reply(
                    source,
                    "I need you to travel deep into the fields and thin out the horse herd. Specifically, I need you to thin out 10 Gray Horses and 10 Red Horses.");

                source.SendOrangeBarMessage("Kill 10 Gray and 10 Red Horses for Big Bunny");
                source.Trackers.Enums.Set(MythicBunny.Higher);

                return;
            }

            case "bunny_higher2":
            {
                source.Trackers.Counters.TryGetValue("MythicBunny", out var grayhorse);
                source.Trackers.Counters.TryGetValue("MythicBunny1", out var redhorse);

                if ((grayhorse < 10) || (redhorse < 10))
                {
                    Subject.Reply(source, "You haven't killed enough gray or red horses.");

                    return;
                }

                Subject.Reply(
                    source,
                    "You've really hopped to it and shown your bunny-licious heroism once again. We're incredibly grateful for your help, and we can't thank you enough. Our warren's crops will be able to grow strong and healthy once again, thanks to you.",
                    "bunny_initial");

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

                source.Trackers.Enums.Set(MythicBunny.HigherComplete);
                source.Trackers.Counters.Remove("MythicBunny", out _);
                source.Trackers.Counters.Remove("MythicBunny1", out _);

                break;
            }

            case "bunny_item":
            {
                Subject.Reply(
                    source,
                    "Don't let us down, Warren Wanderer. We're counting on you to hop to it and bring back the horse hair we need. And remember, the early bunny gets the hair!");

                source.SendOrangeBarMessage("Collect 25 horse hair for Big Bunny");
                source.Trackers.Enums.Set(MythicBunny.Item);

                return;
            }

            case "bunny_item2":
            {
                if (!source.Inventory.RemoveQuantity("Horse Hair", 25))
                {
                    Subject.Reply(
                        source,
                        "Whatever it takes, we need you to gather more horse hair so that we can build warm and snug beds for all of us. We believe in you, Warren Wanderer. We know you can get the job done.");

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

                source.Trackers.Enums.Set(MythicBunny.ItemComplete);

                Subject.Reply(
                    source,
                    "You've really hopped to it and brought us enough horse hair to keep us warm and cozy through the long winter nights. This is bunny-tastic news!",
                    "bunny_initial");

                break;
            }

            case "bunny_ally":
            {
                if (hasHorse
                    && (hasHorse == horse is MythicHorse.Allied or MythicHorse.BossStarted or MythicHorse.BossDefeated))
                {
                    Subject.Reply(source, "Oh no! You already allied with the horses! Get away from us!");
                    source.Trackers.Enums.Set(MythicBunny.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicBunny.Allied);
                source.SendOrangeBarMessage("You are now allied with the bunnies!");

                Subject.Reply(
                    source,
                    $"Remember, {source.Name
                    }, that no matter where your journeys take you, you will always have a home in our warren. The bunny luck be with you always!",
                    "bunny_initial");

                break;
            }

            case "bunny_boss":
            {
                Subject.Reply(
                    source,
                    "Please be careful, Warren Wanderer. We rabbits are a fragile and gentle species, and we need your help to survive. We'll be eagerly waiting for your return, hoping to hear tales of your bunny-licious bravery and triumph over Apple Jack. May the bunny gods be with you!");

                source.Trackers.Enums.Set(MythicBunny.BossStarted);
                source.SendOrangeBarMessage("Kill Apple Jack at least three times.");
            }

                break;

            case "bunny_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicBunny", out var bunnyboss1) || (bunnyboss1 < 3))
                {
                    Subject.Reply(
                        source,
                        "Please rest and recover your strength, and then hop back into action. We'll be here waiting, hoping and praying for your success. The fate of our warren rests on your paws, Warren Wanderer. We're counting on you!");

                    source.SendOrangeBarMessage("You haven't completely defeated Apple Jack.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "Your bravery and skill in battle have truly hopped over our expectations. You've gone above and beyond to protect our warren and its inhabitants, and we will forever be grateful for your bunny-tastic efforts.");

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

                source.Trackers.Counters.Remove("MythicBunny", out _);
                source.Trackers.Enums.Set(MythicBunny.BossDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}