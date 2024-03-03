using Chaos.Common.Definitions;
using Chaos.Models.World;
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
        source.Client.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "You notice tiny crabs sheltering in this barrel.");
        source.SendOrangeBarMessage("Tell WHUG you found this barrel.");
        return;
    }
}