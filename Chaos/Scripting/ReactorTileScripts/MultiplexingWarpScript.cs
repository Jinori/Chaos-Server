using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class MultiplexingWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    #region ScriptVars
    public ICollection<WarpDetails> Warps { get; set; } = null!;
    #endregion

    public MultiplexingWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject) =>
        SimpleCache = simpleCache;

    public override void OnWalkedOn(Creature source)
    {
        var vitality = source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2;

        var warp = Warps.FirstOrDefault(
            w =>
            {
                if (source is not Aisling aisling)
                    return false;

                if (w.MinLevelNotify.HasValue && (w.MinLevelNotify.Value > source.StatSheet.Level))
                    aisling.SendOrangeBarMessage($"You must be level {w.MinLevelNotify} to enter");

                if (w.MinLevel.HasValue && (w.MinLevel.Value > source.StatSheet.Level))
                    return false;

                if (w.MaxLevel.HasValue && (w.MaxLevel.Value < source.StatSheet.Level))
                    return false;

                if ((source.StatSheet.Level > 98) && w.MinVitality.HasValue && (w.MinVitality.Value > vitality))
                    return false;

                if ((source.StatSheet.Level > 98) && w.MaxVitality.HasValue && (w.MaxVitality.Value < vitality))
                    return false;

                return true;
            });

        if (warp == null)
        {
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);

            return;
        }

        var mapInstance = SimpleCache.Get<MapInstance>(warp.Destination.Map);
        source.TraverseMap(mapInstance, warp.Destination);
    }

    public class WarpDetails
    {
        public Location Destination { get; set; } = null!;
        public int? MaxLevel { get; set; }
        public int? MaxVitality { get; set; }
        public int? MinLevel { get; set; }

        public int? MinLevelNotify { get; set; }
        public int? MinVitality { get; set; }
    }
}