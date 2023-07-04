using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
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
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasGargoyle = source.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle);
        var hasZombie = source.Trackers.Enums.TryGetValue(out MythicZombie zombie);
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
                    Subject.Reply(source, "You would be allied to the zombies, just as braindead as they are.");

                    return;
                }

                if (hasMain && !hasGargoyle)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_start1start");

                    return;
                }

                if (gargoyle == MythicGargoyle.Lower)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_lower2start");

                    return;
                }

                if (gargoyle == MythicGargoyle.LowerComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_start3start");

                    return;
                }

                if (gargoyle == MythicGargoyle.Higher)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_higher2start");

                    return;
                }

                if (gargoyle == MythicGargoyle.HigherComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_itemstart");

                    return;
                }

                if (gargoyle == MythicGargoyle.Item)
                {
                    Subject.Reply(source, "Skip", "gargoyle_item2start");

                    return;
                }

                if (gargoyle == MythicGargoyle.ItemComplete)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_allystart");

                    return;
                }

                if (gargoyle == MythicGargoyle.Allied)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_start5start");

                    return;
                }

                if (gargoyle == MythicGargoyle.BossStarted)
                {
                    Subject.Reply(
                        source,
                        "Skip",
                        "gargoyle_boss2start");

                    return;
                }

                if (gargoyle == MythicGargoyle.BossDefeated)
                    Subject.Reply(
                        source,
                        "Your unwavering support and dedication have brought us to new heights of success in our fight against the undead.");

                break;
            }

            case "gargoyle_lower":
            {
                Subject.Reply(source, "Thank you Aisling. With your help, we can put an end to this zombie infestation once and for all.");
                source.SendOrangeBarMessage("Kill 15 Zombie Grunts for Lord Gargoyle");
                source.Trackers.Enums.Set(MythicGargoyle.Lower);

                return;
            }

            case "gargoyle_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("zombiegrunt", out var zombiegrunt) || (zombiegrunt < 15))
                {
                    Subject.Reply(source, "You haven't killed enough Zombie Grunts.");

                    return;
                }

                source.Trackers.Enums.Set(MythicGargoyle.LowerComplete);
                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Counters.Remove("zombiegrunt", out _);

                Subject.Reply(
                    source,
                    "Your bravery and loyalty to the Gargoyle clan are commendable. It is through the efforts of individuals like you that our clan can continue to thrive.",
                    "gargoyle_initial");

                break;
            }

            case "gargoyle_higher":
            {
                Subject.Reply(
                    source,
                    "Thank you very much, Aisling. Your loyalty to our clan rocks! I know you have the stones to take down those 10 zombie soldiers and 10 zombie lumberjacks.");

                source.SendOrangeBarMessage("Kill 10 Zombie Soldiers and Lumberjacks for Lord Gargoyle");
                source.Trackers.Enums.Set(MythicGargoyle.Higher);

                return;
            }

            case "gargoyle_higher2":
            {
                source.Trackers.Counters.TryGetValue("zombiesoldier", out var zombiesoldier);
                source.Trackers.Counters.TryGetValue("zombiefarmer", out var zombielumberjack);

                if ((zombielumberjack < 10) || (zombiesoldier < 10))
                {
                    Subject.Reply(source, "You haven't killed enough Zombie Soldiers and Zombie Lumberjacks.");

                    source.SendOrangeBarMessage($"You have only killed {zombiesoldier} Soldiers and {zombielumberjack} Farmers.");

                    return;
                }

                Subject.Reply(
                    source,
                    "As we continue to battle against the zombie menace, let us remember the lessons of our past triumphs. Let us spread our wings wide and take to the skies, striking fear into the hearts of our foes and bringing hope to our allies.",
                    "gargoyle_initial");

                source.Animate(ani, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, twentyPercent);
                    source.SendOrangeBarMessage($"You received {twentyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 10000000);
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicGargoyle.HigherComplete);
                source.Trackers.Counters.Remove("zombiesoldier", out _);
                source.Trackers.Counters.Remove("zombielumberjack", out _);

                break;
            }

            case "gargoyle_item":
            {
                Subject.Reply(source, "We require these zombie bones for our ritual soon. Please bring back 25 zombie bones Aisling.");
                source.SendOrangeBarMessage("Collect 25 Zombie Bones for Lord Gargoyle");
                source.Trackers.Enums.Set(MythicGargoyle.Item);

                return;
            }

            case "gargoyle_item2":
            {
                if (!source.Inventory.RemoveQuantity("Zombie Bone", 25))
                {
                    Subject.Reply(
                        source,
                        "I implore you to hurry back with those zombie bones. We have an important ritual tonight that requires the bones, and time is of the essence.");

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
                    source.SendOrangeBarMessage("You received 10000000 experience!");
                }

                source.Trackers.Enums.Set(MythicGargoyle.ItemComplete);

                Subject.Reply(
                    source,
                    "With those bones in our possession, our clan will be able to perform our rituals and ceremonies with renewed vigor and power. You have truly done us a great service, and for that, we are eternally grateful.",
                    "gargoyle_initial");

                break;
            }

            case "gargoyle_ally":
            {
                if (hasZombie
                    && (hasZombie == zombie is MythicZombie.Allied or MythicZombie.BossStarted or MythicZombie.BossDefeated))
                {
                    Subject.Reply(
                        source,
                        "As the Lord Gargoyle, I am deeply disappointed and saddened to hear that you have allied yourself with our undead enemies, the zombies. How could you betray your own kin and stand alongside those who seek to destroy us?");

                    source.Trackers.Enums.Set(MythicGargoyle.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicGargoyle.Allied);
                source.SendOrangeBarMessage("You are now allied with the Gargoyles!");

                Subject.Reply(
                    source,
                    $"{source.Name
                    } your decision to stand with the Gargoyle clan fills me with pride and joy. Together, we shall soar to new heights and vanquish our undead foes once and for all.",
                    "gargoyle_initial");

                break;
            }

            case "gargoyle_boss":
            {
                Subject.Reply(source, "I shall see you when you return.");

                source.Trackers.Enums.Set(MythicGargoyle.BossStarted);
                source.SendOrangeBarMessage("Kill Brains three times.");
            }

                break;

            case "gargoyle_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Brains", out var brains) || (brains < 3))
                {
                    Subject.Reply(
                        source,
                        "Ah, I see. It seems that Brains proved to be a tougher adversary than we anticipated. But do not be disheartened, my friend. Even the greatest warriors can be bested in battle from time to time.");

                    source.SendOrangeBarMessage("You haven't completely defeated Brains.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "Your victory over Brains fills me with pride and joy. It is not often that we are able to triumph over such a formidable foe, but you have proven once again that you are a true champion of the Gargoyle clan.");

                source.Animate(ani2, source.Id);

                if (source.UserStatSheet.Level <= 98)
                {
                    ExperienceDistributionScript.GiveExp(source, fiftyPercent);
                    source.SendOrangeBarMessage($"You received {fiftyPercent} experience!");
                } else
                {
                    ExperienceDistributionScript.GiveExp(source, 25000000);
                    source.SendOrangeBarMessage("You received 25000000 experience!");
                }

                source.Trackers.Counters.Remove("Brains", out _);
                source.Trackers.Enums.Set(MythicGargoyle.BossDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss) && (mythicboss >= 5))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}