using Chaos.Common.Definitions;
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
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MythicQuestScripts;

public class MythicBunnyScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicBunnyScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasBunny)

                {
                    Subject.Text = "Ears to you, traveler. I am the leader of this warren of bunnies, and I carrot thank you enough for coming to our aid. The neigh-sayers may think we're just cute and fluffy, but we're tougher than we look.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "bunny_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (bunny == MythicBunny.Lower)
                {
                    Subject.Text = "Well, well, well, look who's back! It's our favorite rabbit-loving adventurer! Have you come to tell us that you've completed the task we gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "bunny_lower2",
                        OptionText = "Yes Big Bunny."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (bunny == MythicBunny.LowerComplete)
                {
                    Subject.Text = "Warren Wanderer, we are in need of your assistance once again. It seems that another group of horses has invaded our territory and is causing chaos and destruction. We need your help to remove them from our fields, just as you did with the previous group.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "bunny_start3",
                        OptionText = "No problem Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (bunny == MythicBunny.Higher)
                {
                    Subject.Text = "Hoppy Greetings, welcome back. Did you clear those hoofed oppressors?";

                    var option = new DialogOption
                    {
                        DialogKey = "bunny_higher2",
                        OptionText = "Yeah, it is done."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (bunny == MythicBunny.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "bunny_item",
                        OptionText = "I can get that."
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (bunny == MythicBunny.Item)
                {
                    Subject.Text = "Hare-oic Aisling! Did you collect all the horse hair?";

                    var option = new DialogOption
                    {
                        DialogKey = "bunny_item2",
                        OptionText = "I have them here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (bunny == MythicBunny.ItemComplete)
                {
                    Subject.Text = "You have proven yourself to be a valuable ally to our warren, dear traveler. You have saved our crops, defended our burrows, and defeated many of our enemies. You have shown us that you share our values of kindness and bravery, and for that, we are very grateful. We would be honored if you would consider allying with us, and becoming a part of our family. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "bunny_ally",
                        OptionText = "Ally with Bunny"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "bunny_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (bunny == MythicBunny.Allied)
                {
                    Subject.Text =
                        "Warren Wanderer, we have another urgent request for you. We have learned that the leader of the horse herd that has been causing us so much trouble is a powerful and dangerous horse named Apple Jack. We need you to go and defeat Apple Jack three times to ensure that our fields remain safe and secure.";
                    var option = new DialogOption
                    {
                        DialogKey = "bunny_start5",
                        OptionText = "Anything for you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (bunny == MythicBunny.BossStarted)
                {
                    Subject.Text =
                        "Did you find Apple Jack? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "bunny_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (bunny == MythicBunny.BossDefeated)
                {

                    Subject.Text = $"Every bunny knows your name {source.Name}! It's all around the warren, we really appreciate your hare-oic efforts.";
                }

                break;
            }

            case "bunny_lower":
            {
                Subject.Text = "You have our paws-tounding gratitude. Don't let the horses get your goat, though - they're quick and nimble, and they can kick like mules. But we believe in you, and we know you'll do us proud. May the bunny luck be with you!";
                source.SendOrangeBarMessage("Kill 20 Purple Horses for Big Bunny");
                source.Enums.Set(MythicBunny.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "bunny_lower2":
            {

                if (!source.Counters.TryGetValue("purplehorse", out var purplehorse) || (purplehorse < 15))
                {
                    Subject.Text = "You haven't killed enough Purple Horses.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicBunny.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("purplehorse", out _);
                Subject.Text = "As you can imagine, horses stomping around all day can really cramp a bunny's style. We've got carrots to grow and holes to dig, and we can't do any of that with a bunch of hooves stomping all over the place. Thank you.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "bunny_initial";

                break;
            }

            case "bunny_higher":
            {
                Subject.Text = "I need you to travel deep into the fields and thin out the horse herd. Specifically, I need you to thin out 10 Gray Horses and 10 Red Horses.";
                source.SendOrangeBarMessage("Kill 10 Gray and 10 Red Horses for Big Bunny");
                source.Enums.Set(MythicBunny.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "bunny_higher2":
            {
                source.Counters.TryGetValue("grayhorse", out var grayhorse);
                source.Counters.TryGetValue("redhorse", out var redhorse);

                if ((grayhorse < 10) && (redhorse < 10))
                {
                    Subject.Text = "You haven't killed enough gray or red horses.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "You've really hopped to it and shown your bunny-licious heroism once again. We're incredibly grateful for your help, and we can't thank you enough. Our warren's crops will be able to grow strong and healthy once again, thanks to you.";
                Subject.NextDialogKey = "bunny_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicBunny.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("grayhorse", out _);
                source.Counters.Remove("redhorse", out _);

                break;
            }

            case "bunny_item":
            {
                Subject.Text = "Don't let us down, Warren Wanderer. We're counting on you to hop to it and bring back the horse hair we need. And remember, the early bunny gets the hair!";
                source.SendOrangeBarMessage("Collect 25 horse hair for Big Bunny");
                source.Enums.Set(MythicBunny.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "bunny_item2":
            {

                if (!source.Inventory.RemoveQuantity("Horse Hair", 25))
                {
                    Subject.Text = "Whatever it takes, we need you to gather more horse hair so that we can build warm and snug beds for all of us. We believe in you, Warren Wanderer. We know you can get the job done.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicBunny.ItemComplete);
                Subject.Text = "You've really hopped to it and brought us enough horse hair to keep us warm and cozy through the long winter nights. This is bunny-tastic news!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "bunny_initial";

                break;
            }

            case "bunny_ally":
            {
                if (hasHorse
                    && (hasHorse == horse is MythicHorse.Allied or MythicHorse.BossStarted or MythicHorse.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Oh no! You already allied with the horses! Get away from us!";
                    source.Enums.Set(MythicBunny.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicBunny.Allied);
                source.SendOrangeBarMessage("You are now allied with the bunnies!");
                Subject.Text = $"Remember, {source.Name}, that no matter where your journeys take you, you will always have a home in our warren. The bunny luck be with you always!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "bunny_initial";

                break;

            }

            case "bunny_boss":
            {
                Subject.Text = "Please be careful, Warren Wanderer. We rabbits are a fragile and gentle species, and we need your help to survive. We'll be eagerly waiting for your return, hoping to hear tales of your bunny-licious bravery and triumph over Apple Jack. May the bunny gods be with you!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicBunny.BossStarted);
                source.SendOrangeBarMessage("Kill Apple Jack atleast three times.");
            }

                break;

            case "bunny_boss2":
            {
                if (!source.Counters.TryGetValue("AppleJack", out var bunnyboss1) || (bunnyboss1 < 3))
                {
                    Subject.Text = "Please rest and recover your strength, and then hop back into action. We'll be here waiting, hoping and praying for your success. The fate of our warren rests on your paws, Warren Wanderer. We're counting on you!";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Apple Jack.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };
                
                Subject.Text = "Your bravery and skill in battle have truly hopped over our expectations. You've gone above and beyond to protect our warren and its inhabitants, and we will forever be grateful for your bunny-tastic efforts.";
                source.Animate(ani2, source.Id);
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Counters.Remove("AppleJack", out _);
                source.Enums.Set(MythicBunny.BossDefeated);
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