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
            case "kobold_initial":
            {
                if (hasKobold && (kobold == MythicKobold.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;

                    Subject.Text =
                        "I told you to get lost, do not make me use these claws. I am still so angry at you. They steal our land and now our allies. (Kobold Leader growls in anger)";

                    Subject.NextDialogKey = "Close";
                }

                if (hasMain && !hasKobold)

                {
                    Subject.Text =
                        "I, as the Kobold leader, have long held animosity towards the Grimlocks. They have taken our land, which was rightfully ours. We once roamed freely on this land, but now we are outsiders on our own territory. The Grimlocks have proven to be aggressive and have repeatedly attacked us, even when we seek peaceful coexistence.";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_start1",
                        OptionText = "That's terrible."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (kobold == MythicKobold.Lower)
                {
                    Subject.Text = "Are the Grimlock Workers cleared? My people are already in position.";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_lower2",
                        OptionText = "They should be distracted now."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (kobold == MythicKobold.LowerComplete)
                {
                    Subject.Text =
                        "I fear that the Grimlocks will not take the loss of their workers lightly. Their guards and rogues have now become a serious threat to our safety, and we cannot afford to let them roam free.";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_start3",
                        OptionText = "What is your plan?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (kobold == MythicKobold.Higher)
                {
                    Subject.Text = "Their forces seem to be really riled up. Did you kill the 10 Grimlock Guards and 10 Grimlock Rogues?";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_higher2",
                        OptionText = "Yes, it's done."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (kobold == MythicKobold.HigherComplete)
                {
                    Subject.Text =
                        "There is another task I must ask of you, adventurer. It has come to our attention that the Grimlocks carry around a strange potion that we believe contains a chemical only found in abundance on that land that used to be ours.";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_itemdescription1",
                        OptionText = "Is it important?"
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
                    Subject.Text =
                        "Adventurer, I would like to extend an offer of alliance to you. We Kobolds are always looking for strong allies who share our values of freedom and self-determination. We may be small in number, but we are fierce and resilient, and we will stand with you in times of need. Will you accept our offer of alliance, adventurer? \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_ally",
                        OptionText = "Ally with Kobold"
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
                        "I have some important information to share with you. Our Kobold brothers have uncovered evidence of a Grimlock princess who is behind a mastermind plan to crush our people once and for all.";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_start5",
                        OptionText = "I am here for you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (kobold == MythicKobold.BossStarted)
                {
                    Subject.Text =
                        "We are eager to know the outcome of your mission, did you find the Grimlock Princess? Is she destroyed?";

                    var option = new DialogOption
                    {
                        DialogKey = "kobold_boss2",
                        OptionText = "I took care of her."
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
                Subject.Text =
                    "My, thank you Aisling. You don't know what it'll mean for us, but we really need this to happen. Kill 15 Grimlock Workers and come back, I'll let my people know the plan.";

                source.SendOrangeBarMessage("Kill 15 Grimlock Workers for Kobold Leader");
                source.Trackers.Enums.Set(MythicKobold.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "kobold_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("grimlockworker", out var grimlockworker) || (grimlockworker < 15))
                {
                    Subject.Text = "You haven't killed enough Grimlock Workers, they are still in the area. It isn't enough.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Trackers.Enums.Set(MythicKobold.LowerComplete);
                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Counters.Remove("grimlockworker", out _);

                Subject.Text =
                    "Your actions have caused a major distraction for the Grimlocks, giving us the opportunity to safely farm resources on our old land, which is crucial for our survival. Thank you.";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "kobold_initial";

                break;
            }

            case "kobold_higher":
            {
                Subject.Text =
                    "You will? That is great to hear, the grimlocks won't know what's coming. My people will be safe with your help. You need to kill 10 Grimlock Guards and 10 Grimlock Rogues.";

                source.SendOrangeBarMessage("Kill 10 Grimlock Guards and 10 Grimlock Rogues");
                source.Trackers.Enums.Set(MythicKobold.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "kobold_higher2":
            {

                source.Trackers.Counters.TryGetValue("grimlockguard", out var grimlockguard);
                source.Trackers.Counters.TryGetValue("grimlockrogue", out var grimlockrogue);

                if ((grimlockguard < 10) || (grimlockrogue < 10))
                {
                    Subject.Text = "You haven't killed enough Grimlock Guards and Grimlock Rogues.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text =
                    "That's great news. They are pretty riled up about it and will probably retreat for now. Thank you Aisling, my people will hear about your impressive abilities for many moons.";

                Subject.NextDialogKey = "kobold_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicKobold.HigherComplete);
                source.Trackers.Counters.Remove("grimlockguard", out _);
                source.Trackers.Counters.Remove("grimlockrogue", out _);

                break;
            }

            case "kobold_item":
            {
                Subject.Text =
                    "That will be fantastic Aisling, I can't wait to have it once again. You should've seen me in my prime, my hair would shine like the sun with just a few drops of this potion. Please, be sure to grab 25! For all of us.";

                source.SendOrangeBarMessage("Collect 25 Strange Potion");
                source.Trackers.Enums.Set(MythicKobold.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "kobold_item2":
            {

                if (!source.Inventory.RemoveQuantity("Strange Potion", 25))
                {
                    Subject.Text = "This won't be enough. Please get us some more.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage($"You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicKobold.ItemComplete);

                Subject.Text =
                    "Perfect! I cannot wait to use this stuff! I missed it so much. My people will be thrilled. Thank you Aisling, this is just short of a miracle.";

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

                    Subject.Text =
                        "No way! You have been allied to the Grimlocks this entire time!? I was so blind in need that I didn't see the traitor before me. I will gouge your eyes out with my new sharp claws. Go far away from me.";

                    source.Trackers.Enums.Set(MythicKobold.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicKobold.Allied);
                source.SendOrangeBarMessage("You are now allied with the Kobolds!");

                Subject.Text =
                    $"We Kobolds are not always trusted by other races, but your actions have shown us that there are still those who are willing to stand with us.";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "kobold_initial";

                break;

            }

            case "kobold_boss":
            {
                Subject.Text =
                    "We owe you for everything but this is the utmost important. My people shall be safe once you defeat the Grimlock Princess, remember you need to defeat her three times.";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Trackers.Enums.Set(MythicKobold.BossStarted);
                source.SendOrangeBarMessage("Kill Grimlock Princess three times.");
            }

                break;

            case "kobold_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Grimlock Princess", out var koboldboss1) || (koboldboss1 < 3))
                {
                    Subject.Text = "She is still out there, I can smell her.";
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

                Subject.Text =
                    "We Kobolds are grateful to you for your bravery and skill in defeating the Grimlock princess. You have shown great honor and courage, and we will always remember your deeds. Our people are now safe thanks to your actions, and we are forever in your debt.";

                source.Animate(ani2, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                }

                source.Trackers.Counters.Remove("Grimlock Princess", out _);
                source.Trackers.Enums.Set(MythicKobold.BossDefeated);
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