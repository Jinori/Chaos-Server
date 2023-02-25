using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.DialogScripts.Quests.MythicQuestScripts;

public class MythicMantisScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicMantisScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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
            case "mantis_initial":
            {
                if (hasMantis && (mantis == MythicMantis.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.";
                    Subject.NextDialogKey = "Close";
                }
                
                if ((main == MythicQuestMain.MythicStarted) && !hasMantis)

                {
                    Subject.Text = "Ears to you, traveler. I am the leader of this warren of bunnies, and I carrot thank you enough for coming to our aid. The neigh-sayers may think we're just cute and fluffy, but we're tougher than we look.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "mantis_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (mantis == MythicMantis.Lower)
                {
                    Subject.Text = "Well, well, well, look who's back! It's our favorite rabbit-loving adventurer! Have you come to tell us that you've completed the task we gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_lower2",
                        OptionText = "Yes Big Bunny."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no1",
                        OptionText = "I'm sorry, not yet."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }
                if (mantis == MythicMantis.LowerComplete)
                {
                    Subject.Text = "Warren Wanderer, we are in need of your assistance once again. It seems that another group of horses has invaded our territory and is causing chaos and destruction. We need your help to remove them from our fields, just as you did with the previous group.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "mantis_start3",
                        OptionText = "No problem Big Bunny."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no",
                        OptionText = "I'm done for now."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (mantis == MythicMantis.Higher)
                {
                    Subject.Text = "Hoppy Greetings, welcome back. Did you clear those hoofed oppressors?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_higher2",
                        OptionText = "Yeah, it is done."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no1",
                        OptionText = "I'm working on it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (mantis == MythicMantis.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "mantis_item",
                        OptionText = "I can get that."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no",
                        OptionText = "Not a chance, good luck."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);
                }

                if (mantis == MythicMantis.Item)
                {
                    Subject.Text = "Hare-oic Aisling! Did you collect all the horse hair?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_item2",
                        OptionText = "I have them here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no1",
                        OptionText = "Still working on it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (mantis == MythicMantis.ItemComplete)
                {
                    Subject.Text = "You have proven yourself to be a valuable ally to our warren, dear traveler. You have saved our crops, defended our burrows, and defeated many of our enemies. You have shown us that you share our values of kindness and bravery, and for that, we are very grateful. We would be honored if you would consider allying with us, and becoming a part of our family. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_ally",
                        OptionText = "Ally with Bunny"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (mantis == MythicMantis.Allied)
                {
                    Subject.Text =
                        "Warren Wanderer, we have another urgent request for you. We have learned that the leader of the horse herd that has been causing us so much trouble is a powerful and dangerous horse named Apple Jack. We need you to go and defeat Apple Jack three times to ensure that our fields remain safe and secure.";
                    var option = new DialogOption
                    {
                        DialogKey = "mantis_start5",
                        OptionText = "Anything for you."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_noboss",
                        OptionText = "I won't do it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;
                }

                if (mantis == MythicMantis.BossStarted)
                {
                    Subject.Text =
                        "Did you find Apple Jack? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_noboss2",
                        OptionText = "I can't do it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;
                }

                if (mantis == MythicMantis.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "mantis_lower":
            {
                Subject.Text = "You have our paws-tounding gratitude. Don't let the horses get your goat, though - they're quick and nimble, and they can kick like mules. But we believe in you, and we know you'll do us proud. May the mantis luck be with you!";
                source.SendOrangeBarMessage("Kill 20 Purple Horses for Big Bunny");
                source.Enums.Set(MythicMantis.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_lower2":
            {

                if (!source.Counters.TryGetValue("BunnyLower", out var mantislower) || (mantislower < 20))
                {
                    Subject.Text = "You haven't killed enough lower horses.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicMantis.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("BunnyLower", out _);
                Subject.Text = "As you can imagine, horses stomping around all day can really cramp a mantis's style. We've got carrots to grow and holes to dig, and we can't do any of that with a bunch of hooves stomping all over the place. Thank you.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;
            }

            case "mantis_higher":
            {
                Subject.Text = "Great, clear 20 horses in the further rooms and come back to me..";
                source.SendOrangeBarMessage("Kill 20 Gray Horses for Big Bunny");
                source.Enums.Set(MythicMantis.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_higher2":
            {

                if (!source.Counters.TryGetValue("BunnyHigher", out var mantishigher) || (mantishigher < 20))
                {
                    Subject.Text = "You haven't killed enough higher horses.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "You've really hopped to it and shown your mantis-licious heroism once again. We're incredibly grateful for your help, and we can't thank you enough. Our warren's crops will be able to grow strong and healthy once again, thanks to you.";
                Subject.NextDialogKey = "mantis_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicMantis.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("BunnyHigher", out _);

                var option = new DialogOption
                {
                    DialogKey = "mantis_item",
                    OptionText = "I can get that."
                };

                var option1 = new DialogOption
                {
                    DialogKey = "mantis_no",
                    OptionText = "Not a chance, good luck."
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Add(option);

                if (!Subject.HasOption(option1))
                    Subject.Options.Add(option1);

                break;
            }

            case "mantis_item":
            {
                Subject.Text = "Don't let us down, Warren Wanderer. We're counting on you to hop to it and bring back the horse hair we need. And remember, the early mantis gets the hair!";
                source.SendOrangeBarMessage("Collect 25 horse hair for Big Bunny");
                source.Enums.Set(MythicMantis.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_item2":
            {

                if (!source.Inventory.RemoveQuantity("Horse Hair", 25))
                {
                    Subject.Text = "Whatever it takes, we need you to gather more horse hair so that we can build warm and snug beds for all of us. We believe in you, Warren Wanderer. We know you can get the job done.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicMantis.ItemComplete);
                Subject.Text = "You've really hopped to it and brought us enough horse hair to keep us warm and cozy through the long winter nights. This is mantis-tastic news!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;
            }

            case "mantis_ally":
            {
                if (hasBee
                    && (hasBee == bee is MythicBee.Allied or MythicBee.BossStarted or MythicBee.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Oh no! You already allied with the horses! Get away from us!";
                    source.Enums.Set(MythicMantis.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicMantis.Allied);
                source.SendOrangeBarMessage("You are now allied with the bunnies!");
                Subject.Text = $"Remember, {source.Name}, that no matter where your journeys take you, you will always have a home in our warren. The mantis luck be with you always!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;

            }

            case "mantis_boss":
            {
                Subject.Text = "Please be careful, Warren Wanderer. We rabbits are a fragile and gentle species, and we need your help to survive. We'll be eagerly waiting for your return, hoping to hear tales of your mantis-licious bravery and triumph over Apple Jack. May the mantis gods be with you!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicMantis.BossStarted);
                source.SendOrangeBarMessage("Kill Apple Jack atleast three times.");
            }

                break;

            case "mantis_boss2":
            {
                if (!source.Counters.TryGetValue("AppleJack", out var mantisboss1) || (mantisboss1 < 3))
                {
                    Subject.Text = "Please rest and recover your strength, and then hop back into action. We'll be here waiting, hoping and praying for your success. The fate of our warren rests on your paws, Warren Wanderer. We're counting on you!";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't killed Apple Jack enough.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };
                
                Subject.Text = "Your bravery and skill in battle have truly hopped over our expectations. You've gone above and beyond to protect our warren and its inhabitants, and we will forever be grateful for your mantis-tastic efforts.";
                source.Animate(ani2, source.Id);
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Counters.Remove("BunnyBoss", out _);
                source.Enums.Set(MythicMantis.BossDefeated);
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