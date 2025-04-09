using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class TormentEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Torment",
            "Blessing",
            "Ard Naomh Aite",
            "Mor Naomh Aite",
            "Naomh Aite",
            "Beag Naomh Aite",
            "Mor Beannaich",
            "Beannaich",
            "Mor Fas Deireas",
            "Fas Deireas",
            "Motivate"
        ];

    private Attributes GetSnapshotAttributes
        => new()
        {
            Dmg = GetVar<int>("dmg"),
            Hit = GetVar<int>("hit"),
            AtkSpeedPct = GetVar<int>("atkSpeedPct"),
            CooldownReductionPct = GetVar<int>("cooldownReductionPct")
        };
    
    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 75,
        AnimationSpeed = 100
    };

    public override byte Icon => 87;
    public override string Name => "Torment";

    protected byte? Sound => 122;

    public override void OnApplied()
    {
        base.OnApplied();
        
        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(GetSnapshotAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        SetVar("dmg", 14);
        SetVar("hit", 22);
        SetVar("atkSpeedPct", 30);
        SetVar("cooldownReductionPct", 5);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(GetSnapshotAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Torment has faded.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }
}