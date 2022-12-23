using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Common.Utilities;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using NLog.Targets;

namespace Chaos.Formulae.Regen;

public sealed class DefaultRegenFormula : IRegenFormula
{
    /// <inheritdoc />
    public int CalculateHealthRegen(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling:
                {
                    int percentToAdd = 10;
                    if (aisling.Status.HasFlag(Status.InnerFire))
                    {
                        percentToAdd += 8;
                    }
                    if (!aisling.Effects.Contains("chiBlocker") && aisling.Equipment.TryGetObject((byte)EquipmentSlot.Boots, out var boots) && boots.Template.TemplateKey.EqualsI("chiAnklet"))
                    {
                        var chiBlock = (ChiAnkletFlags)aisling.Flags.GetFlag<ChiAnkletFlags>();
                        chiBlock &= ChiAnkletFlags.IncreaseRegen1 | ChiAnkletFlags.IncreaseRegen2 | ChiAnkletFlags.IncreaseRegen3 | ChiAnkletFlags.IncreaseRegen4 | ChiAnkletFlags.IncreaseRegen5;
                        switch (chiBlock)
                        {
                            case ChiAnkletFlags.IncreaseRegen1:
                                {
                                    percentToAdd += 1;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen2:
                                {
                                    percentToAdd += 2;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen3:
                                {
                                    percentToAdd += 3;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen4:
                                {
                                    percentToAdd += 4;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen5:
                                {
                                    percentToAdd += 5;
                                }
                                break;
                        }
                    }
                    return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, percentToAdd);
                }
            case Monster:
                return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, 3);
            case Merchant:
                return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumHp, 100);

            default:
                throw new ArgumentOutOfRangeException(nameof(creature), creature, null);
        }
    }

    /// <inheritdoc />
    public int CalculateIntervalSecs(Creature creature) => 6;

    /// <inheritdoc />
    public int CalculateManaRegen(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling:
                {
                    int percentToAdd = 5;
                    if (aisling.Equipment[EquipmentSlot.Boots] is not null && aisling.Equipment[EquipmentSlot.Boots]!.Template!.TemplateKey.EqualsI("chiAnklet"))
                    {
                        var chiBlock = (ChiAnkletFlags)aisling.Flags.GetFlag<ChiAnkletFlags>();
                        chiBlock &= ChiAnkletFlags.IncreaseRegen1 | ChiAnkletFlags.IncreaseRegen2 | ChiAnkletFlags.IncreaseRegen3 | ChiAnkletFlags.IncreaseRegen4 | ChiAnkletFlags.IncreaseRegen5;
                        switch (chiBlock)
                        {
                            case ChiAnkletFlags.IncreaseRegen1:
                                {
                                    percentToAdd += 1;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen2:
                                {
                                    percentToAdd += 2;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen3:
                                {
                                    percentToAdd += 3;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen4:
                                {
                                    percentToAdd += 4;
                                }
                                break;
                            case ChiAnkletFlags.IncreaseRegen5:
                                {
                                    percentToAdd += 5;
                                }
                                break;
                        }
                    }
                    return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, percentToAdd);
                }
            case Monster:
                return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, 3);
            case Merchant:
                return MathEx.GetPercentOf<int>((int)creature.StatSheet.EffectiveMaximumMp, 100);

            default:
                throw new ArgumentOutOfRangeException(nameof(creature), creature, null);
        }
    }
}