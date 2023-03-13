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
                    Subject.Text = "Your actions have brought shame upon yourself, and you will be remembered as a traitor to my people. May the spirits have mercy on your soul, for we will not. Now go, and do not let us catch you lurking in our tunnels again.";
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
                    Subject.Text = "Welcome back. I trust that your mission to defeat the 10 Kobold Soldiers and 10 Kobold Warriors was successful. Your efforts in this battle were crucial to our cause, and we are grateful for your bravery and skill.";

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
                    Subject.Text = "There is another task I have for you Aisling. This is a bit gruesome however it must be done. We didn't start the war over this however in the midst of the war, we discovered something very special about the Kobolds.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_itemdescription1",
                        OptionText = "What about them?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (grimlock == MythicGrimlock.Item)
                {
                    Subject.Text = "Aisling, did you get those Kobold Tails? We are so excited for the contribution to my people.";

                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_item2",
                        OptionText = "Take them quickly."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (grimlock == MythicGrimlock.ItemComplete)
                {
                    Subject.Text = "I must ask you, adventurer: Would you consider allying with us in our fight against the Kobolds? We are always in need of strong and skilled warriors like yourself to aid us in our struggles.\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

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
                        "Now that we are allied, I can tell you this. Our scouts have reported that a particularly vicious Kobold leader by the name of Shank has been causing us no end of trouble. He has been raiding our supply caches and attacking our patrols, and we believe that he is the key to the Kobolds' recent resurgence.";
                    var option = new DialogOption
                    {
                        DialogKey = "grimlock_start5",
                        OptionText = "That monster."
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
                        OptionText = "Yes, Shank is gone."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (grimlock == MythicGrimlock.BossDefeated)
                {

                    Subject.Text = "My people will never forget your heroic deeds, I must not ask you another favor for now. You have done more than enough for my people, thank you again.";
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
                Subject.Text = "Thank you Aisling, I take it I will hear from you soon.";
                source.SendOrangeBarMessage("Kill 10 Kobold Soldiers and 10 Kobold Warriors.");
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
                Subject.Text = "I am counting on you to bring back those Kobold tails. Your success will send a strong message to our enemies that we will not tolerate their attacks on our land. We also really need those tails.";
                source.SendOrangeBarMessage("Collect 25 Kobold Tails for the Grimlock Queen");
                source.Trackers.Enums.Set(MythicGrimlock.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "grimlock_item2":
            {

                if (!source.Inventory.RemoveQuantity("Kobold Tail", 25))
                {
                    Subject.Text = "This is not enough! My people will blow through that many very quickly, please try harder. We are counting on you.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicGrimlock.ItemComplete);
                Subject.Text = "This is so much! We will be satisfied for weeks! Thank you so much. We use the tails to heal our wounds and they really calm my people down.";
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
                    Subject.Text = "I am outraged to discover that you have allied yourself with our sworn enemies, the Kobolds! How dare you betray us in such a manner! I trusted you to be an honorable and loyal ally, but you have proven yourself to be nothing more than a treacherous liar. I have no choice but to order you to leave our territory at once. You are no longer welcome among our people, and we will not hesitate to defend ourselves against you should you attempt to harm us in any way.";
                    source.Trackers.Enums.Set(MythicGrimlock.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicGrimlock.Allied);
                source.SendOrangeBarMessage("You are now allied with the Grimlocks!");
                Subject.Text = $"I must say that I am impressed with your commitment to our cause. Your dedication to our alliance fills me with pride, and I have no doubt that you will be a valuable addition to our underground family.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "grimlock_initial";

                break;

            }

            case "grimlock_boss":
            {
                Subject.Text = "Oh, Aisling. Remember you must defeat him 3 times! Thank you again for handling this, I don't know what we would do without you.";
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
                    Subject.Text = "Shank is still out there? This is not good. Please take care of him as soon as possible, every second he is on the move, my people are in danger.";
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
                
                Subject.Text = "Shank's Defeated!? That's such great news. I will pass along the word to my scouts, they will rest easy. This should really help us in our conflict, without Shank, the kobold's will fall. Thank you Adventurer, the Grimlock people are in your debt. Come back anytime.";
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