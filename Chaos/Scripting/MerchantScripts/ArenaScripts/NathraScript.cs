using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.ArenaScripts;

public sealed class NathraScript : MerchantScriptBase
{
    private readonly IIntervalTimer WalkTimer;
    private readonly IIntervalTimer AnimationTimer;
    public readonly Point FirstCheckPoint = new(5, 10);
    public readonly Point SecondCheckPoint = new(21, 10);
    public readonly Point ThirdCheckPoint = new(21, 31);
    public readonly Point FourthCheckPoint = new(39, 31);
    public readonly Point GreenWinPoint = new(39, 6);
    private readonly Random Random = new();
    private readonly Dictionary<Point, CheckpointState> CheckpointMap = new()
    {
        { new Point(5, 10), CheckpointState.AtFirstPoint },
        { new Point(21, 10), CheckpointState.AtSecondPoint },
        { new Point(21, 31), CheckpointState.AtThirdPoint },
        { new Point(39, 31), CheckpointState.AtFourthPoint },
        { new Point(39, 6), CheckpointState.AtGreenWinPoint }
    };

    private readonly Dictionary<int, List<string>> CheckpointSayings = new()
    {
    {
        1, [
            "Stars pause, eyes watching from the cosmic void.",
            "Veils between worlds thin and tear here easily.",
            "Whispers of ancient powers echo in this slumber.",
            "This ground is sacred, hallowed by forgotten gods.",
            "Can you hear the cosmic heart's ancient beat?",
            "Secrets of R'lyeh murmur in the shadows here.",
            "The gaze of the Great Old Ones feels near.",
            "Air vibrates with the Necronomicon's whispers.",
            "Eldritch shadows flicker, cosmic horrors loom.",
            "Arcane energies converge, reality's fabric thins."
        ]
    },
    {
        2, [
            "Destiny and time intertwine like ancient serpents.",
            "Deep and old shadows hold secrets best left hidden.",
            "Beneath our feet lie secrets from dawnless ages.",
            "A nexus of old, forgotten powers watches us.",
            "Echoes of ancient chants and rituals fill the air.",
            "Dagon's ancient murmurs seep through the earth.",
            "Eldritch runes mark this ground with dark pacts.",
            "Stones here breathe with the life of endless aeons.",
            "Cries of the void's lost souls echo in the wind.",
            "Cthulhu's dark shadow casts over this haunted place."
        ]
    },
    {
        3, [
            "Elder spirits, guide our path and shield from harm.",
            "At these crossroads, unseen cosmic currents converge.",
            "Aeons' cries resonate here, echoing past tragedies.",
            "Here, reality's boundaries grow faint and uncertain.",
            "Past, present, and future merge in dark harmony.",
            "Azathoth's unknowable mists shroud our cryptic path.",
            "Dread whispers of the ancients echo in the silence.",
            "Ground remembers Shub-Niggurath's dark offspring.",
            "Heavy is the air with interdimensional gates' breath.",
            "Nyarlathotep's mad laughter echoes within minds."
        ]
    },
    {
        4, [
            "Eyes that witnessed time's birth watch our journey.",
            "Ancient whispers fill the air, thick with secrets.",
            "Land bears the scars of long-forgotten gods' wars.",
            "Around us, unseen, mysterious forces swirl curiously.",
            "Echoes of ancient cosmic wars imprint the ether.",
            "Rising chants of the deep ones, ancient and relentless.",
            "Prehistoric rites' dark shadows darken the earth.",
            "Stars above are not ours, but from darker skies.",
            "Howls of Tindalos' hounds fill the air, timelessly.",
            "Forbidden knowledge wells up, seeping from the ground."
        ]
    }
};
    
    private readonly Dictionary<CheckpointState, (Direction nextDirection, CheckpointState nextState)> StateTransitionMap = new()
    {
        { CheckpointState.Idle, (Direction.Up, CheckpointState.WalkingToFirstPoint) },
        { CheckpointState.AtFirstPoint, (Direction.Right, CheckpointState.WalkingToSecondPoint) },
        { CheckpointState.AtSecondPoint, (Direction.Down, CheckpointState.WalkingToThirdPoint) },
        { CheckpointState.AtThirdPoint, (Direction.Right, CheckpointState.WalkingToFourthPoint) },
        { CheckpointState.AtFourthPoint, (Direction.Up, CheckpointState.WalkingToGreenWinPoint) }
    };
    
    public CheckpointState NathraState { get; set; }
    
    public enum CheckpointState
    {
        Idle,
        WalkingToFirstPoint,
        AtFirstPoint,
        WalkingToSecondPoint,
        AtSecondPoint,
        WalkingToThirdPoint,
        AtThirdPoint,
        WalkingToFourthPoint,
        AtFourthPoint,
        WalkingToGreenWinPoint,
        AtGreenWinPoint
    }
    
    private Animation Animation { get; } = new()
    {
        AnimationSpeed = 50,
        TargetAnimation = 96
    };

    /// <inheritdoc />
    public NathraScript(Merchant subject)
        : base(subject)
    {
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1200), false);
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
    }

    public void ChangeState(CheckpointState newState) => NathraState = newState;
    private void CheckAndUpdateCheckpoint()
    {
        var currentPoint = new Point(Subject.X, Subject.Y);

        if (CheckpointMap.TryGetValue(currentPoint, out var newState) && (NathraState != newState))
        {
            ChangeState(newState);
            if (newState is CheckpointState.AtFirstPoint or CheckpointState.AtSecondPoint or 
                            CheckpointState.AtThirdPoint or CheckpointState.AtFourthPoint)
            {
                DisplayRandomSayingAtCheckpoint(newState);
            }
        }
    }

    private void DisplayRandomSayingAtCheckpoint(CheckpointState state)
    {
        if (!CheckpointSayings.TryGetValue((int)state, out var sayings))
            return;

        var saying = sayings[Random.Next(sayings.Count)];
        Subject.Say(saying);
    }
    
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        AnimationTimer.Update(delta);
        WalkTimer.Update(delta);
        CheckAndUpdateCheckpoint();

        if (AnimationTimer.IntervalElapsed)
        {
            var points = AoeShape.AllAround.ResolvePoints(Subject, 3);
            var enumerable = points as Point[] ?? points.ToArray();
            
            foreach (var tile in enumerable)
                Subject.MapInstance.ShowAnimation(Animation.GetPointAnimation(tile));
        }

        if (WalkTimer.IntervalElapsed)
        {
            var points = AoeShape.AllAround.ResolvePoints(Subject, 3);
            var playersInAoe = points.SelectMany(point => Subject.MapInstance.GetEntitiesAtPoint<Aisling>(point)).ToList();

            var shouldMove = (playersInAoe.Count != 0) &&
                             playersInAoe.All(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value == ArenaTeam.Gold)) &&
                             playersInAoe.Any(x => x.Trackers.Enums.TryGetValue(out ArenaTeam value) && (value == ArenaTeam.Gold));

            if (shouldMove && StateTransitionMap.TryGetValue(NathraState, out var transition))
            {
                Subject.Direction = transition.nextDirection;
                if (NathraState != transition.nextState)
                    ChangeState(transition.nextState);
            }

            if (shouldMove && NathraState is CheckpointState.WalkingToFirstPoint or CheckpointState.WalkingToSecondPoint or CheckpointState.WalkingToThirdPoint
                                             or CheckpointState.WalkingToFourthPoint or CheckpointState.WalkingToGreenWinPoint)
                Subject.Walk(Subject.Direction);
        }
    }
}