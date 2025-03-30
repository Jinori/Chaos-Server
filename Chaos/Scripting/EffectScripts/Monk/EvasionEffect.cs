using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Monk;

public class EvasionEffect : EffectBase
{
    protected int ArmorClassSaved;
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(10);
    public override byte Icon => 18;
    public override string Name => "Evasion";

    public int GetAcReduction()
    {
        // Base AC reduction formula
        var baseReduction = -3 - 15.0 / 98.0 * (Subject.StatSheet.Level - 1);

        // Additional reduction for every 15k HP
        var bonusReduction = Subject.StatSheet.MaximumHp / 15000;

        return (int)Math.Round(baseReduction - bonusReduction);
    }

    public override void OnApplied()
    {
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

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (source.Effects.Contains("Dodge"))
        {
            source.Effects.Dispel("Dodge");

            return true;
        }

        if (source.Effects.Contains("Evasion"))
        {
            source.Effects.Dispel("Evasion");

            return true;
        }

        return true;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Subject is Aisling aisling && aisling.Equipment.Contains(3))
        {
            if (aisling.Effects.Contains("Dodge"))
                aisling.Effects.Dispel("Dodge");

            if (aisling.Effects.Contains("Evasion"))
                aisling.Effects.Dispel("Evasion");

            AislingSubject?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "A physical shield prevents your ability to dodge.");
        }
    }
}