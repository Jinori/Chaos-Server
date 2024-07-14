using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossHealAlliesScript : MonsterScriptBase
{
    private IIntervalTimer HealTimer { get; }
    private const float HEAL_AMOUNT_PERCENT = 0.2f; // Heals 20% of max health
    private const int HEAL_RANGE = 8; // Heal allies within 8 tiles
    private Animation HealAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 127
    };
    
    
    public BossHealAlliesScript(Monster subject)
        : base(subject)
    {
        HealTimer = new IntervalTimer(TimeSpan.FromSeconds(20));
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        HealTimer.Update(delta);

        if (HealTimer.IntervalElapsed)
        {
            HealAllies();
        }
    }

    private void HealAllies()
    {
        var allies = Map.GetEntitiesWithinRange<Monster>(Subject, HEAL_RANGE)
                        .Where(ally => (ally != Subject) && ally.IsAlive)
                        .ToList();

        foreach (var ally in allies)
        {
            var healAmount = (int)(ally.StatSheet.MaximumHp * HEAL_AMOUNT_PERCENT);
            ally.StatSheet.AddHp(healAmount);
            ally.ShowHealth();
            ally.Animate(HealAnimation);
        }

        Subject.Say("I shall heal your wounds, allies!");
    }
}