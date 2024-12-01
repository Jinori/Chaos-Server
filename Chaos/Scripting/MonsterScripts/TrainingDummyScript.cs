using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TrainingDummyScript(Monster subject) : MonsterScriptBase(subject)
{
    public override void OnAttacked(Creature attacker, int damage)
    {
        if (Subject.Effects.Contains("pramh"))
            Subject.Effects.Dispel("pramh");

        if (Subject.Effects.Contains("beag pramh"))
            Subject.Effects.Dispel("beag pramh");

        if (Subject.Effects.Contains("Wolf Fang Fist"))
            Subject.Effects.Dispel("Wolf Fang Fist");

        if (Subject.Effects.Contains("Amnesia"))
            Subject.Effects.Dispel("Amensia");

        if (attacker is Aisling aisling)
            Subject.Say($"{aisling.Name} did {damage} damage.");
    }
}