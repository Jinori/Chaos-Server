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

public class MythicGrimlockScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicGrimlockScript(Dialog subject, IItemFactory itemFactory)
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
            case "grimlock_initial":
            {
                if (hasGrimlock && (grimlock == MythicGrimlock.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with our enemies and that fills me with rabbit-like fear. I cannot trust you to hop on our side again. Please leave our warren.";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasGrimlock)

                {
                    Subject.Text = "Ears to you, traveler. I am the leader of this warren of bunnies, and I carrot thank you enough for coming to our aid. The neigh-sayers may think we're just cute and fluffy, but we're tougher than we look.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (grimlock == MythicGrimlock.Lower)
                {
                    Subject.Text = "Well, well, well, look who's back! It's our favorite rabbit-loving adventurer! Have you come to tell us that you've completed the task we gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_lower2",
                        OptionText = "Yes Big Grimlock."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (grimlock == MythicGrimlock.LowerComplete)
                {
                    Subject.Text = "Warren Wanderer, we are in need of your assistance once again. It seems that another group of horses has invaded our territory and is causing chaos and destruction. We need your help to remove them from our fields, just as you did with the previous group.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_start3",
                        OptionText = "No problem Big Grimlock."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (grimlock == MythicGrimlock.Higher)
                {
                    Subject.Text = "Hoppy Greetings, welcome back. Did you clear those hoofed oppressors?";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_higher2",
                        OptionText = "Yeah, it is done."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (grimlock == MythicGrimlock.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_item",
                        OptionText = "I can get that."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (grimlock == MythicGrimlock.Item)
                {
                    Subject.Text = "Hare-oic Aisling! Did you collect all the horse hair?";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_item2",
                        OptionText = "I have them here."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (grimlock == MythicGrimlock.ItemComplete)
                {
                    Subject.Text = "You have proven yourself to be a valuable ally to our warren, dear traveler. You have saved our crops, defended our burrows, and defeated many of our enemies. You have shown us that you share our values of kindness and bravery, and for that, we are very grateful. We would be honored if you would consider allying with us, and becoming a part of our family. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_ally",
                        OptionText = "Ally with Grimlock"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "grimlock_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (grimlock == MythicGrimlock.Allied)
                {
                    Subject.Text =
                        "Warren Wanderer, we have another urgent request for you. We have learned that the leader of the horse herd that has been causing us so much trouble is a powerful and dangerous horse named Apple Jack. We need you to go and defeat Apple Jack three times to ensure that our fields remain safe and secure.";
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_start5",
                        OptionText = "Anything for you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);


                    return;
                }

                if (grimlock == MythicGrimlock.BossStarted)
                {
                    Subject.Text =
                        "Did you find Shank? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_boss2",
                        OptionText = " "
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (grimlock == MythicGrimlock.BossDefeated)
                {

                    Subject.Text = " ";
                }

                break;
            }

            case "grimlock_lower":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Kill 15 Kobold Workers.");
                source.Enums.Set(MythicGrimlock.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "grimlock_lower2":
            {

                if (!source.Counters.TryGetValue("koboldworker", out var koboldworker) || (koboldworker < 15))
                {
                    Subject.Text = "You haven't killed enough Kobold Workers";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicGrimlock.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("koboldworker", out _);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "grimlock_initial";

                break;
            }

            case "grimlock_higher":
            {
                Subject.Text = "---Clear 10 Kobold Workers and 10 Kobold Soldiers---";
                source.SendOrangeBarMessage("Kill 10 Kobold Workers and 10 Kobold Soldiers.");
                source.Enums.Set(MythicGrimlock.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "grimlock_higher2":
            {

                source.Counters.TryGetValue("koboldworker", out var koboldworker);
                source.Counters.TryGetValue("koboldsoldier", out var koboldsoldier);

                if ((koboldworker < 10) && (koboldsoldier < 10))
                {
                    Subject.Text = "You haven't killed enough Kobold Workers and Soldiers.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = " ";
                Subject.NextDialogKey = "grimlock_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicGrimlock.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("koboldworker", out _);
                source.Counters.Remove("koboldsoldier", out _);

                break;
            }

            case "grimlock_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 Kobold Tails for the Grimlock Queen");
                source.Enums.Set(MythicGrimlock.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "grimlock_item2":
            {

                if (!source.Inventory.RemoveQuantity("Kobold Tail", 25))
                {
                    Subject.Text = "Whatever it takes, we need you to gather more horse hair so that we can build warm and snug beds for all of us. We believe in you, Warren Wanderer. We know you can get the job done.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicGrimlock.ItemComplete);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "grimlock_initial";

                break;
            }

            case "grimlock_ally":
            {
                if (hasKobold
                    && (hasKobold == kobold is MythicKobold.Allied or MythicKobold.BossStarted or MythicKobold.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    source.Enums.Set(MythicGrimlock.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicGrimlock.Allied);
                source.SendOrangeBarMessage("You are now allied with the Grimlocks!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "grimlock_initial";

                break;

            }

            case "grimlock_boss":
            {
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicGrimlock.BossStarted);
                source.SendOrangeBarMessage("Kill Shank three times.");
            }

                break;

            case "grimlock_boss2":
            {
                if (!source.Counters.TryGetValue("Shank", out var shank) || (shank < 3))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Shank.");

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
                source.Counters.Remove("shank", out _);
                source.Enums.Set(MythicGrimlock.BossDefeated);
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