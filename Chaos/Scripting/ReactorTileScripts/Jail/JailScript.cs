using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.SapphireStream;

public class JailScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public JailScript(ReactorTile subject)
        : base(subject) { }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling || !aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out var timedEvent))
            return;

        aisling.SendOrangeBarMessage("You must serve your current jail sentence before leaving.");
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }

    #region ScriptVars
    #endregion
}