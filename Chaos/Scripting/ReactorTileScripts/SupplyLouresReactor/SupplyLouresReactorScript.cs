using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.SupplyLouresReactor;

public class SupplyLouresReactorScript : ReactorTileScriptBase
{
    /// <inheritdoc />
    public SupplyLouresReactorScript(ReactorTile subject)
        : base(subject) { }

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        aisling.SendOrangeBarMessage("Fight off your attacker! There is no escape!");
    }
}