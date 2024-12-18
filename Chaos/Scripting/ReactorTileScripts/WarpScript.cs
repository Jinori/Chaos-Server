using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class WarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public WarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
        => SimpleCache = simpleCache;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        var targetMap = SimpleCache.Get<MapInstance>(Destination.Map);
        var aisling = source as Aisling;
        var vitality = source.StatSheet.Level == 99 ? source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2 : 0;

        if (source.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
        {
            aisling?.SendOrangeBarMessage($"You must be at least level {targetMap.MinimumLevel} to enter this area.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);

            return;
        }

        if (source.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
        {
            aisling?.SendOrangeBarMessage($"You must be at most level {targetMap.MaximumLevel} to enter this area.");

            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);

            return;
        }

        if (MinLevel.HasValue && (MinLevel.Value > source.StatSheet.Level))
        {
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
            aisling?.SendOrangeBarMessage($"You must be at most level {MinLevel} to enter this area.");

            return;
        }

        if (MaxLevel.HasValue && (MaxLevel.Value < source.StatSheet.Level))
        {
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
            aisling?.SendOrangeBarMessage($"You must be at most level {MaxLevel} to enter this area.");

            return;
        }

        // Check MinVitality and MaxVitality conditions only if the player is level 99
        if (source.StatSheet.Level == 99)
        {
            if (MinVitality.HasValue && (MinVitality.Value > vitality))
            {
                var point = source.DirectionalOffset(source.Direction.Reverse());
                source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
                aisling?.SendOrangeBarMessage($"You must be atleast {MinVitality} Vitality to enter this area.");
            }

            if (MaxVitality.HasValue && (MaxVitality.Value < vitality))
            {
                var point = source.DirectionalOffset(source.Direction.Reverse());
                source.WarpTo(source.Trackers.LastPosition as IPoint ?? point);
                aisling?.SendOrangeBarMessage($"You must be under {MaxVitality} Vitality to enter this area.");

                return;
            }
        }

        source.TraverseMap(targetMap, Destination);
    }

    #region ScriptVars
    protected Location Destination { get; init; } = null!;
    public int? MaxLevel { get; set; }
    public int? MaxVitality { get; set; }
    public int? MinLevel { get; set; }
    public int? MinVitality { get; set; }
    #endregion
}