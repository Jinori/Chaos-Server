using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.BookScripts;

public sealed class LeakingBarrelScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public LeakingBarrelScript(ReactorTile subject)
        : base(subject) {}

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        if (source.Trackers.Flags.HasFlag(EasterEggs.LeakyBarrel))
        {
            source.SendOrangeBarMessage("You've already found this.");
            return;
        }
        
        source.Trackers.Flags.AddFlag(EasterEggs.LeakyBarrel);
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "You notice tiny crabs sheltering in this barrel.");
        source.TryGiveGamePoints(5);
        source.SendOrangeBarMessage("Tell WHUG you found this barrel.");
        return;
    }
}