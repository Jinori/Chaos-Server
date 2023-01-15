using Chaos.Common.Definitions;
using Chaos.Containers;
using Chaos.Data;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.MapScripts
{
    public class TerrorOfTheCryptScript : MapScriptBase
    {
        private enum ScriptState
        {
            Dormant,
            DelayedStart,
            Spawning,
            Spawned
        }

        private readonly IMonsterFactory MonsterFactory;
        private ScriptState State;
        private DateTime? StartTime;
        private TimeSpan StartDelay;
        private IIntervalTimer AnimationInterval;
        private IRectangle AnimationShape;
        private List<Point> ShapeOutline;
        private List<Point> ReverseOutline;
        private int AnimationIndex;
        private Animation Animation;

    public TerrorOfTheCryptScript(MapInstance subject, IMonsterFactory monsterFactory) : base(subject)
        {
            MonsterFactory = monsterFactory;
            StartDelay = TimeSpan.FromSeconds(5);
            AnimationInterval = new IntervalTimer(TimeSpan.FromMilliseconds(200));
            AnimationShape = new Rectangle(new Point(8, 8), 5, 5);
            ShapeOutline = AnimationShape.GetOutline().ToList();
            ReverseOutline = ShapeOutline.AsEnumerable().Reverse().ToList();
            Animation = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 13
            };
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            if (aisling.Flags.HasFlag(QuestFlag1.TerrorOfCryptHunt) && State == ScriptState.Dormant)
                State = ScriptState.DelayedStart;
        }


        public override void Update(TimeSpan delta)
        {
            //someone might relog in this room, need to handle that
            switch (State)
            {
                case ScriptState.DelayedStart:
                    StartTime ??= DateTime.UtcNow;

                    if(DateTime.UtcNow - StartTime > StartDelay)
                    {
                        StartTime = null;
                        State = ScriptState.Spawning;

                        foreach(var aisling in Subject.GetEntities<Aisling>())
                            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You hear a creak, as the coffin begins to slide open...");
                    }

                    break;
                case ScriptState.Spawning:
                    AnimationInterval.Update(delta);

                    if (!AnimationInterval.IntervalElapsed)
                        return;

                    var pt1 = ShapeOutline[AnimationIndex];
                    var pt2 = ReverseOutline[AnimationIndex];

                    Subject.ShowAnimation(Animation.GetPointAnimation(pt1));
                    Subject.ShowAnimation(Animation.GetPointAnimation(pt2));

                    AnimationIndex++;

                    if (AnimationIndex >= ShapeOutline.Count)
                    {
                        var monster = MonsterFactory.Create("terrorLowInsight", Subject, new Point(8, 8));
                        Subject.AddObject(monster, monster);
                        State = ScriptState.Spawned;
                        AnimationIndex = 0;
                    }

                    break;
                case ScriptState.Spawned:

                    if(Subject.GetEntities<Aisling>().Count() == 0)
                    {
                        var monsters = Subject.GetEntities<Monster>().ToList(); ;

                        //if we are transitioning to a dormant state, remove any lingering monsters
                        //this could potentially happen if both players logged out
                        //when they log back in the script will start over
                        foreach (var monster in monsters)
                            Subject.RemoveObject(monster);

                        State = ScriptState.Dormant;
                    }

                    break;
            }
        }
    }
}
