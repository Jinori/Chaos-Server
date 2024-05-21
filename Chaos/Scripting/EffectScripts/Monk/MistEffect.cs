using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class MistEffect : EffectBase
{
    protected int ArmorClassSaved;
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    public override byte Icon => 18;
    public override string Name => "mist";
    public int GetAcReduction() => (int)Math.Round(-3 - (15.0 / 98.0) * (Subject.StatSheet.Level - 1));

    public override void OnApplied()
    {
        base.OnApplied();
        ArmorClassSaved = GetAcReduction();

        var attributesToAdd = new Attributes
        {
            Ac = ArmorClassSaved,
            MagicResistance = -20
        };
        
        Subject.StatSheet.AddBonus(attributesToAdd);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor increased while magic resist decreased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributesToSubtract = new Attributes
        {
            MagicResistance = 20,
            Ac = ArmorClassSaved
        };

        Subject.StatSheet.SubtractMp(100);
        Subject.StatSheet.SubtractBonus(attributesToSubtract);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor and MR has returned to normal.");
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.Effects.Contains("mist"))
        {
            source.Effects.Dispel("mist");

            return true;
        }

        return true;
    }
}