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

public class MythicMantisScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicMantisScript(Dialog subject, IItemFactory itemFactory)
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
            case "mantis_initial":
            {
                if (hasMantis && (mantis == MythicMantis.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasMantis)

                {
                    Subject.Text = " ";
                    
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

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

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
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

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

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

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

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (mantis == MythicMantis.Item)
                {
                    Subject.Text = "Hare-oic Aisling! Did you collect all the horse hair?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_item2",
                        OptionText = "I have them here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

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

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

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

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);    

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
                Subject.Text = " ";
                source.SendOrangeBarMessage("Kill 15 Mythic Bees for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("mythicbee", out var mantislower) || (mantislower < 15))
                {
                    Subject.Text = "You haven't killed enough Mythic Bees";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Trackers.Enums.Set(MythicMantis.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Trackers.Counters.Remove("mythicbee", out _);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;
            }

            case "mantis_higher":
            {
                Subject.Text = "Kill 20 Green Bees";
                source.SendOrangeBarMessage("Kill 20 Green Bees for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_higher2":
            {

                if (!source.Trackers.Counters.TryGetValue("greenbee", out var mantishigher) || (mantishigher < 20))
                {
                    Subject.Text = "You haven't killed enough Green Bees";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = " ";
                Subject.NextDialogKey = "mantis_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicMantis.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Trackers.Counters.Remove("greenbee", out _);

                break;
            }

            case "mantis_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 Mythic Honey for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_item2":
            {

                if (!source.Inventory.RemoveQuantity("Mythic Honey", 25))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicMantis.ItemComplete);
                Subject.Text = " ";
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
                    Subject.Text = " ";
                    source.Trackers.Enums.Set(MythicMantis.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicMantis.Allied);
                source.SendOrangeBarMessage("You are now allied with the Mantises!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;

            }

            case "mantis_boss":
            {
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Trackers.Enums.Set(MythicMantis.BossStarted);
                source.SendOrangeBarMessage("Kill Carolina three times.");
            }

                break;

            case "mantis_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Carolina", out var mantisboss1) || (mantisboss1 < 3))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Carolina.");

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
                source.Trackers.Counters.Remove("carolina", out _);
                source.Trackers.Enums.Set(MythicMantis.BossDefeated);
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