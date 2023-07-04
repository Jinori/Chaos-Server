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
            damage *= 2;
            Subject.Status &= ~Status.Pramh;
            Subject.Effects.Dispel("pramh");
        }

        if (Subject.Effects.Contains("beagpramh"))
        {
            damage *= 2;
            Subject.Status &= ~Status.Pramh;
            Subject.Effects.Dispel("beagpramh");
        }

        if (Subject.Effects.Contains("wolfFangFist"))
        {
            damage *= 2;
            Subject.Effects.Dispel("wolfFangFist");
        }

        if (Subject.Effects.Contains("Amnesia"))
            Subject.Effects.Dispel("Amensia");

        if (attacker is Aisling aisling)
            Subject.Say($"{aisling.Name} did {damage} damage.");
    }
}