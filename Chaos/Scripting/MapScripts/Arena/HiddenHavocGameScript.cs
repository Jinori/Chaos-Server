using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MapScripts.Arena;

public abstract class HiddenHavocGameScript : MapScriptBase
{
    //Shrinking
    private readonly List<IPoint> CurrentMapPoints = new();
    private readonly IEffectFactory EffectFactory;
    private readonly List<IPoint> NextMapPoints = new();
    private readonly ISimpleCache SimpleCache;
    private bool AislingsTouching;
    private DateTime? AnnouncedMorphStart;
    private bool AnnounceMorph;
    private bool CountdownMorphStarted;
    private int CountdownMorphStep;
    private DateTime LastCountdownMorphTime;
    private bool MapWallsCaptured;
    private int MorphCount;
    private double TimePassedSinceMainAnimationStart;
    private double TimePassedSinceTileAnimation;
    private Aisling? TouchOne;
    private Aisling? TouchTwo;
    private bool WinnerDeclared;
    public abstract bool IsHostPlaying { get; set; }
    public abstract string MorphOriginalTemplateKey { get; set; }
    public abstract List<string> MorphTemplateKeys { get; set; }

    public abstract bool ShouldMapShrink { get; set; }
    private IApplyDamageScript ApplyDamageScript { get; }

    private IIntervalTimer ArenaUpdateTimer { get; }

    private Animation BlowupAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 49
    };
    private Animation PreAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 214
    };

    protected HiddenHavocGameScript(MapInstance subject, ISimpleCache simpleCache, IEffectFactory effectFactory)
        : base(subject)
    {
        ArenaUpdateTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        SimpleCache = simpleCache;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        EffectFactory = effectFactory;
    }

    private void DeclareNoWinners()
    {
        WinnerDeclared = true;
        var allToPort = Subject.GetEntities<Aisling>().ToList();
        var allHosts = Subject.GetEntities<Aisling>().Where(IsHost);

        foreach (var player in allToPort)
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, "The round ended in a draw. Everyone died!");
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
        }

        foreach (var host in allHosts)
        {
            host.SendServerMessage(ServerMessageType.OrangeBar2, "The round ended in a draw. Everyone died!");
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            host.TraverseMap(mapInstance, new Point(12, 10));
        }

        Subject.RemoveScript<IMapScript, HiddenHavocGameScript>();
        ResetMaps();
    }

    private void DeclareWinners(IReadOnlyList<Aisling> winners)
    {
        var allToPort = Subject.GetEntities<Aisling>().ToList();
        var allHosts = Subject.GetEntities<Aisling>().Where(IsHost);

        var winningMessage = winners.Count switch
        {
            1 => $"{winners[0].Name} has won the round!",
            2 => $"{winners[0].Name} and {winners[1].Name} have won the round!",
            _ => "The round is over!"
        };

        foreach (var player in allToPort)
        {
            player.SendServerMessage(ServerMessageType.OrangeBar2, winningMessage);
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(12, 10));
        }

        foreach (var host in allHosts)
        {
            host.SendServerMessage(ServerMessageType.OrangeBar2, winningMessage);
            var mapInstance = SimpleCache.Get<MapInstance>("arena_underground");
            host.TraverseMap(mapInstance, new Point(12, 10));
        }

        Subject.RemoveScript<IMapScript, HiddenHavocGameScript>();

        WinnerDeclared = true;
        ResetMaps();
    }

    private List<Aisling> GetAliveAislings(IEnumerable<Aisling> allAislings)
    {
        List<Aisling> aliveAislings;

        if (IsHostPlaying)
        {
            aliveAislings = allAislings
                            .Where(x => x.IsAlive)
                            .ToList();

            return aliveAislings;
        }

        aliveAislings = allAislings
                        .Where(x => x.IsAlive && (!x.Trackers.Enums.TryGetValue(out ArenaHost value) || (value == ArenaHost.None)))
                        .ToList();

        return aliveAislings;
    }

    private bool IsHost(Aisling aisling) =>
        !aisling.Trackers.Enums.TryGetValue(out ArenaHost value) || ((value != ArenaHost.Host) && (value != ArenaHost.MasterHost));

    private void MorphMap()
    {
        var templateKey = MorphTemplateKeys[Math.Min(MorphCount, MorphTemplateKeys.Count - 1)];
        Subject.Morph(templateKey);
        MorphCount++;
    }

    /// <inheritdoc />
    public override void OnEntered(Creature creature)
    {
        var hide = EffectFactory.Create("HiddenHavocHide");
        creature.Effects.Apply(creature, hide);
    }

    private void ResetMaps() => Subject.Morph(MorphOriginalTemplateKey);

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ArenaUpdateTimer.Update(delta);

        if (ShouldMapShrink)
        {
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

        if (!AislingsTouching)
        {
            var aislings = Subject.GetEntities<Aisling>().Where(x => x.IsAlive).ToList();

            foreach (var aisling in aislings)
            {
                var otherAisling = aislings.FirstOrDefault(other => !other.Equals(aisling) && (aisling.DistanceFrom(other) <= 1));

                if (otherAisling != null)
                {
                    TouchOne = aisling;
                    TouchTwo = otherAisling;
                    AislingsTouching = true;

                    break;
                }
            }
        }
        else
        {
            if ((TouchOne != null) && (TouchTwo != null))
            {
                // Aislings are caught touching and map will shrink
                ApplyDamageScript.ApplyDamage(
                    TouchOne,
                    TouchTwo,
                    this,
                    500000);

                ApplyDamageScript.ApplyDamage(
                    TouchTwo,
                    TouchOne,
                    this,
                    500000);

                Subject.ShowAnimation(BlowupAnimation.GetPointAnimation(TouchOne));
                Subject.ShowAnimation(BlowupAnimation.GetPointAnimation(TouchTwo));

                TouchOne = null;
                TouchTwo = null;
                AislingsTouching = false;
            }
        }

        if (!ArenaUpdateTimer.IntervalElapsed)
            return;

        if (!WinnerDeclared)
        {
            var allAislings = Subject.GetEntities<Aisling>().ToList();
            var aliveAislings = GetAliveAislings(allAislings);

            switch (aliveAislings.Count)
            {
                case 0:
                    DeclareNoWinners();

                    break;
                case <= 2:
                    DeclareWinners(aliveAislings);

                    break;
            }
        }
    }
}