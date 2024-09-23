using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MythicQuestScripts;

public class MythicHorseScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicHorseScript(Dialog subject, IItemFactory itemFactory)
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

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "horse_initial":
            {
                if (hasHorse && (horse == MythicHorse.EnemyHorseAllied))
                {
                    Subject.Reply(
                        source,
                        "You're already allied with the bunnies! That's like a kick in the teeth! Giddy-up and leave my sight immediately.");

                    return;
                }

                if (hasMain && !hasHorse)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_start1start");

                    return;
                }

                if (horse == MythicHorse.LowerHorse)
                {
                    Subject.Reply(source, "Skip", "horse_lower2start");

                    return;
                }

                if (horse == MythicHorse.LowerHorseComplete)
                {
                    Subject.Reply(source, "Skip", "horse_start3start");

                    return;
                }

                if (horse == MythicHorse.HigherHorse)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_higher2start");

                    return;
                }

                if (horse == MythicHorse.HigherHorseComplete)
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_start4start");

                if (horse == MythicHorse.ItemHorse)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_item2start");

                    return;
                }

                if (horse == MythicHorse.ItemHorseComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_allystart");

                    return;
                }

                if (horse == MythicHorse.AlliedHorse)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_bossstart");

                    return;
                }

                if (horse == MythicHorse.BossHorseStarted)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "horse_boss2start");

                    return;
                }

                if (horse == MythicHorse.BossHorseDefeated)
                    Subject.Reply(source, "Thank you again partner, you really have saved my herd and our carrot fields are thriving.");

                break;
            }

            case "horse_lower":
            {
                Subject.Reply(source, "I knew i could count on you, I'll see you soon traveler.");
                source.SendOrangeBarMessage("Kill 15 Slick Bunnies for the Horse Leader");
                source.Trackers.Enums.Set(MythicHorse.LowerHorse);

                return;
            }

            case "horse_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicHorse1", out var whitebunny) || (whitebunny < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Slick Bunnies.");

                    return;
                }

                source.Trackers.Enums.Set(MythicHorse.LowerHorseComplete);

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

                source.Trackers.Counters.Remove("MythicHorse1", out _);

                Subject.Reply(
                    source,
                    "Well done, adventurer! We saw you thundering through the fields with those bunnies in tow. You've lassoed 15 of them, by our count! That should teach them to stay away from our carrots.",
                    "horse_initial");

                break;
            }

            case "horse_higher":
            {
                Subject.Reply(
                    source,
                    "We knew we could count on you, partner. Good luck out there, and remember, don't let those angry and clever bunnies get the best of you! (The horse leader lets out a hearty neigh)");

                source.SendOrangeBarMessage("Kill 10 Angry and 10 Clever Bunnies for the Horse Leader");
                source.Trackers.Enums.Set(MythicHorse.HigherHorse);

                return;
            }

            case "horse_higher2":
            {
                source.Trackers.Counters.TryGetValue("MythicHorse2", out var angrybunny);
                source.Trackers.Counters.TryGetValue("MythicHorse3", out var cleverbunny);

                if ((angrybunny < 10) || (cleverbunny < 10))
                {
                    Subject.Reply(source, "You haven't killed enough angry and clever bunnies");

                    return;
                }

                Subject.Reply(
                    source,
                    "Well done, partner! We saw you charging through the fields, and we knew you meant business. Those bunnies didn't stand a chance against you.",
                    "horse_initial");

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

                source.Trackers.Enums.Set(MythicHorse.HigherHorseComplete);
                source.Trackers.Counters.Remove("MythicHorse2", out _);
                source.Trackers.Counters.Remove("MythicHorse3", out _);

                break;
            }

            case "horse_item":
            {
                Subject.Reply(
                    source,
                    "Great! And be careful out there. The bunnies can be quite sneaky, and they're not too fond of outsiders messing with their crops.");

                source.SendOrangeBarMessage("Collect 25 carrots for the Horse Leader");
                source.Trackers.Enums.Set(MythicHorse.ItemHorse);

                return;
            }

            case "horse_item2":
            {
                if (!source.Inventory.RemoveQuantity("Carrot", 25))
                {
                    Subject.Reply(
                        source,
                        "Hmm, that's a start, but it's not quite enough to feed our herd. We were really hoping for at least 25. Any chance you can go out and find some more?");

                    return;
                }

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

                source.Trackers.Enums.Set(MythicHorse.ItemHorseComplete);

                Subject.Reply(
                    source,
                    "Hot diggity! You really are a hero to our herd, my friend. We were getting mighty worried about our carrot supply, but you came through in the clutch. And just in the nick of time, too.",
                    "horse_initial");

                break;
            }

            case "horse_ally":
            {
                if (hasBunny
                    && (hasBunny == bunny is MythicBunny.AlliedBunny or MythicBunny.BossBunnyStarted or MythicBunny.BossBunnyDefeated))
                {
                    Subject.Reply(
                        source,
                        "What?! You're allied with the bunnies? How could you do this to us? We trusted you, and you went and made an alliance with our sworn enemies! Go away!");

                    source.Trackers.Enums.Set(MythicHorse.EnemyHorseAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicHorse.AlliedHorse);
                source.SendOrangeBarMessage("You are now allied with the horses!");

                Subject.Reply(
                    source,
                    $"(neighs gratefully) Thank you, thank you, {source.Name
                    }. You've shown us that you're not just any old mare or stallion. You've proven yourself to be a true herd member, and for that, we are truly grateful.",
                    "horse_initial");

                break;
            }

            case "horse_boss":
            {
                Subject.Reply(
                    source,
                    "That's what I like to hear, my little pony! You've proven yourself to be a dependable ally, and I know you have the chops to take down Mr.Hopps. Just watch your back, he's a slippery sucker, and he'll do anything to keep those carrots for himself.");

                source.Trackers.Enums.Set(MythicHorse.BossHorseStarted);
                source.SendOrangeBarMessage("Kill Mr.Hopps three times.");
            }

                break;

            case "horse_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicHorse", out var mrhopps) || (mrhopps < 3))
                {
                    Subject.Reply(
                        source,
                        "Don't feel down, partner. You gave it your all, and that's all any of us can do. Mr.Hopps may be a tough nut to crack, but we won't give up. We'll keep on fighting until we reach the finish line.");

                    source.SendOrangeBarMessage("Kill Mr.Hopps three times.");

                    return;
                }

                Subject.Reply(
                    source,
                    "Oh, horseshoes and hay bales! You've done it, my trusty ally! We knew you had it in you all along. You've earned your oats today, and then some. I can't thank you enough for what you've done for our herd.");

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

                source.Trackers.Counters.Remove("MythicHorse", out _);
                source.Trackers.Enums.Set(MythicHorse.BossHorseDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5) &&
                    !source.Trackers.Enums.HasValue(MythicQuestMain.CompletedMythic))
                {
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
                }
            }

                break;
        }
    }
}