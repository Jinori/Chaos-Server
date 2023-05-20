using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class TrainingDummyScript : MonsterScriptBase
{
    public TrainingDummyScript(Monster subject) : base(subject)
    {
    }

    public override void OnAttacked(Creature attacker, int damage)
    {
        if (attacker is Aisling aisling)
        {
            Subject.Say($"{aisling.Name} did {damage} damage.");
        }
    }
}