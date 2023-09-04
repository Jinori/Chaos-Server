using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TrainingDummyScript : MonsterScriptBase
{
    public TrainingDummyScript(Monster subject)
        : base(subject) { }

    public override void OnAttacked(Creature attacker, int damage)
    {
        if (Subject.Effects.Contains("pramh"))
        {
            Subject.Status &= ~Status.Pramh;
            Subject.Effects.Dispel("pramh");
        }

        if (Subject.Effects.Contains("beagpramh"))
        {
            Subject.Status &= ~Status.Pramh;
            Subject.Effects.Dispel("beagpramh");
        }

        if (Subject.Effects.Contains("wolfFangFist"))
        {
            Subject.Effects.Dispel("wolfFangFist");
        }

        if (Subject.Effects.Contains("Amnesia"))
            Subject.Effects.Dispel("Amensia");

        // Multiply the damage by the damage multiplier (can be a decimal).

        if (attacker is Aisling aisling)
            Subject.Say($"{aisling.Name} did {damage} damage.");
    }
}