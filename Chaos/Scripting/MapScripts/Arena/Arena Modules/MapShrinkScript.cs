using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public sealed class MapShrinkScript : MapScriptBase
{
    private readonly List<IPoint> CurrentMapPoints = new();
    private readonly List<IPoint> NextMapPoints = new();
    private readonly ISimpleCache SimpleCache;
    private DateTime? AnnouncedMorphStart;
    private bool AnnounceMorph;
    private bool CountdownComplete;
    private bool CountdownMorphStarted;
    private int CountdownMorphStep;
    private DateTime LastCountdownMorphTime;
    private bool MapWallsCaptured;
    private int MorphCount;
    private double TimePassedSinceMainAnimationStart;
    private double TimePassedSinceTileAnimation;
    private IApplyDamageScript ApplyDamageScript { get; }

    private List<string> MorphTemplateKeys { get; } =
        [
            "26007",
            "26008",
            "26009",
            "26010",
            "26011"
        ];

    private List<string> OriginalMapKeys { get; } =
        [
            "26006",
            "26012"
        ];

    private Animation PreAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 214
    };

    private bool ShouldAnimateTiles
        => CountdownMorphStarted
           && (CountdownMorphStep > 0)
           && (DateTime.UtcNow.Subtract(LastCountdownMorphTime)
                       .TotalSeconds
               >= 1);

    /// <inheritdoc />
    public MapShrinkScript(MapInstance subject, ISimpleCache simpleCache)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SimpleCache = simpleCache;
    }

    private void AnimateTiles(TimeSpan delta)
    {
        var currentSecond = (int)Math.Floor(TimePassedSinceMainAnimationStart);

        foreach (var point in NextMapPoints)
            if (CurrentMapPoints.Contains(point) && ((int)Math.Floor(TimePassedSinceMainAnimationStart) == currentSecond))
                Subject.ShowAnimation(PreAnimation.GetPointAnimation(point));

        TimePassedSinceMainAnimationStart += delta.TotalSeconds;

        TimePassedSinceTileAnimation += delta.TotalSeconds;

        if (TimePassedSinceTileAnimation >= 1)
            TimePassedSinceTileAnimation = 0;
    }

    private void AnnouncedMorph()
    {
        AnnouncedMorphStart = DateTime.UtcNow;
        AnnounceMorph = true;
    }

    private void CaptureMapWalls()
    {
        if (MapWallsCaptured)
            return;

        MapTemplate currentMapTemp;
        MapTemplate nextMapTemp;

        if ((Subject.Name == "Lava Arena - Teams") && (MorphCount == 0))
        {
            currentMapTemp = SimpleCache.Get<MapTemplate>("26006");
            nextMapTemp = SimpleCache.Get<MapTemplate>("26007");
        } else
        {
            var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
            currentMapTemp = SimpleCache.Get<MapTemplate>(Subject.Template.TemplateKey);
            nextMapTemp = SimpleCache.Get<MapTemplate>(templateKey);
        }

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

    private void HandleCountdown()
    {
        if (CountdownMorphStarted)
        {
            if (CountdownMorphStep > 0)
            {
                if (DateTime.UtcNow.Subtract(LastCountdownMorphTime)
                            .TotalSeconds
                    >= 1)
                {
                    var message = CountdownMorphStep > 0
                        ? "Lava creeps in " + CountdownMorphStep.ToWords() + " seconds!"
                        : "Lava has flowed inwards!";

                    SendMessageToAllPlayers(message);

                    LastCountdownMorphTime = DateTime.UtcNow;
                    CountdownMorphStep--;
                }
            } else if ((CountdownMorphStep == 0)
                       && (DateTime.UtcNow.Subtract(LastCountdownMorphTime)
                                   .TotalSeconds
                           >= 1))
            {
                CountdownComplete = true;
                SendMessageToAllPlayers("Lava has flowed inwards!");
            }
        } else
        {
            SendMessageToAllPlayers("Lava will claim more of the map in ten seconds!");
            CountdownMorphStarted = true;
            CountdownMorphStep = 9;
            LastCountdownMorphTime = DateTime.UtcNow;
        }
    }

    private void MorphMap()
    {
        var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
        Subject.Morph(templateKey);
        MorphCount++;
    }

    /// <inheritdoc />
    public override void OnMorphed()
    {
        CountdownMorphStep = 9;
        CountdownMorphStarted = false;
        AnnounceMorph = false;
        CurrentMapPoints.Clear();
        NextMapPoints.Clear();
        MapWallsCaptured = false;
        CountdownComplete = false;
    }

    /// <inheritdoc />
    public override void OnMorphing(MapTemplate newMapTemplate)
    {
        if (OriginalMapKeys.Contains(newMapTemplate.TemplateKey))
            return;

        var aislingsToKill = Subject.GetEntitiesAtPoints<Aisling>(CurrentMapPoints)
                                    .Where(x => NextMapPoints.Contains(x))
                                    .ToList();

        foreach (var aisling in aislingsToKill)
        {
            var damage = (int)(aisling.StatSheet.EffectiveMaximumHp * 1000);

            ApplyDamageScript.ApplyDamage(
                aisling,
                aisling,
                this,
                damage);
        }
    }

    private void PerformMorph() => MorphMap();

    private void SendMessageToAllPlayers(string message)
    {
        var allPlayers = Subject.GetEntities<Aisling>()
                                .ToList();

        foreach (var player in allPlayers)
            player.SendActiveMessage(message);
    }

    private bool ShouldPerformMorph() => CountdownMorphStarted && CountdownComplete && (MorphCount != MorphTemplateKeys.Count);

    private bool ShouldStartCountdown()
        => AnnouncedMorphStart.HasValue
           && (DateTime.UtcNow.Subtract(AnnouncedMorphStart.Value)
                       .TotalSeconds
               >= 15)
           && (MorphCount < MorphTemplateKeys.Count);

    private bool ShouldStartMorphAnnouncement() => !AnnounceMorph && (MorphCount < MorphTemplateKeys.Count);

    public override void Update(TimeSpan delta)
    {
        CaptureMapWalls();

        if (ShouldStartMorphAnnouncement())
            AnnouncedMorph();

        if (ShouldAnimateTiles)
            AnimateTiles(delta);

        if (ShouldStartCountdown())
            HandleCountdown();

        if (ShouldPerformMorph())
            PerformMorph();
    }
}