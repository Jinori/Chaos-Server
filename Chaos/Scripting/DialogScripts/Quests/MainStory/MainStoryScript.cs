using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStoryScript(
    Dialog subject,
    IItemFactory itemFactory,
    IMerchantFactory merchantFactory,
    ISimpleCache simpleCache,
    IEffectFactory effectFactory,
    ISpellFactory spellFactory,
    IDialogFactory dialogFactory,
    IClientRegistry<IChaosWorldClient> clientRegistry) : DialogScriptBase(subject)
{
    protected IClientRegistry<IChaosWorldClient> ClientRegistry { get; } = clientRegistry;
    private IExperienceDistributionScript ExperienceDistributionScript { get; } = DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var masterWeapon = new Dictionary<string, List<string>>
        {
            {
                "kalkuri", [
                               "enchantedkalkuri",
                               "empoweredkalkuri"
                           ]
            },
            {
                "holyhybrasylgnarl", [
                                         "enchantedholygnarl",
                                         "empoweredholygnarl"
                                     ]
            },
            {
                "hybrasylazoth", [
                                     "enchantedhybrasylazoth",
                                     "empoweredhybrasylazoth"
                                 ]
            },
            {
                "magusorb", [
                                "enchantedmagusorb",
                                "empoweredmagusorb"
                            ]
            },
            {
                "hybrasylescalon", [
                                       "enchantedhybrasylescalon",
                                       "empoweredhybrasylescalon"
                                   ]
            }
        };

        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region MysteriousArtifact
            case "mysteriousartifact_initial1":
            {
                if (source.Trackers.Enums.HasValue(MainStoryEnums.ReceivedMA))
                {
                    if (source.UserStatSheet.Level < 11)
                    {
                        Subject.Close(source);
                        source.SendOrangeBarMessage("You are too inexperienced to interact with this object.");

                        return;
                    }

                    var option = new DialogOption
                    {
                        DialogKey = "mysteriousartifact_start1",
                        OptionText = "Listen closely to the Artifact"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                    Subject.Reply(source, "Skip", "mysteriousartifact_initial2");
            }

                break;
            case "zephyr_start1":
            {
                var point = new Point(source.X, source.Y);
                var zephyr = merchantFactory.Create("zephyr", source.MapInstance, point);
                var zephyrDialog = dialogFactory.Create("zephyr_initial", zephyr);
                zephyrDialog.Display(source);
            }

                break;

            case "zephyr_initial5":
            {
                source.Trackers.Enums.Set(MainStoryEnums.SpokeToZephyr);
            }

                break;
            #endregion

            #region EnterExitRealm
            case "miraelis_temple_initial":
            case "serendael_temple_initial":
            case "skandara_temple_initial":
            case "theselene_temple_initial":
            {
                if ((Subject.DialogSource.Name != "Miraelis")
                    && (Subject.DialogSource.Name != "Theselene")
                    && (Subject.DialogSource.Name != "Skandara")
                    && (Subject.DialogSource.Name != "Serendael"))
                    return;

                if (source.Inventory.Contains("Mysterious Artifact") && source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mysteriousartifact_start2",
                        OptionText = "Mysterious Artifact"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Flags.HasFlag(MainstoryFlags.AccessGodsRealm))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mysteriousartifact_enterrealm",
                        OptionText = "The God's Realm"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                break;
            }

            case "mysteriousartifact_start2":
            {
                if (source.UserStatSheet.Level < 41)
                {
                    Subject.Reply(
                        source,
                        "That is an impressive artifact and we await your news but you are too weak to visit The God's Realm. Come back when you are stronger Aisling.",
                        "Close");
                    source.SendOrangeBarMessage("You are not ready to face the Gods.");
                }
            }

                break;

            case "mysteriousartifact_enterrealm2":
            {
                var animation = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 5
                };

                Subject.Close(source);
                var godsrealm = simpleCache.Get<MapInstance>("godsrealm");
                var newPoint = new Point(16, 16);

                source.TraverseMap(godsrealm, newPoint);
                source.Animate(animation);

                return;
            }

            case "mainstory_miraelis_leaverealm":
                const Direction DIRECTION = Direction.Down;

            {
                var leaverealm = simpleCache.Get<MapInstance>("mileth_inn");
                var newPoint = new Point(5, 8);

                source.TraverseMap(leaverealm, newPoint);
                source.Walk(DIRECTION);
                source.SendOrangeBarMessage("You wake up from a strange dream in Mileth Inn");

                return;
            }
            #endregion

            #region Miraelis
            case "mainstory_miraelis_initial":
            {
                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedFourthTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_summonerinitial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(CombatTrial.FinishedTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_finishedtrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.CompletedArtifactsHunt))
                {
                    if (source.UserStatSheet.Level < 71)
                    {
                        Subject.Reply(source, "You are too young to start the trials. Return to me when you are stronger.");

                        return;
                    }

                    Subject.Reply(source, "Skip", "mainstory_miraelis_starttrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedFirstTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_retrytrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedSecondTrial))
                {
                    Subject.Reply(source, "I see you are trying the Trial of Luck, speak to Serendael to try again.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedThirdTrial))
                {
                    Subject.Reply(source, "I see you are trying the Trial of Intelligence, speak to Theselene to try again.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedFourthTrial))
                {
                    Subject.Reply(source, "I see you are trying the Trial of Sacrifice, speak to Skandara to try again.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedFirstTrial))
                {
                    Subject.Reply(source, "Speak to Serendael about the Trial of Luck.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedSecondTrial))
                {
                    Subject.Reply(source, "Speak to Theselene about the Trial of Intelligence.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedThirdTrial))
                {
                    Subject.Reply(source, "Speak to Skandara about the Trial of Sacrifice.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_assembleartifacts1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedAssemble))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_assemblereturn");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_return");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact1)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_finisheda1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                    Subject.Reply(source, "Skip", "mainstory_miraelis_initial1");

                break;
            }

            case "mainstory_miraelis_summonerinitial1":
            {
                source.Trackers.Enums.Set(MainStoryEnums.CompletedTrials);

                return;
            }

            case "mainstory_miraelis_starttrial4":
            {
                Subject.Close(source);

                source.Trackers.Enums.Set(CombatTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedFirstTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofcombat");
                var point = new Point(15, 15);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_miraelis_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("combattrialcd", out var cdtime))
                    Subject.Reply(
                        source,
                        $"You have recently tried the Combat Trial. You can try again in {cdtime.Remaining.ToReadableString()}");

                break;
            }

            case "mainstory_miraelis_retrytrial2":
            {
                Subject.Close(source);

                source.Trackers.Enums.Set(CombatTrial.StartedTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofcombat");
                var point = new Point(15, 15);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_miraelis_finishedtrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedFirstTrial);
                ExperienceDistributionScript.GiveExp(source, 200000);
                source.TryGiveGamePoints(5);
                source.Trackers.Enums.Remove<CombatTrial>();

                return;
            }

            case "mainstory_miraelis_initial10":
            {
                source.Trackers.Flags.AddFlag(MainstoryFlags.AccessGodsRealm);
                source.Trackers.Enums.Set(MainStoryEnums.StartedArtifact1);
                source.Inventory.Remove("Mysterious Artifact");
            }

                break;

            case "mainstory_miraelis_return2":
            {
                if (!source.Inventory.HasCount("Earth Artifact", 1))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_returnno");
                    source.SendOrangeBarMessage("You are missing the Earth Artifact.");

                    return;
                }

                var animate = new Animation
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 21
                };
                source.Animate(animate);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedArtifact1);
                source.Trackers.Flags.AddFlag(MainstoryFlags.CompletedArtifact1);
                source.Inventory.Remove("Earth Artifact");
                ExperienceDistributionScript.GiveExp(source, 75000);
                source.SendOrangeBarMessage("You hand over the Earth Artifact.");

                return;
            }
            case "mainstory_miraelis_return4":
            {
                source.SendOrangeBarMessage("Speak to Goddess Theselene about the Fire Artifact");

                return;
            }
            case "mainstory_miraelis_assembleartifacts2":
            {
                source.Trackers.Enums.Set(MainStoryEnums.StartedAssemble);

                return;
            }
            case "mainstory_miraelis_assemblereturn2":
            {
                if (!source.Inventory.HasCount("coal", 50) || !source.Inventory.HasCount("Ruined Iron", 20))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_assemblereturnno");
                    source.SendOrangeBarMessage("You are missing the fifty coal or twenty ruined iron.");

                    return;
                }

                var animate = new Animation
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 937
                };

                var miraelis = Subject.DialogSource as Merchant;
                miraelis!.Animate(animate);

                source.Trackers.Enums.Set(MainStoryEnums.CompletedArtifactsHunt);
                source.Inventory.RemoveQuantity("coal", 50);
                source.Inventory.RemoveQuantity("Ruined Iron", 20);
                ExperienceDistributionScript.GiveExp(source, 125000);
                source.SendOrangeBarMessage("You hand over the coal and ruined iron.");

                return;
            }
            #endregion

            #region Serendael
            case "mainstory_serendael_initial":
            {
                if (source.Trackers.Enums.HasValue(LuckTrial.CompletedTrial))
                {
                    source.Trackers.Enums.Set(LuckTrial.StartedTrial);
                    source.Trackers.Enums.Set(MainStoryEnums.StartedSecondTrial);
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedFirstTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_serendael_starttrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(LuckTrial.CompletedTrial2))
                {
                    Subject.Reply(source, "Skip", "mainstory_serendael_finishedtrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedSecondTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_serendael_retrytrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedSecondTrial))
                {
                    Subject.Reply(source, "Speak to Theselene about the Trial of Intelligence.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedThirdTrial))
                {
                    Subject.Reply(source, "Speak to Skandara about the Trial of Sacrifice.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_assembleartifacts1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    Subject.Reply(source, "The coin flip is on your side if you've made it here but please speak to Goddess Miraelis.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2))
                {
                    Subject.Reply(source, "Skip", "mainstory_serendael_initial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3))
                {
                    Subject.Reply(source, "Skip", "mainstory_serendael_return");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4))
                    Subject.Reply(source, "Skip", "mainstory_serendael_finisheda1");

                break;
            }

            case "mainstory_serendael_starttrial4":
            {
                Subject.Close(source);

                source.Trackers.Enums.Set(LuckTrial.StartedTrial);
                source.Trackers.TimedEvents.AddEvent("lucktrialcd", TimeSpan.FromHours(1), true);
                source.Trackers.Enums.Set(MainStoryEnums.StartedSecondTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofluck");
                var point = new Point(8, 71);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_serendael_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("lucktrialcd", out var cdtime))
                    Subject.Reply(
                        source,
                        $"You have recently tried the Luck Trial. You can try again in {cdtime.Remaining.ToReadableString()}");

                break;
            }

            case "mainstory_serendael_retrytrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(LuckTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedSecondTrial);
                source.Trackers.TimedEvents.AddEvent("lucktrialcd", TimeSpan.FromHours(1), true);
                var mapinstance = simpleCache.Get<MapInstance>("trialofluck");
                var point = new Point(8, 71);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_serendael_finishedtrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedSecondTrial);
                ExperienceDistributionScript.GiveExp(source, 300000);
                source.TryGiveGamePoints(10);
                source.Trackers.Enums.Remove<LuckTrial>();

                return;
            }

            case "mainstory_serendael_initial4":
            {
                source.Trackers.Enums.Set(MainStoryEnums.StartedArtifact3);

                return;
            }

            case "mainstory_serendael_return2":
            {
                if (!source.Inventory.HasCount("Wind Artifact", 1))
                {
                    Subject.Reply(source, "Skip", "mainstory_serendael_returnno");
                    source.SendOrangeBarMessage("You are missing the Wind Artifact.");

                    return;
                }

                var animate = new Animation
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 21
                };
                source.Animate(animate);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedArtifact3);
                source.Trackers.Flags.AddFlag(MainstoryFlags.CompletedArtifact3);
                source.Inventory.Remove("Wind Artifact");
                ExperienceDistributionScript.GiveExp(source, 100000);
                source.SendOrangeBarMessage("You hand over the Wind Artifact.");

                return;
            }
            case "mainstory_serendael_return4":
            {
                source.SendOrangeBarMessage("Speak to Goddess Skandara about the Sea Artifact");
            }

                break;
            #endregion

            #region Theselene
            case "mainstory_theselene_initial":
            {
                if (source.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mainstory_theselene_exchange",
                        OptionText = "Trade 97 Armor"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    Subject.Text.EqualsI("Welcome back great hero. Are you interested in trading your 97 armor in?");
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedSecondTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_theselene_starttrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(IntelligenceTrial.CompletedTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_theselene_finishedtrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedThirdTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_theselene_retrytrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedThirdTrial))
                {
                    Subject.Reply(source, "Speak to Skandara about the Trial of Sacrifice.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    Subject.Reply(source, "Do not bother me, go speak to Goddess Miraelis.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact1))
                {
                    Subject.Reply(source, "Skip", "mainstory_theselene_initial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2))
                {
                    Subject.Reply(source, "Skip", "mainstory_theselene_return");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3))
                    Subject.Reply(source, "Skip", "mainstory_theselene_finisheda1");

                break;
            }

            case "mainstory_theselene_starttrial4":
            {
                Subject.Close(source);

                source.Trackers.Enums.Set(IntelligenceTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedThirdTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofintelligence");
                var point = new Point(10, 27);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_theselene_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("intelligencetrialcd", out var cdtime))
                    Subject.Reply(
                        source,
                        $"You have recently tried the Intelligence Trial. You can try again in {cdtime.Remaining.ToReadableString()}");

                break;
            }

            case "mainstory_theselene_retrytrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(IntelligenceTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedThirdTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofintelligence");
                var point = new Point(10, 27);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_theselene_finishedtrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedThirdTrial);
                ExperienceDistributionScript.GiveExp(source, 400000);
                source.TryGiveGamePoints(5);
                source.Trackers.Enums.Remove<IntelligenceTrial>();

                return;
            }

            case "mainstory_theselene_initial4":
            {
                source.Trackers.Enums.Set(MainStoryEnums.StartedArtifact2);

                return;
            }

            case "mainstory_theselene_return2":
            {
                if (!source.Inventory.HasCount("Fire Artifact", 1))
                {
                    Subject.Reply(source, "Skip", "mainstory_theselene_returnno");
                    source.SendOrangeBarMessage("You are missing the Fire Artifact.");

                    return;
                }

                var animate = new Animation
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 21
                };
                source.Animate(animate);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedArtifact2);
                source.Trackers.Flags.AddFlag(MainstoryFlags.CompletedArtifact2);
                source.Inventory.Remove("Fire Artifact");
                ExperienceDistributionScript.GiveExp(source, 125000);
                source.SendOrangeBarMessage("You hand over the Fire Artifact.");

                return;
            }
            case "mainstory_theselene_return4":
            {
                source.SendOrangeBarMessage("Speak to Goddess Serendael about the Wind Artifact");

                return;
            }

            case "mainstory_theselene_exchange2":
            {
                // Dictionary containing the appropriate armor for each class and gender
                var pentagearDictionary = new Dictionary<(BaseClass, Gender), string[]>
                {
                    {
                        (BaseClass.Warrior, Gender.Male), ["hybrasylplate"]
                    },
                    {
                        (BaseClass.Warrior, Gender.Female), ["hybrasylarmor"]
                    },
                    {
                        (BaseClass.Monk, Gender.Male), ["mountaingarb"]
                    },
                    {
                        (BaseClass.Monk, Gender.Female), ["seagarb"]
                    },
                    {
                        (BaseClass.Rogue, Gender.Male), ["bardocle"]
                    },
                    {
                        (BaseClass.Rogue, Gender.Female), ["kagum"]
                    },
                    {
                        (BaseClass.Priest, Gender.Male), ["dalmatica"]
                    },
                    {
                        (BaseClass.Priest, Gender.Female), ["bansagart"]
                    },
                    {
                        (BaseClass.Wizard, Gender.Male), ["duinuasal"]
                    },
                    {
                        (BaseClass.Wizard, Gender.Female), ["clamyth"]
                    }
                };

                // List of all level 97 armor items
                var level97Armor = new HashSet<string>
                {
                    "hybrasylarmor",
                    "hybrasylplate",
                    "mountaingarb",
                    "seagarb",
                    "bardocle",
                    "kagum",
                    "dalmatica",
                    "bansagart",
                    "duinuasal",
                    "clamyth"
                };

                // Remove any level 97 armor that does not match the current class
                var currentGear = source.Inventory
                                        .Where(item => level97Armor.Contains(item.Template.TemplateKey))
                                        .ToList();
                var hasRemovedOldGear = false;

                foreach (var item in currentGear)
                    if (!pentagearDictionary[(source.UserStatSheet.BaseClass, source.Gender)]
                            .Contains(item.Template.TemplateKey))
                    {
                        source.Inventory.RemoveByTemplateKey(item.Template.TemplateKey);
                        hasRemovedOldGear = true;
                    }

                // Check if the player has given up their old gear before giving new gear
                if (hasRemovedOldGear)
                {
                    // Give the appropriate armor based on the player's current class and gender
                    var gearKey = (source.UserStatSheet.BaseClass, source.Gender);

                    if (pentagearDictionary.TryGetValue(gearKey, out var armor))
                        foreach (var gearItemName in armor)
                        {
                            var gearItem = itemFactory.Create(gearItemName);
                            source.GiveItemOrSendToBank(gearItem);
                        }
                } else
                {
                    // Notify the player that they must give up their old gear first
                    source.Client.SendServerMessage(
                        ServerMessageType.OrangeBar1,
                        "You must give up your old level 97 gear before receiving new gear.");
                    Subject.Close(source);
                }

                break;
            }
            #endregion

            #region Skandara
            case "mainstory_skandara_initial":
            {
                if (source.Titles.ContainsI("Bounty Master") && !source.Trackers.Flags.HasFlag(MainstoryFlags.BountyBoardEpicChampion))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "mainstory_skandara_bountymaster1",
                        OptionText = "I am a Bounty Master"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.UserStatSheet.Master)
                {
                    var hasMasterWeapon = false;

                    foreach ((var baseWeapon, var masterWeapons) in masterWeapon)
                    {
                        // Check for the base weapon first
                        if (source.Inventory.ContainsByTemplateKey(baseWeapon) || source.Bank.Contains(baseWeapon))
                        {
                            hasMasterWeapon = true;

                            break;
                        }

                        // Then check for enchanted or empowered versions
                        foreach (var version in masterWeapons)
                        {
                            var item = itemFactory.Create(version);

                            if (source.Inventory.ContainsByTemplateKey(version)
                                || source.Bank.Contains(item.Template.TemplateKey)
                                || source.Equipment.ContainsByTemplateKey(version))
                            {
                                hasMasterWeapon = true;

                                break;
                            }
                        }

                        // Exit loop if a weapon is found
                        if (hasMasterWeapon)
                            break;
                    }

                    if (!hasMasterWeapon)
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "mainstory_skandara_replaceweapon",
                            OptionText = "Replace Master Weapon"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);

                        return;
                    }
                }

                if (source.Trackers.Flags.HasFlag(MainstoryFlags.FinishedDungeon))
                {
                    var hasEnchantedOrEmpoweredWeapon = false;

                    foreach (var weapon in masterWeapon)
                    {
                        var enchantedOrEmpoweredVersions = weapon.Value; // List of enchanted and empowered versions

                        // Check if the player's inventory contains either the enchanted or empowered version of the weapon
                        foreach (var version in enchantedOrEmpoweredVersions)
                            if (source.Inventory.ContainsByTemplateKey(version))
                            {
                                hasEnchantedOrEmpoweredWeapon = true;

                                break; // Exit inner loop once a match is found
                            }

                        if (hasEnchantedOrEmpoweredWeapon)
                            break; // Exit outer loop once a match is found
                    }

                    // If the player doesn't have any enchanted or empowered weapons, return
                    if (!hasEnchantedOrEmpoweredWeapon)
                        return;

                    // If the player has an enchanted or empowered weapon, add the dialog option
                    var option = new DialogOption
                    {
                        DialogKey = "mainstory_skandara_restoreweapon",
                        OptionText = "Restore Master Weapon"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedThirdTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_skandara_starttrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(SacrificeTrial.FinishedTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_skandara_finishedtrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedFourthTrial))
                {
                    Subject.Reply(source, "Skip", "mainstory_skandara_retrytrial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    Subject.Reply(source, "I am not aware of why we are all here, please speak to Goddess Miraelis.");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3))
                {
                    Subject.Reply(source, "Skip", "mainstory_skandara_initial1");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4))
                {
                    Subject.Reply(source, "Skip", "mainstory_skandara_return");

                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4))
                    Subject.Reply(source, "Skip", "mainstory_skandara_finisheda1");

                break;
            }

            case "mainstory_skandara_bountymaster5":
            {
                source.Trackers.Flags.AddFlag(MainstoryFlags.BountyBoardEpicChampion);
                var nyxPendant = itemFactory.Create("nyxpendant");
                source.GiveItemOrSendToBank(nyxPendant);
                var celebrationEffect = effectFactory.Create("celebration");
                source.Effects.Apply(source, celebrationEffect);
                var spell = spellFactory.Create("Celebrate");
                source.SpellBook.TryAdd(73, spell);

                foreach (var client in ClientRegistry)
                    client.Aisling.SendActiveMessage($"{source.Name} has obtained Nyx's Pendant as a true Bounty Hunter.");

                break;
            }

            case "mainstory_skandara_starttrial4":
            {
                Subject.Close(source);

                source.Trackers.Enums.Set(SacrificeTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedFourthTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofsacrifice");
                var point = new Point(13, 13);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_skandara_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("sacrificetrialcd", out var cdtime))
                    Subject.Reply(
                        source,
                        $"You have recently tried the Sacrifice Trial. You can try again in {cdtime.Remaining.ToReadableString()}");

                break;
            }

            case "mainstory_skandara_retrytrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(SacrificeTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedFourthTrial);
                var mapinstance = simpleCache.Get<MapInstance>("trialofsacrifice");
                var point = new Point(13, 13);
                source.TraverseMap(mapinstance, point);

                return;
            }

            case "mainstory_skandara_finishedtrial3":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedFourthTrial);
                ExperienceDistributionScript.GiveExp(source, 500000);
                source.TryGiveGamePoints(15);
                source.Trackers.Enums.Remove<SacrificeTrial>();

                return;
            }

            case "mainstory_skandara_initial4":
            {
                source.Trackers.Enums.Set(MainStoryEnums.StartedArtifact4);

                return;
            }

            case "mainstory_skandara_return2":
            {
                if (!source.Inventory.HasCount("Sea Artifact", 1))
                {
                    Subject.Reply(source, "Skip", "mainstory_skandara_returnno");
                    source.SendOrangeBarMessage("You are missing the Sea Artifact.");

                    return;
                }

                var animate = new Animation
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 21
                };
                source.Animate(animate);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedArtifact4);
                source.Trackers.Flags.AddFlag(MainstoryFlags.CompletedArtifact4);
                source.Inventory.Remove("Sea Artifact");
                ExperienceDistributionScript.GiveExp(source, 150000);
                source.SendOrangeBarMessage("You hand over the Sea Artifact.");

                return;
            }
            case "mainstory_skandara_return4":
            {
                source.SendOrangeBarMessage("Speak to Goddess Miraelis about the True Elemental Artifact.");

                break;
            }

            case "mainstory_skandara_restoreweapon3":
            {
                foreach ((var baseWeapon, var enchantedOrEmpoweredVersions) in masterWeapon)
                {
                    // Check if the player has either the enchanted or empowered version
                    foreach (var version in enchantedOrEmpoweredVersions)
                        if (source.Inventory.ContainsByTemplateKey(version))
                        {
                            // Remove the enchanted or empowered version
                            source.Inventory.RemoveByTemplateKey(version);

                            var newWeapon = itemFactory.Create(baseWeapon);

                            // Give the base version of the master weapon
                            source.GiveItemOrSendToBank(newWeapon);
                            source.SendOrangeBarMessage($"Skandara replaces your weapon with a {newWeapon.DisplayName}.");

                            return; // Exit after replacing the weapon
                        }
                }

                break;
            }

            case "mainstory_skandara_replaceweapon3":
            {
                if (!source.Inventory.HasCountByTemplateKey("strangestone", 2))
                {
                    Subject.Reply(source, "You are missing the two Strange Stones.");

                    return;
                }

                if (!source.Inventory.HasCountByTemplateKey("polishedcrimsonitebar", 5))
                {
                    Subject.Reply(source, "You are missing the five Polished Crimsonite Bars.");

                    return;
                }

                if (!source.Inventory.HasCountByTemplateKey("polishedcrimsonitebar", 5))
                {
                    Subject.Reply(source, "You are missing the five Polished Azurium Bars.");

                    return;
                }

                if (!source.TryTakeGold(1000000))
                {
                    Subject.Reply(source, "You don't have the 1,000,000 Gold required.");

                    return;
                }

                source.Inventory.RemoveQuantityByTemplateKey("strangestone", 2);
                source.Inventory.RemoveQuantityByTemplateKey("polishedcrimsonitebar", 5);
                source.Inventory.RemoveQuantityByTemplateKey("polishedazuriumbar", 5);

                var weaponDictionary = new Dictionary<BaseClass, string[]>
                {
                    {
                        BaseClass.Warrior, ["hybrasylescalon"]
                    },
                    {
                        BaseClass.Monk, ["kalkuri"]
                    },
                    {
                        BaseClass.Rogue, ["hybrasylazoth"]
                    },
                    {
                        BaseClass.Priest, ["holyhybrasylgnarl"]
                    },
                    {
                        BaseClass.Wizard, ["magusorb"]
                    }
                };

                var gearKey = source.UserStatSheet.BaseClass;

                if (weaponDictionary.TryGetValue(gearKey, out var weapon))
                    foreach (var weaponKey in weapon)
                    {
                        var gearItem = itemFactory.Create(weaponKey);
                        source.GiveItemOrSendToBank(gearItem);
                        source.SendOrangeBarMessage($"Skandara hands you {gearItem.DisplayName}.");
                    }

                Subject.Reply(source, "I hope you take good care of this one. But if you need another, you can always come back.");

                break;
            }
            #endregion
        }
    }
}