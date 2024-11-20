using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EasterEggs;

public sealed class LeakingBarrelScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public LeakingBarrelScript(ReactorTile subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        if (source.Trackers.Flags.HasFlag(Definitions.EasterEggs.LeakyBarrel))
        {
            source.SendOrangeBarMessage("You've already found this.");
            return;
        }
        
        source.Trackers.Flags.AddFlag(Definitions.EasterEggs.LeakyBarrel);
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "You notice tiny crabs sheltering in this barrel.");
        source.TryGiveGamePoints(5);
        source.SendOrangeBarMessage("Do not share Easter Eggs with others.");
    }
}