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

public class MythicGargoyleScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicGargoyleScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Enums.TryGetValue(out MythicQuestMain main);
        var hasGargoyle = source.Enums.TryGetValue(out MythicGargoyle gargoyle);
        var hasZombie = source.Enums.TryGetValue(out MythicZombie zombie);
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
                
                if (hasMain && !hasGargoyle)

                {
                    Subject.Text = "Greetings, puny Aisling. I, Lord Gargoyle, leader of my kind, welcome you. It appears that you have stumbled upon a situation that requires our attention. As you may have noticed, the zombies have been a real pain in our wings for quite some time. They follow us around, and their mindless ways have become a source of irritation to our kind.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_start1",
                        OptionText = "What can I do about it?"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (gargoyle == MythicGargoyle.Lower)
                {
                    Subject.Text = "Ah, welcome back, player! Have you returned victorious in our battle against the zombie menace? I trust that your skills were more than adequate for the task at hand. Tell me, did you enjoy ridding the world of those brainless, shambling creatures?";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_lower2",
                        OptionText = "I did."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }
                if (gargoyle == MythicGargoyle.LowerComplete)
                {
                    Subject.Text = "Ready to take on another mission in service of the Gargoyle clan? Our feud with the zombies continues, and we cannot allow them to encroach upon our territory any further.";
                
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_start3",
                        OptionText = "What now?"
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (gargoyle == MythicGargoyle.Higher)
                {
                    Subject.Text = "Welcome back, Aisling. I trust that your mission was successful? Did you vanquish the Zombie Soldiers and Lumberjacks that were threatening our lands? I hope that their presence will no longer be a problem for us.";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_higher2",
                        OptionText = "They are gone."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (gargoyle == MythicGargoyle.HigherComplete)
                {
                    Subject.Text = "I have yet another task for you to aid us in our ongoing feud with the zombies. We require your assistance in collecting 25 zombie bones. These bones are important for our clan's rituals and ceremonies.";
                    
                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_item",
                        OptionText = "I will collect them."
                    };
                    

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (gargoyle == MythicGargoyle.Item)
                {
                    Subject.Text = "Have you managed to collect the 25 zombie bones we requested?";

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
                    Subject.Text = "Ah, my trusty companion, you have proven yourself time and time again to be a true champion of the Gargoyle clan. Your valor and prowess in battle are truly unmatched, and I believe that you would make an excellent ally to our cause. So I ask you, my winged friend, will you stand with us? Will you pledge your loyalty to the Gargoyle clan and fight alongside us in our battles against the zombies?\n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_ally",
                        OptionText = "Ally with Gargoyle"
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
                        "There is still much work to be done, and our next task is a formidable one. There is a zombie by the name of Brains who has been causing us no small amount of trouble. He is a cunning foe, and his intelligence makes him all the more dangerous.";
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
                        "Ah, my winged ally, you have returned! I trust that your battle with Brains, the cunning zombie, was a success?";

                    var option = new DialogOption
                    {
                        DialogKey = "gargoyle_boss2",
                        OptionText = "Victory indeed."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (gargoyle == MythicGargoyle.BossDefeated)
                {

                    Subject.Text = "Your unwavering support and dedication have brought us to new heights of success in our fight against the undead. ";
                }

                break;
            }

            case "gargoyle_lower":
            {
                Subject.Text = "Thank you Aisling. With your help, we can put an end to this zombie infestation once and for all.";
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
                Subject.Text = "Your bravery and loyalty to the Gargoyle clan are commendable. It is through the efforts of individuals like you that our clan can continue to thrive.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "gargoyle_initial";

                break;
            }

            case "gargoyle_higher":
            {
                Subject.Text = "Thank you very much, Aisling. Your loyalty to our clan rocks! I know you have the stones to take down those 10 zombie soldiers and 10 zombie lumberjacks.";
                source.SendOrangeBarMessage("Kill 10 Zombie Soldiers and Lumberjacks for Lord Gargoyle");
                source.Enums.Set(MythicGargoyle.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "gargoyle_higher2":
            {

                source.Counters.TryGetValue("zombiesoldier", out var zombiesoldier);
                source.Counters.TryGetValue("zombiefarmer", out var zombielumberjack);
                

                if ((zombielumberjack < 10) && (zombiesoldier < 10))
                {
                    Subject.Text = "You haven't killed enough Zombie Soldiers and Zombie Lumberjacks.";
                    Subject.Type = MenuOrDialogType.Normal;
                    source.SendOrangeBarMessage($"You have only killed {zombiesoldier} Soldiers and {zombielumberjack} Farmers.");

                    return;
                }

                Subject.Text = "As we continue to battle against the zombie menace, let us remember the lessons of our past triumphs. Let us spread our wings wide and take to the skies, striking fear into the hearts of our foes and bringing hope to our allies.";
                Subject.NextDialogKey = "gargoyle_initial";
                Subject.Type = MenuOrDialogType.Normal;
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicGargoyle.HigherComplete);
                source.SendOrangeBarMessage($"You've gained {twentyPercent} experience!");
                source.Counters.Remove("zombiesoldier", out _);
                source.Counters.Remove("zombielumberjack", out _);

                break;
            }

            case "gargoyle_item":
            {
                Subject.Text = "We require these zombie bones for our ritual soon. Please bring back 25 zombie bones Aisling.";
                source.SendOrangeBarMessage("Collect 25 Zombie Bones for Lord Gargoyle");
                source.Enums.Set(MythicGargoyle.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "gargoyle_item2":
            {

                if (!source.Inventory.RemoveQuantity("Zombie Bone", 25))
                {
                    Subject.Text = "I implore you to hurry back with those zombie bones. We have an important ritual tonight that requires the bones, and time is of the essence.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }
                
                source.Animate(ani, source.Id);
                ExperienceDistributionScript.GiveExp(source, twentyPercent);
                source.Enums.Set(MythicGargoyle.ItemComplete);
                Subject.Text = "With those bones in our possession, our clan will be able to perform our rituals and ceremonies with renewed vigor and power. You have truly done us a great service, and for that, we are eternally grateful.";
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
                    Subject.Text = "As the Lord Gargoyle, I am deeply disappointed and saddened to hear that you have allied yourself with our undead enemies, the zombies. How could you betray your own kin and stand alongside those who seek to destroy us?";
                    source.Enums.Set(MythicGargoyle.EnemyAllied);

                    return;
                }

                source.Counters.AddOrIncrement("MythicAllies", 1);
                source.Enums.Set(MythicGargoyle.Allied);
                source.SendOrangeBarMessage("You are now allied with the Gargoyles!");
                Subject.Text = $"{source.Name} your decision to stand with the Gargoyle clan fills me with pride and joy. Together, we shall soar to new heights and vanquish our undead foes once and for all.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "gargoyle_initial";

                break;

            }

            case "gargoyle_boss":
            {
                Subject.Text = "I shall see you when you return.";
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
                    Subject.Text = "Ah, I see. It seems that Brains proved to be a tougher adversary than we anticipated. But do not be disheartened, my friend. Even the greatest warriors can be bested in battle from time to time.";
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
                
                Subject.Text = "Your victory over Brains fills me with pride and joy. It is not often that we are able to triumph over such a formidable foe, but you have proven once again that you are a true champion of the Gargoyle clan.";
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