using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Formulae;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class ThunderStanceEffect : EffectBase
{
    private const int MAX_HITS = 5;

    public int Hits
    {
        get => GetVarOrDefault(nameof(Hits), 0);
        set => SetVar(nameof(Hits), value);
    }

    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    public override byte Icon => 94;
    public override string Name => "Thunder Stance";
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IEffectFactory EffectFactory;

    public void ApplyDamage(Creature target, int sourceDamage)
    {
        var damage = sourceDamage / 10;

        ApplyDamageScript.ApplyDamage(
            Source,
            Subject,
            this,
            damage,
            Element.Wind);
        
        Hits++;
        
        if(Hits >= MAX_HITS)
            Subject.Effects.Dispel(Name);

        if (IntegerRandomizer.RollChance(30))
        {
            var crit = EffectFactory.Create("crit");
            Subject.Effects.Apply(Source, crit);
        }
    }

    public ThunderStanceEffect(IEffectFactory effectFactory)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.ElementalEffect;
    }
    
    public override void OnApplied()
    {
        base.OnApplied();

        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your body is charged with electricity");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The charge fades");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (!target.Effects.Contains("Thunder Stance") && !target.Effects.Contains("Lightning Stance"))
            return true;

        (source as Aisling)?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A stance has already been applied.");

        return false;
    }
}