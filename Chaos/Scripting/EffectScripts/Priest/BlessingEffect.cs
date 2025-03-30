using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Priest;

public class BlessingEffect : EffectBase, HierarchicalEffectComponent.IHierarchicalEffectComponentOptions
{
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(15);

    public List<string> EffectNameHierarchy { get; init; } =
        [
            "Blessing",
            "Torment",
            "Ard Naomh Aite",
            "Mor Naomh Aite",
            "Naomh Aite",
            "Beag Naomh Aite",
            "Armachd"
        ];

    protected Animation? Animation { get; } = new()
    {
        TargetAnimation = 512,
        AnimationSpeed = 100
    };

    public override byte Icon => 10;
    public override string Name => "Blessing";

    protected byte? Sound => 122;

    public override void OnApplied()
    {
        base.OnApplied();

        var attributes = new Attributes
        {
            Ac = -15,
            MagicResistance = 20,
            HealBonusPct = 25
        };

        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributes = new Attributes
        {
            Ac = -15,
            MagicResistance = 20,
            HealBonusPct = 25
        };

        Subject.StatSheet.SubtractBonus(attributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Blessing has faded.");
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<HierarchicalEffectComponent>();

        return execution is not null;
    }
}