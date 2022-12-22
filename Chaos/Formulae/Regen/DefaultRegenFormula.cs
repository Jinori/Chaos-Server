using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    /// <inheritdoc />
    public int CalculateIntervalSecs(Aisling aisling) => 6;

    /// <inheritdoc />
    public void Regenerate(Aisling aisling)
    {
        if (!aisling.IsAlive)
            return;

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
    }
}