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

public class MythicGargoyleScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private readonly ISimpleCache SimpleCache;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicGargoyleScript(Dialog subject, IItemFactory itemFactory, ISimpleCache simpleCache)
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
            case "gargoyle_initial":
            {
                if (hasGargoyle && (gargoyle == MythicGargoyle.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    Subject.NextDialogKey = "Close";
                }
                
                if ((main == MythicQuestMain.MythicStarted) && !hasGargoyle)

                {
                    Subject.Text = "";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_start1",
                        OptionText = "What can I do to help?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (gargoyle == MythicGargoyle.Lower)
                {
                    Subject.Text = "";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_lower2",
                        OptionText = "Yes Big Bunny."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (gargoyle == MythicGargoyle.LowerComplete)
                {
                    Subject.Text = " ";
                
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_start3",
                        OptionText = " "
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (gargoyle == MythicGargoyle.Higher)
                {
                    Subject.Text = " ";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_higher2",
                        OptionText = " "
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (gargoyle == MythicGargoyle.HigherComplete)
                {
                    Subject.Text = "Want to collect some horse hair for me?";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_item",
                        OptionText = "I can get that."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (gargoyle == MythicGargoyle.Item)
                {
                    Subject.Text = " ";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_item2",
                        OptionText = "I have them here."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);
                    

                    return;

                }

                if (gargoyle == MythicGargoyle.ItemComplete)
                {
                    Subject.Text = "You have proven yourself to be a valuable ally to our warren, dear traveler. You have saved our crops, defended our burrows, and defeated many of our enemies. You have shown us that you share our values of kindness and bravery, and for that, we are very grateful. We would be honored if you would consider allying with us, and becoming a part of our family. \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_ally",
                        OptionText = "Ally with Bunny"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "gargoyle_no",
                        OptionText = "No thank you."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (gargoyle == MythicGargoyle.Allied)
                {
                    Subject.Text =
                        "Warren Wanderer, we have another urgent request for you. We have learned that the leader of the horse herd that has been causing us so much trouble is a powerful and dangerous horse named Apple Jack. We need you to go and defeat Apple Jack three times to ensure that our fields remain safe and secure.";
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_start5",
                        OptionText = "Anything for you."
                    };


                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (gargoyle == MythicGargoyle.BossStarted)
                {
                    Subject.Text =
                        "Did you find Apple Jack? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (gargoyle == MythicGargoyle.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "gargoyle_lower":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Kill 15 Zombie Grunts for Lord Gargoyle");
                source.Enums.Set(MythicGargoyle.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "gargoyle_lower2":
            {

                if (!source.Counters.TryGetValue("zombiegrunt", out var zombiegrunt) || (zombiegrunt < 15))
                {
                    Subject.Text = "You haven't killed enough Zombie Grunts.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Enums.Set(MythicGargoyle.LowerComplete);
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("zombiegrunt", out _);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "gargoyle_initial";

                break;
            }

            case "gargoyle_higher":
            {
                Subject.Text = "Great, clear 10 Zombie Soldiers and 10 Zombie Farmers--";
                source.SendOrangeBarMessage("Kill 10 Zombie Soldiers and Farmers for Lord Gargoyle");
                source.Enums.Set(MythicGargoyle.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "gargoyle_higher2":
            {

                source.Counters.TryGetValue("zombiesoldier", out var zombiesoldier);
                source.Counters.TryGetValue("zombiefarmer", out var zombiefarmer);
                

                if ((zombiefarmer < 20) && (zombiesoldier < 20))
                {
                    Subject.Text = "You haven't killed enough Zombie Soldiers and Zombie Farmers.";
                    Subject.Type = MenuOrDialogType.Normal;
                    source.SendOrangeBarMessage($"You have only killed {zombiesoldier} Soldiers and {zombiefarmer} Farmers.");

                    return;
                }

                Subject.Text = " ";
                Subject.NextDialogKey = "gargoyle_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicGargoyle.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("zombiesoldier", out _);
                source.Counters.Remove("zombiefarmer", out _);

                var option = new DialogOption
                {
                    DialogKey = "gargoyle_item",
                    OptionText = "I can get that."
                };
                

                if (!Subject.HasOption(option))
                    Subject.Options.Add(option);

                break;
            }

            case "gargoyle_item":
            {
                Subject.Text = " ";
                source.SendOrangeBarMessage("Collect 25 Zombie Bones for Lord Gargoyle");
                source.Enums.Set(MythicGargoyle.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "gargoyle_item2":
            {

                if (!source.Inventory.RemoveQuantity("Zombie Bone", 25))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicGargoyle.ItemComplete);
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "gargoyle_initial";

                break;
            }

            case "gargoyle_ally":
            {
                if (hasZombie
                    && (hasZombie == zombie is MythicZombie.Allied or MythicZombie.BossStarted or MythicZombie.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    source.Enums.Set(MythicGargoyle.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicGargoyle.Allied);
                source.SendOrangeBarMessage("You are now allied with the Gargoyles!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "gargoyle_initial";

                break;

            }

            case "gargoyle_boss":
            {
                Subject.Text = " ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Enums.Set(MythicGargoyle.BossStarted);
                source.SendOrangeBarMessage("Kill Brains three times.");
            }

                break;

            case "gargoyle_boss2":
            {
                if (!source.Counters.TryGetValue("Brains", out var brains) || (brains < 3))
                {
                    Subject.Text = " ";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Brains.");

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
                source.Counters.Remove("Brains", out _);
                source.Enums.Set(MythicGargoyle.BossDefeated);
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