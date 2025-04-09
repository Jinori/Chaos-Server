using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts;
using Chaos.Scripting.MonsterScripts.Boss;

namespace Chaos.Utilities;

public static class DamageHelper
{
    public static int CalculatePercentDamage(Creature source, Creature target, decimal percent, bool useCurrent = false)
    {
        if (target.Script.Is<ThisIsAWorldBossScript>())
        {
            percent /= 10;

            if (percent > 0.5m)
                percent = 0.5m;
        } else if (target.Script.Is<ThisIsAMajorBossScript>())
        {
            percent /= 4;
            
            if (percent > 2.5m)
                percent = 2.5m;
        } else if (target.Script.Is<TrainingDummyScript>() || target.Script.Is<ThisIsABossScript>())
        {
            percent /= 2;

            if (percent > 5)
                percent = 5;
        }
        
        var hp = useCurrent ? target.StatSheet.CurrentHp : (int)target.StatSheet.EffectiveMaximumHp;

        return MathEx.GetPercentOf<int>(hp, percent);
    }
}