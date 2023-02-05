using Chaos.Containers;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts.SapphireStream;

public class SapphireStreamElementWarpScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public SapphireStreamElementWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is Aisling aisling)
        {
            var key = "";
            var hasForm = aisling.Enums.TryGetValue(out MonkElementForm form);

            if (!hasForm)
            {
                aisling.SendOrangeBarMessage("You must choose an elemental form to access this area.");

                var point = source.DirectionalOffset(source.Direction.Reverse());
                source.WarpTo(point);

                return;
            }

            key = form switch
            {
                MonkElementForm.Air   => "air_elemental_master",
                MonkElementForm.Water => "water_elemental_master",
                MonkElementForm.Earth => "earth_elemental_master",
                MonkElementForm.Fire  => "fire_elemental_master",
                _                     => key
            };

            var targetMap = SimpleCache.Get<MapInstance>(key);
            source.TraverseMap(targetMap, new Point(13, 7));
        }
    }

    #region ScriptVars
    #endregion
}