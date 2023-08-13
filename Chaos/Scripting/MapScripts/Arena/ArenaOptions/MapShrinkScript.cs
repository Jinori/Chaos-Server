using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MapScripts.Arena.ArenaOptions;

public sealed class MapShrinkScript : MapScriptBase
{
    private readonly List<IPoint> CurrentMapPoints = new();
    private readonly List<IPoint> NextMapPoints = new();
    private readonly ISimpleCache SimpleCache;
    private DateTime? AnnouncedMorphStart;
    private bool AnnounceMorph;
    private bool CountdownMorphStarted;
    private int CountdownMorphStep;
    private DateTime LastCountdownMorphTime;
    private bool MapWallsCaptured;
    private int MorphCount;
    private double TimePassedSinceMainAnimationStart;
    private double TimePassedSinceTileAnimation;
    private IApplyDamageScript ApplyDamageScript { get; }

    private List<string> MorphTemplateKeys { get; } = new()
        { "26007", "26008", "26009", "26010", "26011" };
    private IIntervalTimer MorphTimer { get; }

    private Animation PreAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 214
    };

    /// <inheritdoc />
    public MapShrinkScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
    {
        MorphTimer = new IntervalTimer(TimeSpan.FromSeconds(15), false);
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SimpleCache = simpleCache;
    }

    private void MorphMap()
    {
        var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
        Subject.Morph(templateKey);
        MorphCount++;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        MorphTimer.Update(delta);

        if (!MorphTimer.IntervalElapsed)
            return;

        if (!MapWallsCaptured)
        {
            var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
            var currentMapTemp = SimpleCache.Get<MapTemplate>(Subject.Template.TemplateKey);
            var nextMapTemp = SimpleCache.Get<MapTemplate>(templateKey);

            for (var x = 0; x < currentMapTemp.Width; x++)
            {
                for (var y = 0; y < currentMapTemp.Height; y++)
                {
                    var point = new Point(x, y);

                    if (currentMapTemp.IsWithinMap(point) && !currentMapTemp.IsWall(point) && !CurrentMapPoints.Contains(point))
                        CurrentMapPoints.Add(point);
                }
            }

            for (var x = 0; x < nextMapTemp.Width; x++)
            {
                for (var y = 0; y < nextMapTemp.Height; y++)
                {
                    var point = new Point(x, y);

                    if (nextMapTemp.IsWithinMap(point) && nextMapTemp.IsWall(point) && !NextMapPoints.Contains(point))
                        NextMapPoints.Add(point);
                }
            }

            MapWallsCaptured = true;
        }

        if (!AnnounceMorph && (MorphCount < MorphTemplateKeys.Count))
        {
            AnnouncedMorphStart = DateTime.UtcNow;
            AnnounceMorph = true;
        }
        else if (AnnouncedMorphStart.HasValue
                 && (DateTime.UtcNow.Subtract(AnnouncedMorphStart.Value).TotalSeconds >= 15)
                 && !CountdownMorphStarted
                 && (MorphCount < MorphTemplateKeys.Count))
        {
            var allPlayers = Subject.GetEntities<Aisling>().ToList();

            foreach (var player in allPlayers)
                player.SendActiveMessage("Lava will claim more of the map in ten seconds!");

            CountdownMorphStarted = true;
            CountdownMorphStep = 9;
            LastCountdownMorphTime = DateTime.UtcNow;
        }
        else if (CountdownMorphStarted && (CountdownMorphStep > 0) && (MorphCount < MorphTemplateKeys.Count))
        {
            if (DateTime.UtcNow.Subtract(LastCountdownMorphTime).TotalSeconds >= 1)
            {
                var allPlayers = Subject.GetEntities<Aisling>().ToList();

                foreach (var player in allPlayers)
                {
                    var message = CountdownMorphStep > 0
                        ? "Lava creeps in " + CountdownMorphStep.ToWords() + " seconds!"
                        : "Lava has flowed inwards!";

                    player.SendActiveMessage(message);
                }

                var currentSecond = (int)Math.Floor(TimePassedSinceMainAnimationStart);

                foreach (var nowall in CurrentMapPoints)
                    if (NextMapPoints.Contains(nowall))
                        if ((int)Math.Floor(TimePassedSinceMainAnimationStart) == currentSecond)
                            Subject.ShowAnimation(PreAnimation.GetPointAnimation(nowall));

                TimePassedSinceMainAnimationStart += delta.TotalSeconds; // Increment the main animation timer

                TimePassedSinceTileAnimation += delta.TotalSeconds;

                if (TimePassedSinceTileAnimation >= 1)
                {
                    TimePassedSinceTileAnimation = 0;
                    CurrentMapPoints.Clear();
                }

                LastCountdownMorphTime = DateTime.UtcNow;
                CountdownMorphStep--;
            }
        }
        else if (CountdownMorphStarted && (CountdownMorphStep == 0) && (MorphCount < MorphTemplateKeys.Count))
        {
            var aislingsToKill = Subject.GetEntitiesAtPoints<Aisling>(CurrentMapPoints).Where(x => NextMapPoints.Contains(x)).ToList();

            foreach (var aisling in aislingsToKill)
            {
                var damage = (int)(aisling.StatSheet.EffectiveMaximumHp * 1000);

                ApplyDamageScript.ApplyDamage(
                    aisling,
                    aisling,
                    this,
                    damage);
            }

            MorphMap();
            CountdownMorphStep = 9;
            CountdownMorphStarted = false;
            AnnounceMorph = false;
            CurrentMapPoints.Clear();
            NextMapPoints.Clear();
            MapWallsCaptured = false;
        }
    }
}