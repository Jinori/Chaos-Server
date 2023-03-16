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

public class MythicMantisScript : DialogScriptBase
{
    private readonly IItemFactory ItemFactory;
    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    /// <inheritdoc />
    public MythicMantisScript(Dialog subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        var hasMain = source.Trackers.Enums.TryGetValue(out MythicQuestMain main);
        var hasMantis = source.Trackers.Enums.TryGetValue(out MythicMantis mantis);
        var hasBee = source.Trackers.Enums.TryGetValue(out MythicBee bee);
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
            case "mantis_initial":
            {
                if (hasMantis && (mantis == MythicMantis.EnemyAllied))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = "Oh lucky us. You're allied with the bees. We'll devour you like the rest of them.";
                    Subject.NextDialogKey = "Close";
                }

                if (hasMain && !hasMantis)

                {
                    Subject.Text = "Welcome to our colony, stranger. I am the Mantis King. What brings you here?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_start1",
                        OptionText = "I have come seeking adventure."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (mantis == MythicMantis.Lower)
                {
                    Subject.Text =
                        "Tell me, have you killed 15 Mythic bees? You know my colony will tell me if you haven't.";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_lower2",
                        OptionText = "They are dead King Mantis."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (mantis == MythicMantis.LowerComplete)
                {
                    Subject.Text =
                        "Can I interest you in another adventure Aisling? This time a little more difficult.";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_start3",
                        OptionText = "I'm always up for adventure."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (mantis == MythicMantis.Higher)
                {
                    Subject.Text = "Was that as fun as it usually is for me? I always enjoy a good massacre. Did you get all 20?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_higher2",
                        OptionText = "Yes it was fun!"
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (mantis == MythicMantis.HigherComplete)
                {
                    Subject.Text = "You are one quick Aisling, I like that. Oh, Hey. Now that those bees are dead, would you mind gathering 25 Mythic Honey for my colony? It's really just a good substance to have. Sometimes we use it to keep our nymphs in place. They like to scatter once they're born, but with a little mythic honey, they'll be there till we get back.";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_item",
                        OptionText = "That's kinda funny. Will do."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                }

                if (mantis == MythicMantis.Item)
                {
                    Subject.Text = "Heh, you're back fast. Did you get all the Mythic Honey? My colony is thrilled we're getting another shipment of this stuff. Where is it?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_item2",
                        OptionText = "Here they are."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;

                }

                if (mantis == MythicMantis.ItemComplete)
                {
                    Subject.Text =
                        $"Speaking of being part of the colony, that's an excellent idea. You've already done what most of us do in a day, how about it? Would you like to become one of our allies {source.Name}? \n((Remember, you may only have up to 5 Alliances and you cannot remove alliances.))";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_ally",
                        OptionText = "Ally with Mantis"
                    };

                    var option1 = new DialogOption
                    {
                        DialogKey = "mantis_no",
                        OptionText = "Not my style."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    if (!Subject.HasOption(option1))
                        Subject.Options.Add(option1);

                    return;

                }

                if (mantis == MythicMantis.Allied)
                {
                    Subject.Text =
                        "That's good we're allies now. I must tell you of the ultimate challenge. I haven't seen this one yet, some say she looks like a wasp but not many lived to report back. Are you interested in another adventure? I don't know if you'll make it back from this one.";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_start5",
                        OptionText = "She can't be that bad."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (mantis == MythicMantis.BossStarted)
                {
                    Subject.Text =
                        "Did you find Apple Jack? Is it done?";

                    var option = new DialogOption
                    {
                        DialogKey = "mantis_boss2",
                        OptionText = "I carried out what was asked of me."
                    };

                    if (!Subject.HasOption(option))
                        Subject.Options.Add(option);

                    return;
                }

                if (mantis == MythicMantis.BossDefeated)
                {

                    Subject.Text = "Thank you again Aisling for your help. We are winning our fight.";
                }

                break;
            }

            case "mantis_lower":
            {
                Subject.Text = "Good, good. I will see you when you return. Remember, go kill 15 Mythic Bees. Don't leave any alive.";
                source.SendOrangeBarMessage("Kill 15 Mythic Bees for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Lower);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_lower2":
            {

                if (!source.Trackers.Counters.TryGetValue("mythicbee", out var mantislower) || (mantislower < 15))
                {
                    Subject.Text = "Disappointing, loyalty is everything and if you aren't willing, don't come back.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                source.Trackers.Enums.Set(MythicMantis.LowerComplete);
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

                source.Trackers.Counters.Remove("mythicbee", out _);
                Subject.Text = "Impressive. 15 Mythic bees without blinking an eye. I admire your loyalty.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;
            }

            case "mantis_higher":
            {
                Subject.Text = "Show me how you do it. Good luck Aisling, they can be awefully tricky to kill. Make sure you slay 20 of the Green Bees.";
                source.SendOrangeBarMessage("Kill 20 Green Bees for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Higher);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_higher2":
            {

                if (!source.Trackers.Counters.TryGetValue("greenbee", out var mantishigher) || (mantishigher < 20))
                {
                    Subject.Text = "Didn't quite finish them all did you? Go back and make sure they're dead.";
                    Subject.Type = MenuOrDialogType.Normal;

                    return;
                }

                Subject.Text = "It's quite exciting to come out on top. They're also very delicious if you're a mantis. Good work.";
                Subject.NextDialogKey = "mantis_initial";
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

                source.Trackers.Enums.Set(MythicMantis.HigherComplete);
                source.Trackers.Counters.Remove("greenbee", out _);

                break;
            }

            case "mantis_item":
            {
                Subject.Text = "Does that make us bad parents? Ah, who cares. Grab us 25 Mythic Honey.";
                source.SendOrangeBarMessage("Collect 25 Mythic Honey for King Mantis");
                source.Trackers.Enums.Set(MythicMantis.Item);
                Subject.Type = MenuOrDialogType.Normal;

                return;
            }

            case "mantis_item2":
            {

                if (!source.Inventory.RemoveQuantity("Mythic Honey", 25))
                {
                    Subject.Text = "Nah, this won't be enough. Go get us some more, there's plenty in there.";
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

                source.Trackers.Enums.Set(MythicMantis.ItemComplete);
                Subject.Text = "Ah, just enough. We could always use more but this will keep us satisfied for some time. Thank you Aisling, you really are becoming part of the colony.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;
            }

            case "mantis_ally":
            {
                if (hasBee
                    && (hasBee == bee is MythicBee.Allied or MythicBee.BossStarted or MythicBee.BossDefeated))
                {
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.Text = " ";
                    source.Trackers.Enums.Set(MythicMantis.EnemyAllied);

                    return;
                }

                source.Trackers.Counters.AddOrIncrement("MythicAllies", 1);
                source.Trackers.Enums.Set(MythicMantis.Allied);
                source.SendOrangeBarMessage("You are now allied with the Mantis!");
                Subject.Text = $" ";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "mantis_initial";

                break;

            }

            case "mantis_boss":
            {
                Subject.Text = "That would be fantastic. Good luck Adventurer, and if I don't see you again, the colony appreciates your loyalty.";
                Subject.Type = MenuOrDialogType.Normal;
                Subject.NextDialogKey = "Close";
                source.Trackers.Enums.Set(MythicMantis.BossStarted);
                source.SendOrangeBarMessage("Kill Carolina three times.");
            }

                break;

            case "mantis_boss2":
            {
                if (!source.Trackers.Counters.TryGetValue("Carolina", out var mantisboss1) || (mantisboss1 < 3))
                {
                    Subject.Text = "Carolina is still out there, please find her and defeat her three times.";
                    Subject.Type = MenuOrDialogType.Normal;
                    Subject.NextDialogKey = "Close";
                    source.SendOrangeBarMessage("You haven't completely defeated Carolina.");

                    return;
                }

                var ani2 = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 21
                };

                Subject.Text = $"I can't believe you did it. That was a miracle! The whole Mantis Colony is talking about your adventures and bravery. You took her down, no problem. We are relieved in the revenge you applied today, thank you {source.Name}!";
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
                source.Trackers.Counters.Remove("carolina", out _);
                source.Trackers.Enums.Set(MythicMantis.BossDefeated);
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