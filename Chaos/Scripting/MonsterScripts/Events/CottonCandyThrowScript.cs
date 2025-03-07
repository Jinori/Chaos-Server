using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;

namespace Chaos.Scripting.MonsterScripts.Events
{
    public class CottonCandyThrowScript : MonsterScriptBase
    {
        private readonly IIntervalTimer _knockbackTimer = new IntervalTimer(TimeSpan.FromMilliseconds(300));
        
        public CottonCandyThrowScript(Monster subject)
            : base(subject) { }
        

        public override void Update(TimeSpan delta)
        {
            _knockbackTimer.Update(delta);

            if (!_knockbackTimer.IntervalElapsed)
                return;

            var nearbyAislings = Map.GetEntitiesWithinRange<Aisling>(Subject, 2)
                                    .ThatAreAlive()
                                    .ToList();

            foreach (var aisling in nearbyAislings)
            {
                ApplyKnockback(aisling);
            }
        }

        private void ApplyKnockback(Aisling aisling)
        {
            var distance = Subject.ManhattanDistanceFrom(aisling);
            var pushDistance = distance switch
            {
                0 => 3,  // Direct hit → Strongest knockback
                1 => 2,  // Nearby → Moderate knockback
                _ => 0   // Outside range → No push
            };

            if (pushDistance <= 0)
                return;

            var pushDirection = GetKnockbackDirection(aisling);
            MoveAisling(aisling, pushDirection, pushDistance);
        }

        private Direction GetKnockbackDirection(Aisling aisling)
        {
            var dx = aisling.X - Subject.X;
            var dy = aisling.Y - Subject.Y;

            var possibleDirections = new List<Direction>();

            switch (dx)
            {
                case > 0:
                    possibleDirections.Add(Direction.Right);
                    break;
                case < 0:
                    possibleDirections.Add(Direction.Left);
                    break;
            }

            switch (dy)
            {
                case > 0:
                    possibleDirections.Add(Direction.Down);
                    break;
                case < 0:
                    possibleDirections.Add(Direction.Up);
                    break;
            }

            return possibleDirections.Count == 0 ? Direction.Invalid : possibleDirections[Random.Shared.Next(possibleDirections.Count)];
        }


        private void MoveAisling(Aisling aisling, Direction direction, int distance)
        {
            if (direction == Direction.Invalid)
                return;

            for (var i = 0; i < distance; i++)
            {
                var nextPosition = aisling.DirectionalOffset(direction);

                if (Map.IsWalkable(nextPosition, aisling.Type))
                {
                    aisling.WarpTo(nextPosition);
                    aisling.Animate(Throw);
                    aisling.SendOrangeBarMessage("You got wrapped up in cotton candy! Yum!");
                }
                else
                {
                    break;
                }
            }
        }

        
        private Animation Throw { get; } = new()
        {
            AnimationSpeed = 100,
            TargetAnimation = 19
        };
    }
}
