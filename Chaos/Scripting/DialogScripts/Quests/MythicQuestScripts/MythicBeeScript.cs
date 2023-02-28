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

public class MythicBeeScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicBeeScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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
            case "bee_initial":
            {
                if (hasBee && (bee == MythicBee.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "I cannot allow a member of the hive to be allied with our enemies. It puts our entire colony at risk, and I simply cannot tolerate it. So, I must ask you to leave our hive and never return.";
                    Subject.NextDialogKey = "Close";
                }
                
                if (hasMain && !hasBee)

                {
                    Subject.Text = "Buzz buzz, welcome to my hive, little one. I am the Bee Queen, ruler of these lands. For centuries, my hive has been at war with the Mantis Colony, those vicious and ruthless hunters. They have no respect for the hard work and sacrifice of my bees. We've been stuck in this sticky situation for far too long. They constantly attack our hive and steal our precious honey, leaving us with little to sustain ourselves.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "bee_start1",
                        OptionText = "Buzz me in Queen."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (bee == MythicBee.Lower)
                {
                    Subject.Text = "Ah, welcome back, my industrious friend! I see that you have returned to our hive, buzzing with the excitement of your latest mission. Your efforts have certainly created quite a buzz around here! Did you get rid of those mantis?";

                    var option = new DialogOption
                    {
                        DialogKey = "bee_lower2",
                        OptionText = "Yes Queen Bee."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (bee == MythicBee.LowerComplete)
                {
                    Subject.Text = "Greetings, my dear buzz-worthy friend! We have identified a congregation of mantis that are a different species from before. They are blocking our access to some crucial flowers deeper in the forest. We simply cannot get to them without your help. ";
                
                    var option = new DialogOption
                    {
                        DialogKey = "bee_start3",
                        OptionText = "What can I do?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (bee == MythicBee.Higher)
                {
                    Subject.Text = "Greetings, my busy bee friend! I'm abuzz with excitement to see you again. I hope your wings aren't too tired from all the flying. Is the area clear for my worker bees? Can we get that sweet nectar?";

                    var option = new DialogOption
                    {
                        DialogKey = "bee_higher2",
                        OptionText = "Yeah, it is clear."
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (bee == MythicBee.HigherComplete)
                {
                    Subject.Text = "I have a new task for you, and it's sure to be a real honey of a mission. We need you to buzz around and collect some dendron flowers for us. The Mantis Colony uses them to lure in my workers, you can probably take them from any of the mantis.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "bee_item",
                        OptionText = "I will get the flowers."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (bee == MythicBee.Item)
                {
                    Subject.Text = "Bee-friend! You're back. Were you able to collect those dendron flowers?";

                    var option = new DialogOption
                    {
                        DialogKey = "bee_item2",
                        OptionText = "I have them here."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (bee == MythicBee.ItemComplete)
                {
                    Subject.Text = "We bees are known for our loyalty and hard work. And we always stick together as a hive. So, the choice is yours: will you be a busy bee and join us in our fight, or will you be a drone and buzz off?\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "bee_ally",
                        OptionText = "Ally with Bee"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "bee_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (bee == MythicBee.Allied)
                {
                    Subject.Text =
                        "We have a crucial mission that only a bee like you can accomplish. The mantis colony is led by a fiery and cunning leader who has been causing all sorts of trouble for us and our hive.";
                    var option = new DialogOption
                    {
                        DialogKey = "bee_start5",
                        OptionText = "Tell me more."
                    };
                    
                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (bee == MythicBee.BossStarted)
                {
                    Subject.Text =
                        "Bee-venturer! Are you okay? Did you find Fire Tree?";

                    var option = new DialogOption
                    {
                        DialogKey = "bee_boss2",
                        OptionText = "I found him, he's gone."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (bee == MythicBee.BossDefeated)
                {

                    Subject.Text = "The whole hive is buzzing about your efforts, you're an honorary bee around here. Thank you again!";
                }

                break;
            }

            case "bee_lower":
            {
                Subject.Text = "I wish you the best of luck on your mission. May your stinger be swift and true, and may you return to our hive victorious. Buzz on, my friend!";
                source.SendOrangeBarMessage("Kill 15 Mythic Mantis for Queen Bee");
                source.Enums.Set(MythicBee.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "bee_lower2":
            {

                if (!source.Counters.TryGetValue("MythicMantis", out var MythicMantis) || (MythicMantis < 15))
                {
                    Subject.Text = "You haven't killed enough Mythic Mantis.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicBee.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("MythicMantis", out _);
                Subject.Text = "Your bravery and dedication have not gone unnoticed. By eliminating some of the mythic mantis, we can now send our workers to gather pollen and nectar without fear of attack.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "bee_initial";

                break;
            }

            case "bee_higher":
            {
                Subject.Text = "Please eliminate 20 Brown Mantis. The nectar from the flowers that they are guarding is among the sweetest we have ever tasted.";
                source.SendOrangeBarMessage("Kill 20 Brown Mantis for the Bee Queen.");
                source.Enums.Set(MythicBee.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "bee_higher2":
            {

                if (!source.Counters.TryGetValue("brownmantis", out var brownmantis) || (brownmantis < 20))
                {
                    Subject.Text = "You haven't killed enough Brown Mantis.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text =
                    "Thank you from the bottom of my queenly heart. You truly are the pollen to our flower. Buzz on, my dear bee protector!";

                Subject.NextDialogKey = "bee_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicBee.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("brownmantis", out _);

            }

                break;

            case "bee_item":
            {
                Subject.Text = "Thank you mighty bee! Fight the mantis colony and collect 25 dendron flowers from them.";
                source.SendOrangeBarMessage("Collect 25 Dendron Flower for the Queen Bee");
                source.Enums.Set(MythicBee.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "bee_item2":
            {

                if (!source.Inventory.RemoveQuantity("Dendron Flower", 25))
                {
                    Subject.Text = "This isn't enough Dendron Flowers to feed the hive, we need more of its sweet nectar.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicBee.ItemComplete);
                Subject.Text = "With these flowers, we will be able to produce the most delicious and nutritious nectar that will keep our hive buzzing with joy and energy. You have truly outdone yourself this time, my dear ally.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "bee_initial";

                break;
            }

            case "bee_ally":
            {
                if (hasMantis
                    && (hasMantis == mantis is MythicMantis.Allied or MythicMantis.BossStarted or MythicMantis.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "No way! You have been allied to the Mantis Colony this entire time traitor! Buzz off!";
                    source.Enums.Set(MythicBee.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicBee.Allied);
                source.SendOrangeBarMessage("You are now allied with the Bees!");
                Subject.Text = $"So let me just say, we're pollen for you {source.Name}! It's bee-n a long time since we've had such a dedicated ally in our fight against the mantis colony. With you on our side, we can bee unstoppable!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "bee_initial";

                break;

            }

            case "bee_boss":
            {
                Subject.Text = "So fly out, my buzz-worthy ally, and bring us victory over Fire Tree!";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicBee.BossStarted);
                source.SendOrangeBarMessage("Kill Fire Tree three times.");
            }

                break;

            case "bee_boss2":
            {
                if (!source.Counters.TryGetValue("FireTree", out var firetree) || (firetree < 3))
                {
                    Subject.Text =  "Oh dear, it seems Fire Tree is still at large. This is troubling news for my hive and my workers. Please be careful, we cannot afford to let the mantis leader continue to harm us. I urge you to finish the task at hand and take down Fire Tree once and for all. The safety and well-being of my hive and all its inhabitants depend on it.";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Fire Tree.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };
                
                Subject.Text =  "Buzzing fantastic! You've done it! You've defeated Fire Tree, the notorious mantis leader. Thank you so much for your dedication and bravery in protecting my hive and my fellow bees. Your efforts will not go unnoticed. You truly are the bee's knees!";
                source.Animate(ani2, source.Id);
                ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                source.Counters.Remove("FireTree", out _);
                source.Enums.Set(MythicBee.BossDefeated);
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