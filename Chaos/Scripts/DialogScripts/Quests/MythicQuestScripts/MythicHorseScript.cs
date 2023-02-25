using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;

namespace Chaos.Scripts.DialogScripts.Quests;

public class MythicHorseScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicHorseScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        SimpleCache = simpleCache;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Enums.TryGetValue(out MythicQuestMain main);
        var hasBunny = source.Enums.TryGetValue(out MythicBunny bunny);
        var hasHorse = source.Enums.TryGetValue(out MythicHorse horse);
        var hasGargoyle = source.Enums.TryGetValue(out MythicGargoyle gargoyle);
        var hasZombie = source.Enums.TryGetValue(out MythicZombie zombie);
        var hasFrog = source.Enums.TryGetValue(out MythicFrog frog);
        var hasWolf = source.Enums.TryGetValue(out MythicWolf wolf);
        var hasMantis = source.Enums.TryGetValue(out MythicMantis mantis);
        var hasBee = source.Enums.TryGetValue(out MythicBee bee);
        var hasKobold = source.Enums.TryGetValue(out MythicKobold kobold);
        var hasGrimlock = source.Enums.TryGetValue(out MythicGrimlock grimlock);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var twentyPercent = MathEx.GetPercentOf<int>(tnl, 20);
        var fiftyPercent = MathEx.GetPercentOf<int>(tnl, 50);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "horse_initial":
            {
                if (hasHorse && (horse == MythicHorse.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "I told you to go away! You are not welcome here anymore!";
                    Subject.NextDialogKey = "Close";
                }
                
                if ((main == MythicQuestMain.MythicStarted) && !hasHorse)
                {
                    Subject.Text = "Well howdy there, traveler! You look like just the horse-whisperer we need. We've got a carrot conundrum on our hooves - those darn bunnies keep snatching our carrots from right under our noses. We need you to take care of the situation.";
                    
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
                    Subject.Text = "Welcome back, my friend! Have you managed to defeat those bunnies?";

                    var option = new DialogOption
                    {
                        DialogKey = "horse_lower2",
                        OptionText = "Yeah, I cleared them."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_no1",
                        OptionText = "I'm sorry, not yet."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }
                
                if (horse == MythicHorse.LowerComplete)
                {
                    Subject.Text = "We're grateful for all you've done for us, but we've got another problem that needs fixin'.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "horse_start3",
                        OptionText = "What's the trouble now, your equine-ness?"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_no",
                        OptionText = "I'm done for now."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }
                
                if (horse == MythicHorse.Higher)
                {
                    Subject.Text = "Hoppy Greetings, welcome back. Did you clear those hoofed oppressors?";

                    var option = new DialogOption
                    {
                        DialogKey = "horse_higher2",
                        OptionText = "Yeah, it is done."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_no1",
                        OptionText = "I'm working on it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }
                
                if (horse == MythicHorse.HigherComplete)
                {
                    Subject.Text = "We've got another job for you, if you're up for it.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "horse_start4",
                        OptionText = "Sure thing, what do you need?"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_no",
                        OptionText = "No, good luck."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);
                }
                
                if (horse == MythicHorse.Item)
                {
                    Subject.Text = "Well, well, well, would you look at that! The prodigal carrot gatherer returns! Did you manage to find all 25 carrots we needed?";

                    var option = new DialogOption
                    {
                        DialogKey = "horse_item2",
                        OptionText = "I did! I scoured the fields."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_no1",
                        OptionText = "I am still searching."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }
                
                if (horse == MythicHorse.ItemComplete)
                {
                    Subject.Text = "You have proven yourself to be a valuable ally to our warren, dear traveler. You have saved our crops, defended our burrows, and defeated many of our enemies. You have shown us that you share our values of kindness and bravery, and for that, we are very grateful. We would be honored if you would consider allying with us, and becoming a part of our family. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

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
                    Subject.Text =
                        "Listen up, neigh-sayer! I have a new mission for you, and it's a whinny-taker. You see, all these bunnies have been giving us the hoof, and they all answer to one bunny-boss: Mr. Hopps. He's the one who's been whipping them into a frenzy and leading them in their carrot heists";

                    var option = new DialogOption
                    {
                        DialogKey = "horse_boss",
                        OptionText = "Consider it done.."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_noboss",
                        OptionText = "I won't do it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;
                }
                if (horse == MythicHorse.BossStarted)
                {
                    Subject.Text =
                        "Did you find the horse bosses? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "horse_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "horse_noboss2",
                        OptionText = "I can't do it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;
                }
                if (horse == MythicHorse.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "horse_lower":
            {
                Subject.Text = "You have our paws-tounding gratitude. Don't let the horses get your goat, though - they're quick and nimble, and they can kick like mules. But we believe in you, and we know you'll do us proud. May the horse luck be with you!";
                source.SendOrangeBarMessage("Kill 20 White Bunnies for the Horse Leader");
                source.Enums.Set(MythicHorse.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "horse_lower2":
            {

                if (!source.Counters.TryGetValue("HorseLower", out var horselower) || (horselower < 20))
                {
                    Subject.Text = "You haven't killed enough lower horses.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicHorse.LowerComplete);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("HorseLower", out _);
                Subject.Text = "Well done, adventurer! We saw you thundering through the fields with those bunnies in tow. You've lassoed 20 of them, by our count! That should teach them to stay away from our carrots.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "horse_initial";

                break;
            }

            case "horse_higher":
            {
                Subject.Text = "We knew we could count on you, partner. Good luck out there, and remember, don't let those brown bunnies get the best of you! (The horse leader lets out a hearty neigh as you walk away.)";
                source.SendOrangeBarMessage("Kill 20 Brown Bunnies for the Horse Leader");
                source.Enums.Set(MythicHorse.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "horse_higher2":
            {

                if (!source.Counters.TryGetValue("HorseHigher", out var horsehigher) || (horsehigher < 20))
                {
                    Subject.Text = "You haven't killed enough brown bunnies";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "Well done, partner! We saw you charging through the fields, and we knew you meant business. Those brown bunnies didn't stand a chance against you.";
                Subject.NextDialogKey = "horse_initial";
                Subject.Type = MenuOrDialogType.Normal;
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicHorse.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("HorseHigher", out _);

                break;
            }

            case "horse_item":
            {
                Subject.Text = "Great! And be careful out there. The bunnies can be quite sneaky, and they're not too fond of outsiders messing with their crops.";
                source.SendOrangeBarMessage("Collect 25 carrots for the Horse Leader");
                source.Enums.Set(MythicHorse.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "horse_item2":
            {

                if (!source.Inventory.RemoveQuantity("Carrot", 25))
                {
                    Subject.Text = "Hmm, that's a start, but it's not quite enough to feed our herd. We were really hoping for at least 25. Any chance you can go out and find some more?";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicHorse.ItemComplete);
                Subject.Text = "Hot diggity! You really are a hero to our herd, my friend. We were getting mighty worried about our carrot supply, but you came through in the clutch. And just in the nick of time, too.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "horse_initial";

                break;
            }

            case "horse_ally":
            {
                if (hasBunny
                    && (hasBunny == bunny is MythicBunny.Allied or MythicBunny.BossStarted or MythicBunny.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "What?! You're allied with the bunnies? How could you do this to us? We trusted you, and you went and made an alliance with our sworn enemies! Go away!";
                    source.Enums.Set(MythicHorse.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicHorse.Allied);
                source.SendOrangeBarMessage("You are now allied with the bunnies!");
                Subject.Text = $"Remember, {source.Name}, that no matter where your journeys take you, you will always have a home in our warren. The horse luck be with you always!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "horse_initial";

                break;

            }

            case "horse_boss":
            {
                Subject.Text = "That's what I like to hear, my little pony! You've proven yourself to be a dependable ally, and I know you have the chops to take down Mr. Hopps. Just watch your back, he's a slippery sucker, and he'll do anything to keep those carrots for himself.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicHorse.BossStarted);
                source.SendOrangeBarMessage("Kill the bunny boss three times.");
            }

                break;

            case "horse_boss2":
            {
                if (!source.Counters.TryGetValue("MrHopps", out var horseboss1) || (horseboss1 < 3))
                {
                    Subject.Text = "Please go kill the bunny boss atleast three times.";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";

                    return;
                }

                Subject.Text = "Oh my hop! Thank you so much! This has really helped us bunnies get ahead. Those big, bad horses have been thumping on us for far too long, but now we can finally fight back!";
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Counters.Remove("HorseBoss", out _);
                source.Enums.Set(MythicHorse.BossDefeated);
                source.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                {
                    source.Enums.Set(MythicQuestMain.CompletedAll);
                }
            }

                break;
        }
    }
}