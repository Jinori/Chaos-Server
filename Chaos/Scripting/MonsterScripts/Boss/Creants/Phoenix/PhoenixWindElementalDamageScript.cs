using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Phoenix;

public class PhoenixWindElementalDamageScript : MonsterScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript = ApplyNonAttackDamageScript.Create();
    private readonly IIntervalTimer DamageTimer = new IntervalTimer(TimeSpan.FromMilliseconds(100));
    
    /// <inheritdoc />
    public PhoenixWindElementalDamageScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        DamageTimer.Update(delta);

        if (DamageTimer.IntervalElapsed)
        {
            var nearbyAislings = Map.GetEntitiesWithinRange<Aisling>(Subject, 2)
                                    .ThatAreAlive()
                                    .ToList();

            foreach (var aisling in nearbyAislings)
            {
                var distance = Subject.ManhattanDistanceFrom(aisling);
                var damage = distance switch
                {
                    0 => MathEx.GetPercentOf<int>((int)aisling.StatSheet.EffectiveMaximumHp, 15),
                    1 => MathEx.GetPercentOf<int>((int)aisling.StatSheet.EffectiveMaximumHp, 10),
                    2 => MathEx.GetPercentOf<int>((int)aisling.StatSheet.EffectiveMaximumHp, 5),
                    _ => 0
                };
                
                ApplyDamageScript.ApplyDamage(Subject, aisling, this, damage);
            }
        }
    }
}