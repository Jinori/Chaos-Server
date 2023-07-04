using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public sealed class PetFollowScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PetFollowScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!ShouldMove || (Target != null))
            return;

        const double TIME_SPAN_SECONDS = 3;
        var now = DateTime.UtcNow;

        if (Subject.PetOwner is null)
            return;

        if (Subject.MapInstance != Subject.PetOwner.MapInstance)
        {
            Subject.TraverseMap(Subject.PetOwner.MapInstance, Subject.PetOwner);

            return;
        }

        var item = Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(Subject.PetOwner)
                          .FirstOrDefault(x => now - x.Creation > TimeSpan.FromSeconds(TIME_SPAN_SECONDS));

        if (item is not null)
            return;

        var playerDistance = Subject.PetOwner.DistanceFrom(Subject);

        switch (playerDistance)
        {
            case <= 4:
                Subject.Wander();

                break;
            case > 4 and < 13:
                Subject.Pathfind(new Point(Subject.PetOwner.X, Subject.PetOwner.Y));

                break;
            case >= 13:
                Subject.WarpTo(Subject.PetOwner);

                break;
        }

        Subject.MoveTimer.Reset();
    }
}