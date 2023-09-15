using Chaos.Common.Definitions;
using Chaos.Definitions;
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

    private readonly Dictionary<BaseClass, Dictionary<ClassStatBracket, Attributes>> ClassStatCaps = new()
    {
        {
            BaseClass.Warrior, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 150, Int = 60, Wis = 60, Con = 100, Dex = 115 } },
                { ClassStatBracket.Master, new Attributes { Str = 215, Int = 100, Wis = 100, Con = 150, Dex = 180 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 255, Int = 150, Wis = 150, Con = 180, Dex = 215 } }
            }
        },
        // Add other classes here
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

        // Get the appropriate stat caps based on class and ClassStatBracket
        if (ClassStatCaps.TryGetValue(source.UserStatSheet.BaseClass, out var statCapsByBracket))
        {
            var currentBracket = GetCurrentStatBracket(source); // Implement this method to determine the current ClassStatBracket
            
            if (statCapsByBracket.TryGetValue(currentBracket, out var currentCaps))
            {
                if (OptionActionMappings.TryGetValue(optionIndex.Value, out var action))
                {
                    // Check if the stat is already at or above the cap
                    if (IsStatCapped(source, currentCaps, optionIndex.Value))
                    {
                        source.SendOrangeBarMessage("You've reached the stat cap for this attribute.");
                        return;
                    }

                    // Perform the stat increase
                    action(source, statBuyCost);
                    source.Client.SendAttributes(StatUpdateType.Primary);
                }
            }
        }
    }
   
    private ClassStatBracket GetCurrentStatBracket(Aisling source)
    {
        if (source.UserStatSheet.Master)
            return ClassStatBracket.Master;

        if (source.UserStatSheet.Master)  //Change this to check for Grandmaster when implemented
            return ClassStatBracket.Grandmaster;

        return ClassStatBracket.PreMaster;
    }
    
    private bool IsStatCapped(Aisling source, Stats caps, byte optionIndex) =>
        optionIndex switch
        {
            1 => source.UserStatSheet.Str >= caps.Str,
            2 => source.UserStatSheet.Int >= caps.Int,
            3 => source.UserStatSheet.Wis >= caps.Wis,
            4 => source.UserStatSheet.Con >= caps.Con,
            5 => source.UserStatSheet.Dex >= caps.Dex,
            _ => false
        };
}