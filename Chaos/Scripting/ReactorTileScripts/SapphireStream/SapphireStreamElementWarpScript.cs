using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.SapphireStream;

public class SapphireStreamElementWarpScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public SapphireStreamElementWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    /// <inheritdoc />
    /// This method is called when a Creature (source) walks on the MapObject. If the source is an Aisling, it checks to see if it has a
    /// MonkElementForm (form) set. Depending on the form, the key is set to one of the  elemental masters. Then the targetMap is set to
    /// the corresponding MapInstance. Finally, the source is warped to the targetMap at the Point (13, 7).
    public override void OnWalkedOn(Creature source)
    {
        if (source is Aisling aisling)
        {
            var key = "";
            var hasForm = aisling.Trackers.Enums.TryGetValue(out MonkElementForm form);

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