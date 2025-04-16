using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class EventMultiplexingWarpScript : ConfigurableReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected IIntervalTimer AnimationTimer { get; set; }

    public EventMultiplexingWarpScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        AnimationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);
    }

    public override void OnWalkedOn(Creature source)
    {
        var currentDate = DateTime.UtcNow;

        // Check if the current map is part of an active event
        if ((EventDestination != null) && !EventPeriod.IsEventActive(currentDate, EventDestination))
            return;

        // Calculate vitality only if the player's level is 99
        var vitality = source.StatSheet.Level == 99 ? source.StatSheet.MaximumHp + source.StatSheet.MaximumMp * 2 : 0;

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

                // Check MinVitality and MaxVitality conditions only if the player is level 99
                if (source.StatSheet.Level == 99)
                {
                    if (w.MinVitality.HasValue && (w.MinVitality.Value > vitality))
                        return false;

                    if (w.MaxVitality.HasValue && (w.MaxVitality.Value < vitality))
                        return false;
                }

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

    public override void Update(TimeSpan delta)
    {
        var currentDate = DateTime.UtcNow;
        if ((EventDestination != null) && !EventPeriod.IsEventActive(currentDate, EventDestination))
            return;
        
        AnimationTimer.Update(delta);

        if (AnimationTimer.IntervalElapsed)
        {
            var animation = new Animation
            {
                TargetAnimation = 214,
                AnimationSpeed = 200
            };

            Subject.MapInstance.ShowAnimation(animation.GetPointAnimation(Subject));
        }
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

    #region ScriptVars
    public ICollection<WarpDetails> Warps { get; set; } = null!;
    public string? EventDestination { get; init; } = null!;
    #endregion
}