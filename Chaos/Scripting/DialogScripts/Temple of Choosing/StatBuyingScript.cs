using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class StatBuyingScript : DialogScriptBase
{
    private readonly Dictionary<BaseClass, int> BaseClassHealthLimits = new()
    {
        { BaseClass.Warrior, 4149 },
        { BaseClass.Wizard, 3899 },
        { BaseClass.Priest, 3649 },
        { BaseClass.Monk, 6149 },
        { BaseClass.Rogue, 4399 }
    };

    private readonly Dictionary<BaseClass, int> BaseClassOffset = new()
    {
        { BaseClass.Warrior, 4000 },
        { BaseClass.Wizard, 3750 },
        { BaseClass.Priest, 3500 },
        { BaseClass.Monk, 6000 },
        { BaseClass.Rogue, 4250 }
    };

    private readonly Dictionary<byte, Action<Aisling, Attributes>> OptionActionMappings = new()
    {
        {
            1, (source, cost) => IncreaseAttribute(
                source,
                "Strength",
                cost,
                attribute => attribute.Str++)
        },
        {
            2, (source, cost) => IncreaseAttribute(
                source,
                "Intelligence",
                cost,
                attribute => attribute.Int++)
        },
        {
            3, (source, cost) => IncreaseAttribute(
                source,
                "Wisdom",
                cost,
                attribute => attribute.Wis++)
        },
        {
            4, (source, cost) => IncreaseAttribute(
                source,
                "Constitution",
                cost,
                attribute => attribute.Con++)
        },
        {
            5, (source, cost) => IncreaseAttribute(
                source,
                "Dexterity",
                cost,
                attribute => attribute.Dex++)
        }
    };

    public StatBuyingScript(Dialog subject)
        : base(subject) { }

    private static void IncreaseAttribute(
        Aisling source,
        string attribute,
        Attributes cost,
        Action<Attributes> update
    )
    {
        update(cost);
        source.UserStatSheet.Add(cost);
        source.SendOrangeBarMessage($"{attribute} increased by one to {cost}. 150 Health taken.");
    }

    public override void OnDisplaying(Aisling source)
    {
        var formula = (source.StatSheet.MaximumHp - BaseClassOffset[source.UserStatSheet.BaseClass]) / 150;

        if (formula > 0)
            Subject.InjectTextParameters(formula);
        else
        {
            Subject.Reply(source, "You cannot buy any stats at this time, with your current vitality.");
            Subject.Options.Clear();
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is null)
            return;

        if (source.UserStatSheet.Level < 99)
        {
            source.SendOrangeBarMessage("You cannot buy stats until you are of the 99th level.");
            Subject.Close(source);

            return;
        }

        if (source.StatSheet.MaximumHp <= BaseClassHealthLimits[source.UserStatSheet.BaseClass])
        {
            source.SendOrangeBarMessage("Doing so would make you too feeble to stand. Return with more health.");
            Subject.Close(source);

            return;
        }

        var statBuyCost = new Attributes { MaximumHp = 150 };
        source.StatSheet.Subtract(statBuyCost);

        if (OptionActionMappings.TryGetValue(optionIndex.Value, out var action))
            action(source, statBuyCost);

        source.Client.SendAttributes(StatUpdateType.Primary);
    }
}