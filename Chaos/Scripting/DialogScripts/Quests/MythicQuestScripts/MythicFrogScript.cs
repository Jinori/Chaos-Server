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

public class MythicFrogScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicFrogScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasFrog = source.Trackers.Enums.TryGetValue(out MythicFrog frog);
        var hasWolf = source.Trackers.Enums.TryGetValue(out MythicWolf wolf);
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
            case "frog_initial":
            {
                if (hasFrog && (frog == MythicFrog.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;

                    Subject.Text =
                        "You have allied yourself with our enemies. You have no honor and have betrayed our trust. Please leave our army at once.";

                    Subject.NextDialogKey = "Close";
                }

                if (hasMain && !hasFrog)

                {
                    Subject.Text =
                        "Hello adventurer, I am the leader of the Frog Army. Croak! We are in a dire situation and we need your help.";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_start1",
                        OptionText = "What seems to be the problem?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (frog == MythicFrog.Lower)
                {
                    Subject.Text = "Ribbit. Have you come to tell me that you've completed the task I gave you?";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_lower2",
                        OptionText = "Yes, I have slain 15 Mythic Wolves."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_no1",
                        OptionText = "I'm sorry, not yet."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (frog == MythicFrog.LowerComplete)
                {
                    Subject.Text = "We have another task that we hope you can help us with.";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_start3",
                        OptionText = "Sure, How can i help?"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_no",
                        OptionText = "I'm done for now."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (frog == MythicFrog.Higher)
                {
                    Subject.Text = "Croak! Welcome back, Did you clear the wolves yet?";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_higher2",
                        OptionText = "Yeah, it is done."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_no1",
                        OptionText = "I'm working on it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (frog == MythicFrog.HigherComplete)
                {
                    Subject.Text =
                        "Ribbit aisling! Are you willing to collect some wolf skin for us? Croak! It would come in handy for armors and other stuff for our army.";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_item",
                        OptionText = "I can get that."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_no",
                        OptionText = "Not a chance, good luck."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);
                }

                if (frog == MythicFrog.Item)
                {
                    Subject.Text = "Ribbit! Did you collect the wolf skin we asked for?";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_item2",
                        OptionText = "I have them here."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_no1",
                        OptionText = "Still working on it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (frog == MythicFrog.ItemComplete)
                {
                    Subject.Text =
                        "You have proven yourself a mighty warrior. Thanks to you we can finally hop in peace, grow our army, and defend ourselves from our enemies. We would be honored if you would consider allying with us, and becoming a powerful part of our army. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_ally",
                        OptionText = "Ally with the Frogs"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (frog == MythicFrog.Allied)
                {
                    Subject.Text =
                        "We have another urgent request for you. Deep in the cave you can find the leader of the wolves, Nymeria. She is the one who is giving the orders to chase us. She finds it very amusing to watch us run in fear. She must be very lonely in that cave to find such joy in others pain. Please stop her at all cost. Defeat Nymeria three times to ensure that she will never mess with us again.";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_start5",
                        OptionText = "I'll stop her at all cost."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_noboss",
                        OptionText = "I won't do it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;
                }

                if (frog == MythicFrog.BossStarted)
                {
                    Subject.Text =
                        "Did you slay Nymeria?";

                    var option = new DialogOption
                    {
                        DialogKey = "frog_boss2",
                        OptionText = "I've slain the beast."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "frog_noboss2",
                        OptionText = "I can't do it."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;
                }

                if (frog == MythicFrog.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. You will always be part of our army.";
                }

                break;
            }

            case "frog_lower":
            {
                Subject.Text = "Thank you, adventurer. We appreciate your assistance. Good luck on your quest.";
                source.SendOrangeBarMessage("Kill 15 Mythic Wolves for the Frog King.");
                source.Trackers.Enums.Set(MythicFrog.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "frog_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("Mythicwolf", out var mythicwolf) || (mythicwolf < 15))
                {
                    Subject.Text = "You haven't killed enough Mythic Wolves.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Trackers.Enums.Set(MythicFrog.LowerComplete);
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

                source.Trackers.Counters.Remove("MythicWolf", out _);

                Subject.Text =
                    "Excellent work, adventurer! You have proven yourself to our army. Now, it is time to deal with the stronger wolves that have been causing us even more trouble.";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "frog_initial";

                break;
            }

            case "frog_higher":
            {
                Subject.Text =
                    "Remember to slay 10 of each of the stronger wolves, the White Wolf and the Bearded Wolf. They are more powerful and cunning than the ones you faced before, so be careful.";

                source.SendOrangeBarMessage("Kill 10 White Wolves and 10 Bearded Wolves.");
                source.Trackers.Enums.Set(MythicFrog.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "frog_higher2":
            {
                source.Trackers.Counters.TryGetValue("WhiteWolf", out var whitewolf);
                source.Trackers.Counters.TryGetValue("BeardedWolf", out var beardedwolf);

                if ((whitewolf < 10) || (beardedwolf < 10))
                {
                    Subject.Text = "You haven't killed enough White Wolves and Bearded Wolves.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "Croak! You've really proven you could be a powerful ally.";
                Subject.NextDialogKey = "frog_initial";
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

                source.Trackers.Enums.Set(MythicFrog.HigherComplete);
                source.Trackers.Counters.Remove("WhiteWolf", out _);
                source.Trackers.Counters.Remove("BeardedWolf", out _);

                var option = new DialogOption
                {
                    DialogKey = "frog_item",
                    OptionText = "I can get that."
                };

                var option1 = new DialogOption
                {
                    DialogKey = "frog_no",
                    OptionText = "Not a chance, good luck."
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Add(option);

                if (!Subject.HasOption(option1))
                    Subject.Options.Add(option1);

                break;
            }

            case "frog_item":
            {
                Subject.Text =
                    "Please be careful, we don't want you to ribbit the wrong way into danger. They may be more aggressive now that their pack has been weakened. Return to us once you have collected the wolf skins.";

                source.SendOrangeBarMessage("Collect 25 Wolf Skin for the Frog King");
                source.Trackers.Enums.Set(MythicFrog.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "frog_item2":
            {

                if (!source.Inventory.RemoveQuantity("Wolf Skin", 25))
                {
                    Subject.Text =
                        "We need 25 wolf skins to make some protective clothing for our frog army. Croak! We understand that this may be a difficult task, but we believe that you are more than capable of accomplishing it.";

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

                source.Trackers.Enums.Set(MythicFrog.ItemComplete);

                Subject.Text =
                    "Excellent work, adventurer. You continue to prove yourself as a valuable friend to our army. These skins will be very useful for our army's survival.";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "frog_initial";

                break;
            }

            case "frog_ally":
            {
                if (hasWolf
                    && (hasWolf == wolf is MythicWolf.Allied or MythicWolf.BossStarted or MythicWolf.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Ribbit! It seems you already allied with the Wolves! Go away!";
                    source.Trackers.Enums.Set(MythicFrog.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicFrog.Allied);
                source.SendOrangeBarMessage("You are now allied with the Frogs!");

                Subject.Text = $"Remember, {source.Name
                }, no matter where your journeys take you, you will always have a home within our army!";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "frog_initial";

                break;

            }

            case "frog_boss":
            {
                Subject.Text =
                    "Please be careful, Nymeria is a fierce beast and is not to be taken lightly. We need your help to finally be rid of these beast . We'll be waiting for your return, may the frog be with you!";

                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Trackers.Enums.Set(MythicFrog.BossStarted);
                source.SendOrangeBarMessage("Kill Nymeria at least three times.");
            }

                break;

            case "frog_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Nymeria", out var frogboss1) || (frogboss1 < 3))
                {
                    Subject.Text = "Please rest and recover your strength. We're all cheering for you!";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't killed Nymeria enough times.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Text =
                    "Your skill in battle have truly croaked over our expectations. You have proven yourself to our army, and we will forever be grateful for you. Croak!";

                source.Animate(ani2, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage($"You received 25000000 experience!");
                }

                source.Trackers.Counters.Remove("FrogBoss", out _);
                source.Trackers.Enums.Set(MythicFrog.BossDefeated);
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