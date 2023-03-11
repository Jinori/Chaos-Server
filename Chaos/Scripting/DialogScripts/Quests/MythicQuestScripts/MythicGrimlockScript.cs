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
                    Subject.Text = "Greetings, adventurer. I am Queen Grimlock, ruler of this land. Our people have been locked in a bitter feud with the vile kobolds for many years. We have lost too many of our own in this endless conflict, and it pains me greatly to see my people suffer.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_start1",
                        OptionText = "Anything I can do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (grimlock == MythicGrimlock.Lower)
                {
                    Subject.Text = "Have you managed to fight off the 15 Worker Kobolds and protect our lands from the devastation they were causing?";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_lower2",
                        OptionText = "Yes Queen Grimlock."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (grimlock == MythicGrimlock.LowerComplete)
                {
                    Subject.Text = "Adventurer, I must warn you that our conflict with the Kobolds has escalated since the disappearance of their Kobold Workers. Their forces have gathered, and they seem to be preparing for a large-scale war. We cannot let this happen.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_start3",
                        OptionText = "What is your plan of action?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (grimlock == MythicGrimlock.Higher)
                {
                    Subject.Text = "Greetings, adventurer. I trust that your mission to defeat the 10 Kobold Soldiers and 10 Kobold Warriors was successful. Your efforts in this battle were crucial to our cause, and we are grateful for your bravery and skill.";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_higher2",
                        OptionText = "Yes, it was."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (grimlock == MythicGrimlock.HigherComplete)
                {
                    Subject.Text = "I have a new task that I would ask of you. It is said that the Kobolds possess a strange potion with unique properties... We aren't too sure what it is used for, but we must have enough to experiment with. They will not share willingly, it must be taken by force. Could you collect 25 of them for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_item",
                        OptionText = "I will for you."
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
                Subject.Text = "Please go kill 15 Kobold Workers, that'll stop them from damaging our land any further and supplying any more wars between us. Your willingness to fight on our behalf is appreciated.";
                source.SendOrangeBarMessage("Kill 15 Kobold Workers.");
                source.Trackers.Enums.Set(MythicGrimlock.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "grimlock_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("koboldworker", out var koboldworker) || (koboldworker < 15))
                {
                    Subject.Text = "You haven't killed enough Kobold Workers";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Trackers.Enums.Set(MythicGrimlock.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Trackers.Counters.Remove("koboldworker", out _);
                Subject.Text = "While the elimination of any living beings is never something to be taken lightly, the Kobold workers posed a genuine threat to our land. Your actions have helped ensure the safety and security of our lands, and for that, we are grateful.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "grimlock_initial";

                break;
            }

            case "grimlock_higher":
            {
                Subject.Text = "---Clear 10 Kobold Workers and 10 Kobold Soldiers---";
                source.SendOrangeBarMessage("Kill 10 Kobold Workers and 10 Kobold Soldiers.");
                source.Trackers.Enums.Set(MythicGrimlock.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "grimlock_higher2":
            {
                source.Trackers.Counters.TryGetValue("koboldworker", out var koboldsoldier);
                source.Trackers.Counters.TryGetValue("koboldsoldier", out var koboldwarrior);

                if ((koboldsoldier < 10) && (koboldwarrior < 10))
                {
                    Subject.Text = "You haven't killed enough Kobold Soldiers and Warriors.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "With the loss of these fighters, the Kobolds will surely be weakened, and their chances of winning this conflict have been significantly reduced. The Grimlocks are a proud people, and we do not forget those who fight alongside us. You have earned our respect and gratitude, and we will remember your deeds for generations to come.";
                Subject.NextDialogKey = "grimlock_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicGrimlock.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Trackers.Counters.Remove("koboldworker", out _);
                source.Trackers.Counters.Remove("koboldsoldier", out _);

                break;
            }

            case "grimlock_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 Kobold Tails for the Grimlock Queen");
                source.Trackers.Enums.Set(MythicGrimlock.Item);
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
                source.Trackers.Enums.Set(MythicGrimlock.ItemComplete);
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
                    source.Trackers.Enums.Set(MythicGrimlock.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicGrimlock.Allied);
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
                source.Trackers.Enums.Set(MythicGrimlock.BossStarted);
                source.SendOrangeBarMessage("Kill Shank three times.");
            }

                break;

            case "grimlock_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Shank", out var shank) || (shank < 3))
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
                source.Trackers.Counters.Remove("shank", out _);
                source.Trackers.Enums.Set(MythicGrimlock.BossDefeated);
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