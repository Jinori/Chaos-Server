using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Abstractions;

public interface IReactorTileScript : IScript, IDeltaUpdatable
{
    void OnClicked(Aisling source);

    void OnGoldDroppedOn(Creature source, Money money);

    void OnGoldPickedUpFrom(Aisling source, Money money);

    void OnItemDroppedOn(Creature source, GroundItem groundItem);

    void OnItemPickedUpFrom(Aisling source, GroundItem groundItem);
    void OnWalkedOn(Creature source);
}