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

    private Attributes BonusAttributes = null!;

    public override byte Icon => 10;
    public override string Name => "Blessing";

    protected byte? Sound => 122;

    public override void OnApplied()
    {
        base.OnApplied();

        BonusAttributes = new Attributes
        {
            Ac = GetVar<int>("ac"),
            MagicResistance = GetVar<int>("magicResistance"),
            HealBonusPct = GetVar<int>("healBonusPct")
        };
        
        Subject.Animate(Animation!);
        Subject.StatSheet.AddBonus(BonusAttributes);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
    }

    public override void OnDispelled() => OnTerminated();

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source)
    {
        SetVar("ac", -15);
        SetVar("magicResistance", 20);
        SetVar("healBonusPct", 25);
    }

    public override void OnTerminated()
    {
        Subject.StatSheet.SubtractBonus(BonusAttributes);
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