using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class ChiAnkletScript : ConfigurableItemScriptBase
{
    protected AdvClass AdvClass { get; init; } = AdvClass.None;
    protected BaseClass BaseClass { get; init; } = BaseClass.Monk;
    protected EquipmentType EquipmentType { get; init; }
    protected Gender Gender { get; init; } = Gender.Unisex;
    protected int? MinLevel { get; init; }
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }

    public ChiAnkletScript(Item subject)
        : base(subject) { }

    public override void OnEquipped(Aisling source)
    {
        var ac = 0;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac1))
            ac = 1;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac2))
            ac = 2;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac3))
            ac = 3;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac4))
            ac = 4;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac5))
            ac = 5;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac6))
            ac = 6;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac7))
            ac = 7;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac8))
            ac = 8;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac9))
            ac = 9;

        var attributes = new Attributes
        {
            Ac = ac
        };

        source.StatSheet.SubtractBonus(attributes);
        source.Client.SendAttributes(StatUpdateType.Secondary);
    }

    public override void OnUnEquipped(Aisling source)
    {
        var ac = 0;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac1))
            ac = 1;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac2))
            ac = 2;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac3))
            ac = 3;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac4))
            ac = 4;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac5))
            ac = 5;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac6))
            ac = 6;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac7))
            ac = 7;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac8))
            ac = 8;

        if (source.Trackers.Flags.HasFlag(ChiAnkletFlags.Ac9))
            ac = 9;

        var attributes = new Attributes
        {
            Ac = ac
        };

        source.StatSheet.AddBonus(attributes);
        source.Client.SendAttributes(StatUpdateType.Secondary);
    }

    public override void OnUse(Aisling source)
    {
        if (!source.IsAlive)
        {
            source.SendOrangeBarMessage("You can't do that");

            return;
        }

        //gender check
        if ((Gender != source.Gender) && (Gender != Gender.Unisex))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if ((source.UserStatSheet.BaseClass != BaseClass.Diacht) && (BaseClass != source.UserStatSheet.BaseClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if ((AdvClass != AdvClass.None) && (AdvClass != source.UserStatSheet.AdvClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (MinLevel.HasValue && (MinLevel.Value > source.UserStatSheet.Level))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        source.Equip(EquipmentType, Subject);
    }
}