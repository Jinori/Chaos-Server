using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
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
                if (hasHorse && (horse == MythicHorse.EnemyAllied))
                {
                    Subject.Reply(source,
                        "You're already allied with the bunnies! That's like a kick in the teeth! Giddy-up and leave my sight immediately.");
                }

                if (hasMain && !hasHorse)
                {
                    Subject.Reply(source,
                        "Well howdy there, traveler! You look like just the horse-whisperer we need. We've got a carrot conundrum on our hooves - those darn bunnies keep snatching our carrots from right under our noses. We need you to take care of the situation.");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_start1",
                        OptionText = "What do you need me to do?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (horse == MythicHorse.Lower)
                {
                    Subject.Reply(source, "Welcome back, my friend! Have you managed to defeat those bunnies?");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_lower2",
                        OptionText = "Yeah, I cleared them."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);


                    return;

                }

                if (horse == MythicHorse.LowerComplete)
                {
                    Subject.Reply(source, "We're grateful for all you've done for us, but we've got another problem that needs fixin'.");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_start3",
                        OptionText = "What's the trouble now, your equine-ness?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (horse == MythicHorse.Higher)
                {
                    Subject.Reply(source,
                        "Did you manage to hoof it over to their warren and give them a good neighing? Or did they outsmart you and give you the slip? I hope you didn't let them burrow under your skin.");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_higher2",
                        OptionText = "They were no match for me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);


                    return;

                }

                if (horse == MythicHorse.HigherComplete)
                {
                    Subject.Reply(source,
                        "It looks like you're itching for another adventure. I can see it in your eyes, you're ready to gallop off into the sunset.");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_start4",
                        OptionText = "I sure am."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (horse == MythicHorse.Item)
                {
                    Subject.Reply(source,
                        "Well, well, well, would you look at that! The prodigal carrot gatherer returns! Did you manage to find all 25 carrots we needed?");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_item2",
                        OptionText = "I did! I scoured the fields."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (horse == MythicHorse.ItemComplete)
                {
                    Subject.Reply(source,
                        "The horse herd really appreciates your bravery and we need someone with a little more horsepower to join us. So here's what I propose: why don't we form an alliance? You help us drive those bunnies away, and we'll give you free rein to our carrot fields. What do you say, partner?\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_ally",
                        OptionText = "Ally with Horse"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (horse == MythicHorse.Allied)
                {
                    Subject.Reply(source,
                        "New Ally! I have another mission for you, and it's a whinny-taker. You see, all these bunnies have been giving us the hoof, and they all answer to one bunny-boss: Mr. Hopps. He's the one who's been whipping them into a frenzy and leading them in their carrot heists.");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_boss",
                        OptionText = "Consider it done."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (horse == MythicHorse.BossStarted)
                {
                    Subject.Reply(source,
                        "(neighs excitedly) Hello, hello, my valiant friend! It's good to see you've made it back in one piece. So, did you manage to defeat that rascally rabbit, Mr. Hopps?");

                    var option = new DialogOption
                    {
                        DialogKey = "horse_boss2",
                        OptionText = "Mr.Hopps has been defeated."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);


                    return;
                }

                if (horse == MythicHorse.BossDefeated)
                {

                    Subject.Reply(source, "Thank you again partner, you really have saved my herd and our carrot fields are thriving.");
                }

                break;
            }

            case "horse_lower":
            {
                Subject.Reply(source, "I knew i could count on you, I'll see you soon traveler.");
                source.SendOrangeBarMessage("Kill 15 White Bunnies for the Horse Leader");
                source.Trackers.Enums.Set(MythicHorse.Lower);
                

                return;
            }

            case "horse_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("whitebunny", out var whitebunny) || (whitebunny < 15))
                {
                    Subject.Reply(source, "You haven't killed enough White Bunnies.");
                    

                    return;
                }

                source.Trackers.Enums.Set(MythicHorse.LowerComplete);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }
                source.Trackers.Counters.Remove("whitebunny", out _);

                Subject.Reply(source,
                    "Well done, adventurer! We saw you thundering through the fields with those bunnies in tow. You've lassoed 15 of them, by our count! That should teach them to stay away from our carrots.");

                
                

                break;
            }

            case "horse_higher":
            {
                Subject.Reply(source,
                    "We knew we could count on you, partner. Good luck out there, and remember, don't let those brown and purple bunnies get the best of you! (The horse leader lets out a hearty neigh)");

                source.SendOrangeBarMessage("Kill 10 Brown and 10 Purple Bunnies for the Horse Leader");
                source.Trackers.Enums.Set(MythicHorse.Higher);
                

                return;
            }

            case "horse_higher2":
            {
                source.Trackers.Counters.TryGetValue("brownbunny", out var brownbunny);
                source.Trackers.Counters.TryGetValue("purplebunny", out var purplebunny);

                if ((brownbunny < 10) || (purplebunny < 10))
                {
                    Subject.Reply(source, "You haven't killed enough brown and purple bunnies");
                    

                    return;
                }

                Subject.Reply(source,
                    "Well done, partner! We saw you charging through the fields, and we knew you meant business. Those bunnies didn't stand a chance against you.");

                
                

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicHorse.HigherComplete);
                source.Trackers.Counters.Remove("brownbunny", out _);
                source.Trackers.Counters.Remove("purplebunny", out _);

                break;
            }

            case "horse_item":
            {
                Subject.Reply(source,
                    "Great! And be careful out there. The bunnies can be quite sneaky, and they're not too fond of outsiders messing with their crops.");

                source.SendOrangeBarMessage("Collect 25 carrots for the Horse Leader");
                source.Trackers.Enums.Set(MythicHorse.Item);
                

                return;
            }

            case "horse_item2":
            {

                if (!source.Inventory.RemoveQuantity("Carrot", 25))
                {
                    Subject.Reply(source,
                        "Hmm, that's a start, but it's not quite enough to feed our herd. We were really hoping for at least 25. Any chance you can go out and find some more?");

                    

                    return;
                }


                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicHorse.ItemComplete);

                Subject.Reply(source,
                    "Hot diggity! You really are a hero to our herd, my friend. We were getting mighty worried about our carrot supply, but you came through in the clutch. And just in the nick of time, too.");

                
                

                break;
            }

            case "horse_ally":
            {
                if (hasBunny
                    && (hasBunny == bunny is MythicBunny.Allied or MythicBunny.BossStarted or MythicBunny.BossDefeated))
                {
                    

                    Subject.Reply(source,
                        "What?! You're allied with the bunnies? How could you do this to us? We trusted you, and you went and made an alliance with our sworn enemies! Go away!");

                    source.Trackers.Enums.Set(MythicHorse.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicHorse.Allied);
                source.SendOrangeBarMessage("You are now allied with the bunnies!");

                Subject.Reply(source, $"(neighs gratefully) Thank you, thank you, {source.Name
                }. You've shown us that you're not just any old mare or stallion. You've proven yourself to be a true herd member, and for that, we are truly grateful.");

                
                

                break;

            }

            case "horse_boss":
            {
                Subject.Reply(source,
                    "That's what I like to hear, my little pony! You've proven yourself to be a dependable ally, and I know you have the chops to take down Mr.Hopps. Just watch your back, he's a slippery sucker, and he'll do anything to keep those carrots for himself.");

                
                
                source.Trackers.Enums.Set(MythicHorse.BossStarted);
                source.SendOrangeBarMessage("Kill Mr.Hopps three times.");
            }

                break;

            case "horse_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MrHopps", out var mrhopps) || (mrhopps < 3))
                {
                    Subject.Reply(source,
                        "Don't feel down, partner. You gave it your all, and that's all any of us can do. Mr. Hopps may be a tough nut to crack, but we won't give up. We'll keep on fighting until we reach the finish line.");

                    
                    
                    source.SendOrangeBarMessage("Kill Mr.Hopps three times.");

                    return;
                }

                Subject.Reply(source,
                    "Oh, horseshoes and hay bales! You've done it, my trusty ally! We knew you had it in you all along. You've earned your oats today, and then some. I can't thank you enough for what you've done for our herd.");

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage($"You received 25000000 experience!");
                }

                source.Trackers.Counters.Remove("mrhopps", out _);
                source.Trackers.Enums.Set(MythicHorse.BossDefeated);
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