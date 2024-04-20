using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public sealed class PetDeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PetDeathScript(Monster subject)
        : base(subject) { }

    public override void OnDeath()
    {
        Subject.PetOwner?.Trackers.TimedEvents.AddEvent("PetDeath", TimeSpan.FromMinutes(5), true);
        Subject.PetOwner = null;
        Map.RemoveEntity(Subject);
    }
}