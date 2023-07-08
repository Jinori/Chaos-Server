using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Casino;

public class CasinoLockMovementScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public CasinoLockMovementScript(ReactorTile subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling || aisling.OnTwentyOneTile)
            return;

        aisling.OnTwentyOneTile = true;
    }
}