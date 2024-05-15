using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.Mileth;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.MainStory;

public class MainStoryScript(
    Dialog subject,
    IItemFactory itemFactory,
    ILogger<SpareAStickScript> logger,
    IMerchantFactory merchantFactory,
    ISimpleCache simpleCache,
    IDialogFactory dialogFactory)
    : DialogScriptBase(subject)
{
    private readonly ILogger<SpareAStickScript> Logger = logger;

    private readonly ISimpleCache SimpleCache = simpleCache;

    private IExperienceDistributionScript ExperienceDistributionScript { get; } =
        DefaultExperienceDistributionScript.Create();

    public override void OnDisplaying(Aisling source)
    {
        var hasStage = source.Trackers.Enums.TryGetValue(out MainStoryEnums stage);
        var tnl = LevelUpFormulae.Default.CalculateTnl(source);
        var tenPercent = Convert.ToInt32(.10 * tnl);

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

                if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact1)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact3)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FinishedArtifact4)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedAssemble)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedArtifactsHunt)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.StartedCircuitTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedCircuitTrial)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.SearchForSummoner)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.FoundSummoner2)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.DefeatedServant)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.KilledSummoner)
                    || source.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
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
                }
                break;
            }
            
            case "mainstory_miraelis_initial10":
            {
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
                ExperienceDistributionScript.GiveExp(source, tenPercent);
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
                    TargetAnimation = 14
                };
                
                var miraelis = Subject.DialogSource as Merchant;
                miraelis!.Animate(animate);
                
                source.Trackers.Enums.Set(MainStoryEnums.CompletedArtifactsHunt);
                source.Inventory.RemoveQuantity("coal", 50);
                source.Inventory.RemoveQuantity("Ruined Iron", 20);
                ExperienceDistributionScript.GiveExp(source, tenPercent);
                source.SendOrangeBarMessage("You hand over the coal and ruined iron.");
                return;
            }
            #endregion
            
            #region Serendael
            case "mainstory_serendael_initial":
            {
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
                ExperienceDistributionScript.GiveExp(source, tenPercent);
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
                ExperienceDistributionScript.GiveExp(source, tenPercent);
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
                ExperienceDistributionScript.GiveExp(source, tenPercent);
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