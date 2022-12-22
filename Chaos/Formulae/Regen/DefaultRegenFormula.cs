using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Common.Utilities;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    /// <inheritdoc />
    public int CalculateHealthRegen(Creature creature)
    {
        var percentToRegenerate = creature switch
        {
            Aisling  => 10,
            Monster  => 3,
            Merchant => 100,
            _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
        };

        return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, percentToRegenerate);
    }

    /// <inheritdoc />
    public int CalculateIntervalSecs(Creature creature) => 6;

        if (aisling.Status.HasFlag(Status.InnerFire))
        {
            aisling.StatSheet.AddHealthPct(8);
        }
        if (aisling.Equipment[EquipmentSlot.Boots] is not null && aisling.Equipment[EquipmentSlot.Boots]!.Template!.TemplateKey.EqualsI("chiAnklet"))
        {
            var chiBlock = (ChiAnkletFlags)aisling.Flags.GetFlag<ChiAnkletFlags>();
            chiBlock &= ChiAnkletFlags.IncreaseRegen1 | ChiAnkletFlags.IncreaseRegen2 | ChiAnkletFlags.IncreaseRegen3 | ChiAnkletFlags.IncreaseRegen4 | ChiAnkletFlags.IncreaseRegen5;
            switch (chiBlock)
            {
                case ChiAnkletFlags.IncreaseRegen1:
                    {
                        aisling.StatSheet.AddHealthPct(1);
                        aisling.StatSheet.AddManaPct(1);
                    }
                    break;
                case ChiAnkletFlags.IncreaseRegen2:
                    {
                        aisling.StatSheet.AddHealthPct(2);
                        aisling.StatSheet.AddManaPct(2);
                    }
                    break;
                case ChiAnkletFlags.IncreaseRegen3:
                    {
                        aisling.StatSheet.AddHealthPct(3);
                        aisling.StatSheet.AddManaPct(3);
                    }
                    break;
                case ChiAnkletFlags.IncreaseRegen4:
                    {
                        aisling.StatSheet.AddHealthPct(4);
                        aisling.StatSheet.AddManaPct(4);
                    }
                    break;
                case ChiAnkletFlags.IncreaseRegen5:
                    {
                        aisling.StatSheet.AddHealthPct(5);
                        aisling.StatSheet.AddManaPct(5);
                    }
                    break;
            }
        }
        aisling.StatSheet.AddHealthPct(10);
        aisling.StatSheet.AddManaPct(5);
        aisling.Client.SendAttributes(StatUpdateType.Vitality);
    /// <inheritdoc />
    public int CalculateManaRegen(Creature creature)
    {
        var percentToRegenerate = creature switch
        {
            Aisling  => 5,
            Monster  => 1.5m,
            Merchant => 100,
            _        => throw new ArgumentOutOfRangeException(nameof(creature), creature, null)
        };

        return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, percentToRegenerate);
    }
}