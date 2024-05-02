using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class TSReconTileScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public TSReconTileScript(ReactorTile subject)
        : base(subject){ }
    
    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out TheSacrificeQuestStage stage);

        if (!hasStage)
            return;

        if (hasStage && stage is not TheSacrificeQuestStage.Reconaissance)
            return;

        if (new Point(7, 11).Equals(Subject))
        {
            if (aisling.Trackers.Flags.HasFlag(ReconPoints.Reconpoint1))
            {
                aisling.SendOrangeBarMessage("You've already scouted this location, move to the next.");
                return;
            }
            aisling.Trackers.Flags.AddFlag(ReconPoints.Reconpoint1);
            aisling.SendOrangeBarMessage("You jot down some notes of the location.");
        }
        
        if (new Point(7, 32).Equals(Subject))
        {
            if (aisling.Trackers.Flags.HasFlag(ReconPoints.Reconpoint2))
            {
                aisling.SendOrangeBarMessage("You've already scouted this location, move to the next.");
                return;
            }
            aisling.Trackers.Flags.AddFlag(ReconPoints.Reconpoint2);
            aisling.SendOrangeBarMessage("You jot down some notes of the location.");
        }
        if (new Point(13, 33).Equals(Subject))
        {
            if (aisling.Trackers.Flags.HasFlag(ReconPoints.Reconpoint3))
            {
                aisling.SendOrangeBarMessage("You've already scouted this location, move to the next.");
                return;
            }
            aisling.Trackers.Flags.AddFlag(ReconPoints.Reconpoint3);
            aisling.SendOrangeBarMessage("You jot down some notes of the location.");
        }
        if (new Point(30, 28).Equals(Subject))
        {
            if (aisling.Trackers.Flags.HasFlag(ReconPoints.Reconpoint4))
            {
                aisling.SendOrangeBarMessage("You've already scouted this location, move to the next.");
                return;
            }
            
            aisling.Trackers.Flags.AddFlag(ReconPoints.Reconpoint4);
            aisling.SendOrangeBarMessage("You jot down some notes of the location.");
        }
        if (new Point(31, 8).Equals(Subject))
        {
            if (aisling.Trackers.Flags.HasFlag(ReconPoints.Reconpoint5))
            {
                aisling.SendOrangeBarMessage("You've already scouted this location, move to the next.");
                return;
            }
            aisling.Trackers.Flags.AddFlag(ReconPoints.Reconpoint5);
            aisling.SendOrangeBarMessage("You jot down some notes of the location.");
        }
        
        if (new Point(26, 6).Equals(Subject))
        {
            if (aisling.Trackers.Flags.HasFlag(ReconPoints.Reconpoint6))
            {
                aisling.SendOrangeBarMessage("You've already scouted this location, move to the next.");
                return;
            }
            
            aisling.Trackers.Flags.AddFlag(ReconPoints.Reconpoint6);
            aisling.SendOrangeBarMessage("You jot down some notes of the location.");
        }
    }
}