using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class ExpStatBuyingScript(Dialog subject) : DialogScriptBase(subject)
{

    private static void IncreaseAttribute(
        Aisling source,
        string attribute,
        long expCost,
        Action<Attributes> update
    )
    {
        if (source.UserStatSheet.TotalExp >= expCost)
        {
            update(source.UserStatSheet);

            if (!source.UserStatSheet.TrySubtractTotalExp(expCost))
            {
                source.SendOrangeBarMessage($"Error trying to take {expCost}.");
                return;
            }
            
            source.SendOrangeBarMessage($"{attribute} increased by one. {expCost} experience used.");
            source.Client.SendAttributes(StatUpdateType.Full);
        }
        else
        {
            source.SendOrangeBarMessage("Not enough experience to increase this stat.");
            source.Client.SendAttributes(StatUpdateType.Full);
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
        
        if (ClassStatCaps.TryGetValue(source.UserStatSheet.BaseClass, out var statCapsByBracket))
        {
            var currentBracket = GetCurrentStatBracket(source);

            if (statCapsByBracket.TryGetValue(currentBracket, out var currentCaps))
            {
                if (OptionActionMappings.TryGetValue(optionIndex.Value, out var action))
                {
                    if (IsStatCapped(source, currentCaps, optionIndex.Value))
                    {
                        source.SendOrangeBarMessage("You've reached the stat cap for this attribute.");
                        return;
                    }
                    
                    var expCost = CalculateExpCost(source, optionIndex.Value);
                    
                    action(source, expCost);
                    source.Client.SendAttributes(StatUpdateType.Full);
                }
            }
        }
    }

    private long CalculateExpCost(Aisling source, byte optionIndex)
    {
        var currentStatValue = optionIndex switch
        {
            1 => source.UserStatSheet.Str,
            2 => source.UserStatSheet.Int,
            3 => source.UserStatSheet.Wis,
            4 => source.UserStatSheet.Con,
            5 => source.UserStatSheet.Dex,
            _ => 0
        };

        return currentStatValue * 75000;
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
    
    private readonly Dictionary<byte, Action<Aisling, long>> OptionActionMappings = new()
    {
        { 1, (source, expCost) => IncreaseAttribute(source, "Strength", expCost, attribute => attribute.Str++) },
        { 2, (source, expCost) => IncreaseAttribute(source, "Intelligence", expCost, attribute => attribute.Int++) },
        { 3, (source, expCost) => IncreaseAttribute(source, "Wisdom", expCost, attribute => attribute.Wis++) },
        { 4, (source, expCost) => IncreaseAttribute(source, "Constitution", expCost, attribute => attribute.Con++) },
        { 5, (source, expCost) => IncreaseAttribute(source, "Dexterity", expCost, attribute => attribute.Dex++) }
    };

    
    private ClassStatBracket GetCurrentStatBracket(Aisling source)
    {
        if (source.UserStatSheet.Master)
            return ClassStatBracket.Master;

        if (source.UserStatSheet.Master && (source.UserStatSheet.MaximumHp +  (source.UserStatSheet.MaximumMp * 2) >= 80000))
            return ClassStatBracket.Grandmaster;

        return ClassStatBracket.PreMaster;
    }

    private readonly Dictionary<BaseClass, Dictionary<ClassStatBracket, Attributes>> ClassStatCaps = new()
    {
        {
            BaseClass.Warrior, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 120, Int = 50, Wis = 50, Con = 80, Dex = 100 } },
                { ClassStatBracket.Master, new Attributes { Str = 180, Int = 80, Wis = 80, Con = 120, Dex = 150 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 215, Int = 100, Wis = 100, Con = 150, Dex = 180 } }
            }
        },
        {
            BaseClass.Monk, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 100, Int = 50, Wis = 50, Con = 120, Dex = 80 } },
                { ClassStatBracket.Master, new Attributes { Str = 150, Int = 80, Wis = 80, Con = 180, Dex = 120 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 180, Int = 100, Wis = 100, Con = 215, Dex = 150 } }
            }
        },
        {
            BaseClass.Rogue, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 100, Int = 50, Wis = 50, Con = 80, Dex = 120 } },
                { ClassStatBracket.Master, new Attributes { Str = 150, Int = 80, Wis = 80, Con = 120, Dex = 180 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 180, Int = 100, Wis = 100, Con = 150, Dex = 215 } }
            }
        },
        {
            BaseClass.Wizard, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 50, Int = 120, Wis = 100, Con = 80, Dex = 50 } },
                { ClassStatBracket.Master, new Attributes { Str = 80, Int = 180, Wis = 150, Con = 120, Dex = 80 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 100, Int = 215, Wis = 180, Con = 150, Dex = 100 } }
            }
        },
        {
            BaseClass.Priest, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 50, Int = 100, Wis = 120, Con = 80, Dex = 50 } },
                { ClassStatBracket.Master, new Attributes { Str = 80, Int = 150, Wis = 180, Con = 120, Dex = 80 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 100, Int = 180, Wis = 215, Con = 150, Dex = 100 } }
            }
        },
    };
}
