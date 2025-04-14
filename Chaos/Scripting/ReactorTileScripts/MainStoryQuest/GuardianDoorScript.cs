using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class GuardianDoorScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public GuardianDoorScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);

        if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1))
        {
            if (aisling.MapInstance.Template.TemplateKey != "463")
            {
                aisling.SendOrangeBarMessage("You are not on this Guardian.");
                WarpBack(aisling);

                return;
            }
        }else if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2))
        {
            if (aisling.MapInstance.Template.TemplateKey != "98")
            {
                aisling.SendOrangeBarMessage("You are not on this Guardian.");
                WarpBack(aisling);

                return;
            }
        }else if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3))
        {
            if (aisling.MapInstance.Template.TemplateKey != "3066")
            {
                aisling.SendOrangeBarMessage("You are not on this Guardian.");
                WarpBack(aisling);  
                return;
            }
        }else if (source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4))
        {
            if (aisling.MapInstance.Template.TemplateKey != "4731")
            {
                aisling.SendOrangeBarMessage("You are not on this Guardian.");
                WarpBack(aisling);

                return;
            }
        }

        // Ensure the Aisling has a group and all members are nearby
        if (aisling.Group is null || !aisling.Group.All(member => member.OnSameMapAs(aisling) && member.WithinRange(aisling)))
        {
            aisling.SendOrangeBarMessage("Your group members must be nearby to enter the Guardian's Domain.");
            WarpBack(aisling);

            return;
        }

        // Define the artifact-specific logic
        var artifactChecks = new[]
        {
            (MainStoryEnums.StartedArtifact1, MainstoryFlags.CompletedArtifact1, "Earth Artifact", new Rectangle(
                11,
                16,
                2,
                3)),
            (MainStoryEnums.StartedArtifact2, MainstoryFlags.CompletedArtifact2, "Fire Artifact", new Rectangle(
                16,
                8,
                2,
                3)),
            (MainStoryEnums.StartedArtifact3, MainstoryFlags.CompletedArtifact3, "Wind Artifact", new Rectangle(
                11,
                16,
                2,
                3)),
            (MainStoryEnums.StartedArtifact4, MainstoryFlags.CompletedArtifact4, "Sea Artifact", new Rectangle(
                4,
                9,
                2,
                3))
        };

        foreach ((var artifactEnum, var artifactFlag, var artifactItem, var rectangle) in artifactChecks)
            if (source.Trackers.Enums.HasValue(artifactEnum) && AllMembersMeetRequirements(artifactEnum, artifactFlag, artifactItem))
            {
                var groupMembers = aisling.Group
                                          .ThatAreWithinRange(source, 13)
                                          .ToList();

                foreach (var member in groupMembers)
                {
                    var randomPoint = rectangle.GetRandomPoint();
                    member.TraverseMap(targetMap, randomPoint);
                }

                return;
            }

        // If no artifact checks pass, notify and warp back
        aisling.SendOrangeBarMessage("Someone in your group isn't on this quest.");
        WarpBack(aisling);

        return;

        bool AllMembersMeetRequirements(MainStoryEnums artifactEnum, MainstoryFlags artifactFlag, string artifactItem)
            => aisling.Group.All(member => member.Trackers.Enums.HasValue(artifactEnum) || member.Trackers.Flags.HasFlag(artifactFlag))
               && aisling.Group.All(member => !member.Inventory.HasCount(artifactItem, 1));
    }

    private void WarpBack(Aisling aisling)
    {
        var point = aisling.DirectionalOffset(aisling.Direction.Reverse());
        aisling.WarpTo(point);
    }
}