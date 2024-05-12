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
    {
        SimpleCache = simpleCache;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        
        if (source is not Aisling aisling)
            return;

        Point point;
        if (aisling.Group is null || aisling.Group.Any(x => !x.OnSameMapAs(aisling) || !x.WithinRange(aisling)))
        {
            // Send a message to the Aisling
            aisling.SendOrangeBarMessage("You must have a group nearby to enter the Guardian's Domain.");
            // Warp the source back
            point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
            return;
        }

        var mainmember1 = source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1);
        var allMembersHaveQuestEnum1 = aisling.Group.All(member =>
            member.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact1) 
            || member.Trackers.Flags.HasFlag(MainstoryFlags.CompletedArtifact1) 
            && aisling.Group.All(x => !x.Inventory.HasCount("Earth Artifact", 1)));

        var group1 = aisling.Group.ThatAreWithinRange(source, 13).ToList();

        
        if (mainmember1 && allMembersHaveQuestEnum1)
        {
            var rectangle = new Rectangle(11, 16, 2, 3);
            
            foreach (var member in group1)
            {
                var randomPoint = rectangle.GetRandomPoint();
                member.TraverseMap(targetMap, randomPoint);
            }

            return;
        }
        
        var mainmember2 = source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2);
        var allMembersHaveQuestEnum2 = aisling.Group.All(member =>
            member.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact2) 
            || member.Trackers.Flags.HasFlag(MainstoryFlags.CompletedArtifact2) 
            && aisling.Group.All(x => !x.Inventory.HasCount("Fire Artifact", 1)));

        var group2 = aisling.Group.ThatAreWithinRange(source, 13).ToList();

        
        if (mainmember2 && allMembersHaveQuestEnum2)
        {
            var rectangle = new Rectangle(16, 8, 2, 3);
            
            foreach (var member in group2)
            {
                var randomPoint = rectangle.GetRandomPoint();
                member.TraverseMap(targetMap, randomPoint);
            }

            return;
        }

        var mainmember3 = source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3);
        
        var allMembersHaveQuestEnum3 = aisling.Group.All(member =>
            member.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact3) 
            || member.Trackers.Flags.HasFlag(MainstoryFlags.CompletedArtifact3) 
            && aisling.Group.All(x => !x.Inventory.HasCount("Wind Artifact", 1)));

        var group3 = aisling.Group.ThatAreWithinRange(source, 13).ToList();

        
        if (mainmember3 && allMembersHaveQuestEnum3)
        {
            var rectangle = new Rectangle(11, 16, 2, 3);
            
            foreach (var member in group3)
            {
                var randomPoint = rectangle.GetRandomPoint();
                member.TraverseMap(targetMap, randomPoint);
            }

            return;
        }
        
        var mainmember4 = source.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4);
        var allMembersHaveQuestEnum4 = aisling.Group.All(member =>
            member.Trackers.Enums.HasValue(MainStoryEnums.StartedArtifact4) 
            || member.Trackers.Flags.HasFlag(MainstoryFlags.CompletedArtifact4) 
            && aisling.Group.All(x => !x.Inventory.HasCount("Sea Artifact", 1)));

        var group4 = aisling.Group.ThatAreWithinRange(source, 13).ToList();

        
        if (mainmember4 && allMembersHaveQuestEnum4)
        {
            var rectangle = new Rectangle(4, 9, 2, 3);
            
            foreach (var member in group4)
            {
                var randomPoint = rectangle.GetRandomPoint();
                member.TraverseMap(targetMap, randomPoint);
            }

            return;
        }
        
        aisling.SendOrangeBarMessage("Someone in your group isn't on this quest.");
        // Warp the source back
        point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }
}