using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Jail;

public class JailScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public JailScript(ReactorTile subject)
        : base(subject) { }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (!aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out var timedEvent))
        {
            var escape = new Point(3, 6);
            source.WarpTo(escape);
            aisling.SendOrangeBarMessage("Your jail sentence is up, stay out of trouble.");
            return;
        }

        var timeLeft = timedEvent.Remaining.ToReadableString(
            false,
            true,
            true,
            true);
        
        aisling.SendPersistentMessage($"{timeLeft} until you finish your sentence.");
        var point = source.DirectionalOffset(source.Direction.Reverse());
        source.WarpTo(point);
    }
}