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

public class MythicWolfScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicWolfScript(Dialog subject, IItemFactory itemFactory)
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
            case "wolf_initial":
            {
                if (hasWolf && (wolf == MythicWolf.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You have allied yourself with the frogs. You can never be one with the pack. Get lost.";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasWolf)

                {
                    Subject.Text = "The wolf leader steps forward and greets you with a low growl, its piercing eyes fixed on you. Hello aisling, what do you want?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_start1",
                        OptionText = "I came to see if you needed some help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (wolf == MythicWolf.Lower)
                {
                    Subject.Text = "Well look who it is. Were you able to take out the Green Frogs?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_lower2",
                        OptionText = "Yeap. They don't seem too dangerous to me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (wolf == MythicWolf.LowerComplete)
                {
                    Subject.Text = "Good job taking out the green frogs, however that was just the beginning. Deeper in the swamp you can find red and blue frogs. They are even more poisonous then the green ones. Are you willing to help?";
                
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_start3",
                        OptionText = "Sure, how can I help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (wolf == MythicWolf.Higher)
                {
                    Subject.Text = "Have you killed the red and blue frogs yet?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_higher2",
                        OptionText = "Yeah, it is done."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (wolf == MythicWolf.HigherComplete)
                {
                    Subject.Text = "You seem to be unaffected by the frogs poison. How could this be? Nevermind for now... We could use your help once again. Since you seem to be immune to the frogs, could you bring us back 25 frog meat? Our food supply has been running low.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_item",
                        OptionText = "Just 25? No problem."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (wolf == MythicWolf.Item)
                {
                    Subject.Text = "Do you have the frog meat?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_item2",
                        OptionText = "Here you go."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (wolf == MythicWolf.ItemComplete)
                {
                    Subject.Text = "You have earned our respect fearless one. Would you consider allying with us? You would fit in well with our wolf pack. *The Wolf Pack Leader lets of a fierce howl you can hear echo in the distance*\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_ally",
                        OptionText = "Ally with Wolves."
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "wolf_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (wolf == MythicWolf.Allied)
                {
                    Subject.Text =
                        "Welcome back fearless one! Thank you again for bringing honor to our wolf pack!";
                    var option = new DialogOption
                    {
                        DialogKey = "wolf_start5",
                        OptionText = "Anything for the pack."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (wolf == MythicWolf.BossStarted)
                {
                    Subject.Text =
                        "Have you killed Frogger yet?";

                    var option = new DialogOption
                    {
                        DialogKey = "wolf_boss2",
                        OptionText = "Frogger hops no more."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (wolf == MythicWolf.BossDefeated)
                {

                    Subject.Text = "Hello fearless one! Thank you for all you have done. I have no other task for you.";
                }

                break;
            }

            case "wolf_lower":
            {
                Subject.Text = "Stay safe, friend.";
                source.SendOrangeBarMessage("Kill 15 Green Frogs for the Wolf Pack Leader");
                source.Trackers.Enums.Set(MythicWolf.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "wolf_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("mythicfrog", out var wolflower) || (wolflower < 15))
                {
                    Subject.Text = "You haven't killed enough Green Frogs";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Trackers.Enums.Set(MythicWolf.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Trackers.Counters.Remove("mythicfrog", out _);
                Subject.Text = "Ha! You seem brave. Please come back when you can, I have another task for you.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "wolf_initial";

                break;
            }

            case "wolf_higher":
            {
                Subject.Text = "You seem to have no fear. You are either very brave or just plain out stupid. Either way you have earned my respect. Please come back to me once the task is complete.";
                source.SendOrangeBarMessage("Kill 10 Blue and 10 Red Frogs for Wolf Pack Leader.");
                source.Trackers.Enums.Set(MythicWolf.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "wolf_higher2":
            {
                source.Trackers.Counters.TryGetValue("bluefrog", out var bluefrog);
                source.Trackers.Counters.TryGetValue("redfrog", out var redfrog);

                if ((bluefrog < 10) || (redfrog < 10))
                {
                    Subject.Text = "You haven't killed enough Blue and Red Frogs.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "Thanks for the help once again. You have earned our respect. I have another task for you when you are ready.";
                Subject.NextDialogKey = "wolf_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicWolf.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Trackers.Counters.Remove("redfrog", out _);
                source.Trackers.Counters.Remove("bluefrog", out _);

                var option = new DialogOption
                {
                    DialogKey = "wolf_item",
                    OptionText = "I can get that."
                };

                if (!Subject.HasOption(option))
                    Subject.Options.Add(option);

                break;
            }

            case "wolf_item":
            {
                Subject.Text = "Hurry back, the other wolves are starting to get very hangry.";
                source.SendOrangeBarMessage("Collect 25 Frog Meat for Wolf Pack Leader");
                source.Trackers.Enums.Set(MythicWolf.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "wolf_item2":
            {

                if (!source.Inventory.RemoveQuantity("Frog Meat", 25))
                {
                    Subject.Text = "You do not have enough Frog Meat.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Trackers.Enums.Set(MythicWolf.ItemComplete);
                Subject.Text = "Thank you. I was starting to get a headache from all the howling. This should be enough to feed us for awhile. Please come back to see me when you can. ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "wolf_initial";

                break;
            }

            case "wolf_ally":
            {
                if (hasFrog
                    && (hasFrog == frog is MythicFrog.Allied or MythicFrog.BossStarted or MythicFrog.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "You are already allied with the Frogs? *The wolf pack leader begins to growl* Begone!";
                    source.Trackers.Enums.Set(MythicWolf.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicWolf.Allied);
                source.SendOrangeBarMessage("You are now allied with the Wolves!");
                Subject.Text = $"Wise choice friend! Welcome to the wolf pack!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "wolf_initial";

                break;

            }

            case "wolf_boss":
            {
                Subject.Text = "That's the spirit! We will be here awaiting your return.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Trackers.Enums.Set(MythicWolf.BossStarted);
                source.SendOrangeBarMessage("Kill Frogger three times.");
            }

                break;

            case "wolf_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Frogger", out var wolfboss1) || (wolfboss1 < 3))
                {
                    Subject.Text = "Frogger's army is still there!";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Frogger.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };
                
                Subject.Text = "What a victory! Frogger and his army are no more. Thank you for all the help fearless one.";
                source.Animate(ani2, source.Id);
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Trackers.Counters.Remove("frogger", out _);
                source.Trackers.Enums.Set(MythicWolf.BossDefeated);
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