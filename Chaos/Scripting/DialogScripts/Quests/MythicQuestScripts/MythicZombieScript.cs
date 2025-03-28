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

public class MythicZombieScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicZombieScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasZombie = source.Trackers.Enums.TryGetValue(out MythicZombie zombie);
        var hasGargoyle = source.Trackers.Enums.TryGetValue(out MythicGargoyle gargoyle);
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
            case "zombie_initial":
            {
                if (hasZombie && (zombie == MythicZombie.EnemyZombieAllied))
                {
                    Subject.Reply(
                        source,
                        "The friendship you started with the gargoyles is not welcome here, it is bad that you turned your back on us. Leave!");

                    return;
                }

                if (hasMain && !hasZombie)
                {
                    Subject.Reply(source, "skip", "zombie_start1start");

                    return;
                }

                if (zombie == MythicZombie.LowerZombie)
                {
                    Subject.Reply(source, "skip", "Zombie_lower2start");

                    return;
                }

                if (zombie == MythicZombie.LowerZombieComplete)
                {
                    Subject.Reply(source, "skip", "Zombie_start3start");

                    return;
                }

                if (zombie == MythicZombie.HigherZombie)
                {
                    Subject.Reply(source, "skip", "Zombie_higher2start");

                    return;
                }

                if (zombie == MythicZombie.HigherZombieComplete)
                {
                    Subject.Reply(source, "skip", "zombie_itemstart");

                    return;
                }

                if (zombie == MythicZombie.ItemZombie)
                {
                    Subject.Reply(source, "skip", "zombie_item2start");

                    return;
                }

                if (zombie == MythicZombie.ItemZombieComplete)
                {
                    Subject.Reply(source, "skip", "zombie_allystart");

                    return;
                }

                if (zombie == MythicZombie.AlliedZombie)
                {
                    Subject.Reply(source, "skip", "zombie_start5start");

                    return;
                }

                if (zombie == MythicZombie.BossZombieStarted)
                {
                    Subject.Reply(source, "skip", "zombie_boss2start");

                    return;
                }

                if (zombie == MythicZombie.BossZombieDefeated)
                    Subject.Reply(source, "we likess your skillss, you are a great ally to the zombie. Thanks you Aisling!");

                break;
            }

            case "zombie_lower":
            {
                Subject.Reply(source, "Get moooooving. Now where are my braaaaaaainss at... Uuuuurgh.");
                source.SendOrangeBarMessage("Kill 15 Mythic Dunans for the Superior Zombie");
                source.Trackers.Enums.Set(MythicZombie.LowerZombie);

                return;
            }

            case "zombie_lower2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicZombie1", out var zombielower) || (zombielower < 15))
                {
                    Subject.Reply(source, "Uuuurgh! I can still hear them mmooooocking us!");

                    return;
                }

                source.Trackers.Enums.Set(MythicZombie.LowerZombieComplete);
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

                source.Trackers.Counters.Remove("MythicZombie1", out _);

                Subject.Reply(
                    source,
                    "Thas what I like to watch! A goooood smack to them Quirky Dunans does them right! You make us look better when yous fight on our side.");

                break;
            }

            case "zombie_higher":
            {
                Subject.Reply(source, "Rip apart 10 Gargoyle Servants and 10 Guards");
                source.SendOrangeBarMessage("Kill 10 Gargoyle Servants and 10 Guards");
                source.Trackers.Enums.Set(MythicZombie.HigherZombie);

                return;
            }

            case "zombie_higher2":
            {
                source.Trackers.Counters.TryGetValue("MythicZombie2", out var gargoyleservant);
                source.Trackers.Counters.TryGetValue("MythicZombie3", out var gargoyleguard);

                if ((gargoyleservant < 10) || (gargoyleguard < 10))
                {
                    Subject.Reply(source, "I can still hear themmmmmm.");

                    return;
                }

                Subject.Reply(source, "Gooooood job. Soon we will be able to roooooooammm in peace. Uuuurgh!");

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

                source.Trackers.Enums.Set(MythicZombie.HigherZombieComplete);
                source.Trackers.Counters.Remove("MythicZombie2", out _);
                source.Trackers.Counters.Remove("MythicZombie3", out _);

                break;
            }

            case "zombie_item":
            {
                Subject.Reply(source, "Briiing me back 25 of them Dark Flames... we shaaall light our graaveeyards with them!");
                source.SendOrangeBarMessage("Collect 25 Dark Flames for Superior Zombie");
                source.Trackers.Enums.Set(MythicZombie.ItemZombie);

                return;
            }

            case "zombie_item2":
            {
                if (!source.Inventory.RemoveQuantity("Dark Flame", 25))
                {
                    Subject.Reply(
                        source,
                        "We noooo light anythiing with that! There no use for anything less than 25 Dark Flames! Go nooow!");

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

                source.Trackers.Enums.Set(MythicZombie.ItemZombieComplete);
                Subject.Reply(source, "Thisss is what we waanted. You did good Aisling!");

                break;
            }

            case "zombie_ally":
            {
                if (hasGargoyle
                    && (hasGargoyle
                        == gargoyle is MythicGargoyle.AlliedGargoyle
                                       or MythicGargoyle.BossGargoyleStarted
                                       or MythicGargoyle.BossGargoyleDefeated))
                {
                    Subject.Reply(
                        source,
                        "Thiss is not good. You went behind our baaacksss and helped them scummy Gargoylesss! I don't like you anymore! Leave my sightsss.");
                    source.Trackers.Enums.Set(MythicZombie.EnemyZombieAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicZombie.AlliedZombie);
                source.SendOrangeBarMessage("You are now allied with the Zombies!");
                Subject.Reply(source, "Yesss! A gooood ally to our graveyardsss. We are happy to haaave you Aisling.");

                break;
            }

            case "zombie_boss":
            {
                Subject.Reply(source, "That Gargoyle Fiend won't know whaat hit him when you find him! Rip him up Aisling.");
                source.Trackers.Enums.Set(MythicZombie.BossZombieStarted);
                source.SendOrangeBarMessage("Kill Gargoyle Fiend three times.");
            }

                break;

            case "zombie_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("MythicZombie", out var zombieboss1) || (zombieboss1 < 3))
                {
                    Subject.Reply(source, "Thaaat Gargoyle Fiend still flies over my fieldsss! You go show him he can't do that noo more!");

                    source.SendOrangeBarMessage("You haven't defeated Gargoyle Fiend.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Reply(
                    source,
                    "Yeaaahh you showed him whaat he can't do no more! I am grateful for yooour friendship Aisling! That was some gooood fightin.");
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

                source.Trackers.Counters.Remove("MythicZombie", out _);
                source.Trackers.Enums.Set(MythicZombie.BossZombieDefeated);
                source.Trackers.Counters.AddOrIncrement("MythicBoss", 1);

                if (source.Trackers.Counters.TryGetValue("MythicBoss", out var mythicboss)
                    && (mythicboss >= 5)
                    && !source.Trackers.Enums.HasValue(MythicQuestMain.CompletedMythic))
                    source.Trackers.Enums.Set(MythicQuestMain.CompletedAll);
            }

                break;
        }
    }
}