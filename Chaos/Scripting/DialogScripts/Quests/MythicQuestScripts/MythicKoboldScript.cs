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
        var hasMain = source.Enums.TryGetValue(out MythicQuestMain main);
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
            case "kobold_initial":
            {
                if (hasKobold && (kobold == MythicKobold.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasKobold)

                {
                    Subject.Text = "Ears to you, traveler. I am the leader of this warren of bunnies, and I carrot thank you enough for coming to our aid. The neigh-sayers may think we're just cute and fluffy, but we're tougher than we look.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "kobold_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (kobold == MythicKobold.Lower)
                {
                    Subject.Text = "Well, well, well, look who's back! It's our favorite rabbit-loving adventurer! Have you come to tell us that you've completed the task we gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_lower2",
                        OptionText = "Yes Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (kobold == MythicKobold.LowerComplete)
                {
                    Subject.Text = "Warren Wanderer, we are in need of your assistance once again. It seems that another group of horses has invaded our territory and is causing chaos and destruction. We need your help to remove them from our fields, just as you did with the previous group.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "kobold_start3",
                        OptionText = "No problem Big Bunny."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (kobold == MythicKobold.Higher)
                {
                    Subject.Text = "Hoppy Greetings, welcome back. Did you clear those hoofed oppressors?";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_higher2",
                        OptionText = "Yeah, it is done."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                    
                    return;

                }

                if (kobold == MythicKobold.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "kobold_item",
                        OptionText = "I can get that."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (kobold == MythicKobold.Item)
                {
                    Subject.Text = "Hare-oic Aisling! Did you collect all the horse hair?";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_item2",
                        OptionText = "I have them here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (kobold == MythicKobold.ItemComplete)
                {
                    Subject.Text = "You have proven yourself to be a valuable ally to our warren, dear traveler. You have saved our crops, defended our burrows, and defeated many of our enemies. You have shown us that you share our values of kindness and bravery, and for that, we are very grateful. We would be honored if you would consider allying with us, and becoming a part of our family. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_ally",
                        OptionText = "Ally with Bunny"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "kobold_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (kobold == MythicKobold.Allied)
                {
                    Subject.Text =
                        "Warren Wanderer, we have another urgent request for you. We have learned that the leader of the horse herd that has been causing us so much trouble is a powerful and dangerous horse named Apple Jack. We need you to go and defeat Apple Jack three times to ensure that our fields remain safe and secure.";
                    var option = new DialogOption
                    {
                        DialogKey = "kobold_start5",
                        OptionText = "Anything for you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (kobold == MythicKobold.BossStarted)
                {
                    Subject.Text =
                        "Did you find Apple Jack? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (kobold == MythicKobold.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "kobold_lower":
            {
                Subject.Text = "You have our paws-tounding gratitude. Don't let the horses get your goat, though - they're quick and nimble, and they can kick like mules. But we believe in you, and we know you'll do us proud. May the kobold luck be with you!";
                source.SendOrangeBarMessage("Kill 20 Purple Horses for Big Bunny");
                source.Enums.Set(MythicKobold.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "kobold_lower2":
            {

                if (!source.Counters.TryGetValue("grimlockworker", out var grimlockworker) || (grimlockworker < 15))
                {
                    Subject.Text = "You haven't killed enough Grimlock Workers.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicKobold.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("grimlockworker", out _);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "kobold_initial";

                break;
            }

            case "kobold_higher":
            {
                Subject.Text = "Great, clear 10 Grimlock Guards and Grimlock Rogues further rooms and come back to me..";
                source.SendOrangeBarMessage("Kill 10 Grimlock Guards and 10 Grimlock Rogues");
                source.Enums.Set(MythicKobold.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "kobold_higher2":
            {

                source.Counters.TryGetValue("grimlockguard", out var grimlockguard);
                source.Counters.TryGetValue("grimlockrogue", out var grimlockrogue);

                if ((grimlockguard < 10) && (grimlockrogue < 10))
                {
                    Subject.Text = "You haven't killed enough Grimlock Guards and Grimlock Rogues.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "You've really hopped to it and shown your kobold-licious heroism once again. We're incredibly grateful for your help, and we can't thank you enough. Our warren's crops will be able to grow strong and healthy once again, thanks to you.";
                Subject.NextDialogKey = "kobold_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicKobold.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("grimlockguard", out _);
                source.Counters.Remove("grimlockrogue", out _);

                break;
            }

            case "kobold_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 something");
                source.Enums.Set(MythicKobold.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "kobold_item2":
            {

                if (!source.Inventory.RemoveQuantity("Strange Potion", 25))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicKobold.ItemComplete);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "kobold_initial";

                break;
            }

            case "kobold_ally":
            {
                if (hasGrimlock
                    && (hasGrimlock == grimlock is MythicGrimlock.Allied or MythicGrimlock.BossStarted or MythicGrimlock.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Oh no! You already allied with the horses! Get away from us!";
                    source.Enums.Set(MythicKobold.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicKobold.Allied);
                source.SendOrangeBarMessage("You are now allied with the Kobolds!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "kobold_initial";

                break;

            }

            case "kobold_boss":
            {
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicKobold.BossStarted);
                source.SendOrangeBarMessage("Kill Grimlock Princess three times.");
            }

                break;

            case "kobold_boss2":
            {
                if (!source.Counters.TryGetValue("Grimlock Princess", out var koboldboss1) || (koboldboss1 < 3))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated the Grimlock Princess");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };
                
                Subject.Text = " ";
                source.Animate(ani2, source.Id);
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Counters.Remove("BunnyBoss", out _);
                source.Enums.Set(MythicKobold.BossDefeated);
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