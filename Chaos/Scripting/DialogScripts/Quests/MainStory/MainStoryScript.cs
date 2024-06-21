using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
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
    IDialogFactory dialogFactory)
    : DialogScriptBase(subject)
{
    private readonly ISimpleCache SimpleCache = simpleCache;

    private IExperienceDistributionScript ExperienceDistributionScript { get; } =
        DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out MainStoryEnums stage);

        switch (Subject.Template.TemplateKey.ToLower())
        {
            #region MysteriousArtifact
            case "mysteriousartifact_yes":
            {
                if (hasStage && stage == MainStoryEnums.MysteriousArtifactFound)
                {
                    var mysteriousartifact = itemFactory.Create("mysteriousartifact");
                    source.GiveItemOrSendToBank(mysteriousartifact);
                    source.Trackers.Enums.Set(MainStoryEnums.ReceivedMA);
                }

                break;
            }

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
                {
                    Subject.Reply(source, "Skip", "mysteriousartifact_initial2");
                }
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
                if (Subject.DialogSource.Name != "Miraelis" && Subject.DialogSource.Name != "Theselene" &&
                    Subject.DialogSource.Name != "Skandara" && Subject.DialogSource.Name != "Serendael")
                    return;
                
                if (source.Inventory.Contains("Mysterious Artifact")
                    && source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
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
                    Subject.Reply(source,
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
                var godsrealm = SimpleCache.Get<MapInstance>("godsrealm");
                var newPoint = new Point(16, 16);

                source.TraverseMap(godsrealm, newPoint);
                source.Animate(animation);
                return;
            }
            
            case "mainstory_miraelis_leaverealm":
                const Direction DIRECTION = Direction.Down;
            {
                var leaverealm = SimpleCache.Get<MapInstance>("mileth_inn");
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

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact1) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4))
                {
                    Subject.Reply(source,
                        "Skip", "mainstory_miraelis_finisheda1");
                    return;
                }

                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    Subject.Reply(source, "Skip", "mainstory_miraelis_initial11");
                }
                break;
            }

            case "mainstory_miraelis_summonerinitial1":
            {
                source.Trackers.Enums.Set(MainStoryEnums.CompletedTrials);
                source.Trackers.TimedEvents.AddEvent("mainstorycd1", TimeSpan.FromHours(1), true);
                return;
            }

            case "mainstory_miraelis_starttrial4":
            {
                Subject.Close(source);
                
                source.Trackers.Enums.Set(CombatTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedFirstTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofcombat");
                var point = new Point(15, 15);
                source.TraverseMap(mapinstance, point);
                
                return;
            }
            
            case "mainstory_miraelis_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("combattrialcd", out var cdtime))
                {
                    Subject.Reply(source, $"You have recently tried the Combat Trial. You can try again in {cdtime.Remaining.ToReadableString()}");
                }
                break;
            }
            
            case "mainstory_miraelis_retrytrial2":
            {
                Subject.Close(source);
                
                source.Trackers.Enums.Set(CombatTrial.StartedTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofcombat");
                var point = new Point(15, 15);
                source.TraverseMap(mapinstance, point);
                return;
            }

            case "mainstory_miraelis_finishedtrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedFirstTrial);
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

                var animate = new Animation()
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 21
                };
                source.Animate(animate);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedArtifact1);
                source.Trackers.Flags.AddFlag(MainstoryFlags.CompletedArtifact1);
                source.Inventory.Remove("Earth Artifact");
                ExperienceDistributionScript.GiveExp(source, 50000);
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

                var animate = new Animation()
                {
                    AnimationSpeed = 200,
                    TargetAnimation = 937
                };
                
                var miraelis = Subject.DialogSource as Merchant;
                miraelis!.Animate(animate);
                
                source.Trackers.Enums.Set(MainStoryEnums.CompletedArtifactsHunt);
                source.Inventory.RemoveQuantity("coal", 50);
                source.Inventory.RemoveQuantity("Ruined Iron", 20);
                ExperienceDistributionScript.GiveExp(source, 75000);
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

                if (source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4))
                {
                    Subject.Reply(source,
                        "Skip", "mainstory_serendael_finisheda1");
                }

                break;
            }

            case "mainstory_serendael_starttrial4":
            {
                Subject.Close(source);
                
                source.Trackers.Enums.Set(LuckTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedSecondTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofluck");
                var point = new Point(8, 71);
                source.TraverseMap(mapinstance, point);
                
                return;
            }
            
            case "mainstory_serendael_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("lucktrialcd", out var cdtime))
                {
                    Subject.Reply(source, $"You have recently tried the Luck Trial. You can try again in {cdtime.Remaining.ToReadableString()}");
                }
                break;
            }
            
            case "mainstory_serendael_retrytrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(LuckTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedSecondTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofluck");
                var point = new Point(8, 71);
                source.TraverseMap(mapinstance, point);
                return;
            }

            case "mainstory_serendael_finishedtrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedSecondTrial);
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

                var animate = new Animation()
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

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3) ||
                    source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3))
                {
                    Subject.Reply(source,
                        "Skip", "mainstory_theselene_finisheda1");
                }
                break;
            }
            
            case "mainstory_theselene_starttrial4":
            {
                Subject.Close(source);
                
                source.Trackers.Enums.Set(IntelligenceTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedThirdTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofintelligence");
                var point = new Point(10, 27);
                source.TraverseMap(mapinstance, point);
                
                return;
            }
            
            case "mainstory_theselene_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("intelligencetrialcd", out var cdtime))
                {
                    Subject.Reply(source, $"You have recently tried the Intelligence Trial. You can try again in {cdtime.Remaining.ToReadableString()}");
                }
                break;
            }
            
            case "mainstory_theselene_retrytrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(IntelligenceTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedThirdTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofintelligence");
                var point = new Point(10, 27);
                source.TraverseMap(mapinstance, point);
                return;
            }

            case "mainstory_theselene_finishedtrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedThirdTrial);
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

                var animate = new Animation()
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
            #endregion

            #region Skandara
            case "mainstory_skandara_initial":
            {
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
                {
                    Subject.Reply(source,
                        "Skip", "mainstory_skandara_finisheda1");
                }
                break;
            }
            
            case "mainstory_skandara_starttrial4":
            {
                Subject.Close(source);
                
                source.Trackers.Enums.Set(SacrificeTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedFourthTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofsacrifice");
                var point = new Point(13, 13);
                source.TraverseMap(mapinstance, point);
                
                return;
            }
            
            case "mainstory_skandara_retrytrial1":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("sacrificetrialcd", out var cdtime))
                {
                    Subject.Reply(source, $"You have recently tried the Sacrifice Trial. You can try again in {cdtime.Remaining.ToReadableString()}");
                }
                break;
            }
            
            case "mainstory_skandara_retrytrial2":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(SacrificeTrial.StartedTrial);
                source.Trackers.Enums.Set(MainStoryEnums.StartedFourthTrial);
                var mapinstance = SimpleCache.Get<MapInstance>("trialofsacrifice");
                var point = new Point(13, 13);
                source.TraverseMap(mapinstance, point);
                return;
            }

            case "mainstory_skandara_finishedtrial3":
            {
                Subject.Close(source);
                source.Trackers.Enums.Set(MainStoryEnums.FinishedFourthTrial);
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

                var animate = new Animation()
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
            }
                break;
            #endregion
        }
    }
}