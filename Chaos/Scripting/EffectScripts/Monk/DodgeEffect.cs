using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class DodgeEffect : EffectBase
{
    protected int ArmorClassSaved;
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    public override byte Icon => 18;
    public override string Name => "Dodge";
    public int GetAcReduction() => (int)Math.Round(-3 - (15.0 / 98.0) * (Subject.StatSheet.Level - 1));

    public override void OnApplied()
    {
        base.OnApplied();
        ArmorClassSaved = GetAcReduction();

        var attributesToAdd = new Attributes
        {
            Ac = ArmorClassSaved
        };
        var attributesToSubtract = new Attributes
        {
            MagicResistance = 15
        };
        
        Subject.StatSheet.AddBonus(attributesToAdd);
        Subject.StatSheet.SubtractBonus(attributesToSubtract);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor increased while Magic Resist decreased.");
    }

    public override void OnDispelled() => OnTerminated();

    public override void OnTerminated()
    {
        var attributesToAdd = new Attributes
        {
            MagicResistance = 15
        };
        var attributesToSubtract = new Attributes
        {
            Ac = ArmorClassSaved
        };
        
        Subject.StatSheet.AddBonus(attributesToAdd);
        Subject.StatSheet.SubtractBonus(attributesToSubtract);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Full);
        AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Armor and Magic Resist have returned to normal.");
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (Subject is Aisling aisling && aisling.Equipment.Contains(3))
        {
            aisling.Effects.Dispel("Dodge");
            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A physical shield prevents your magic.");
        }
    }
    
    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.Effects.Contains("Dodge"))
        {
            source.Effects.Dispel("Dodge");

            return true;
        }

        return true;
    }
}