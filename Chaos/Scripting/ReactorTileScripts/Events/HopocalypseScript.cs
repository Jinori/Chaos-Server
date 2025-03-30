using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Events;

public class HopocalypseScript : ReactorTileScriptBase
{
    private readonly Point LeftWarp = new(0, 9);
    private readonly Point LeftWarpExit = new(19, 9);

    private readonly Point RightWarp = new(20, 9);
    private readonly Point RightWarpExit = new(1, 9);

    /// <inheritdoc />
    public HopocalypseScript(ReactorTile subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (!source.IsAlive)
            return;

        // Left tunnel entry
        if (new Point(source.X, source.Y) == LeftWarp)
        {
            source.WarpTo(LeftWarpExit);

            return;
        }

        // Right tunnel entry
        if (new Point(source.X, source.Y) == RightWarp)
            source.WarpTo(RightWarpExit);
    }
}