using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class MotivateEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(2);

    private Attributes BonusAttributes = null!;
    
    /// <inheritdoc />
    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Torment",
            "Motivate"
        ];

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 516,
        AnimationSpeed = 100
    };

    public override byte Icon => 99;
    public override string Name => "Motivate";
    protected byte? Sound => 121;

    public override void OnApplied()
    {
        base.OnApplied();

        BonusAttributes = new Attributes
        {
            AtkSpeedPct = GetVar<int>("atkSpeedPct")
        };
        
        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source) => SetVar("atkSpeedPct", 25);

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Attack Speed has returned to normal.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }
}