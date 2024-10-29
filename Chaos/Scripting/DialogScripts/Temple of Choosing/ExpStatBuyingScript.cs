using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class ExpStatBuyingScript(Dialog subject) : DialogScriptBase(subject)
{
    private readonly Dictionary<BaseClass, Dictionary<ClassStatBracket, Attributes>> ClassStatCaps = new()
    {
        {
            BaseClass.Warrior, new Dictionary<ClassStatBracket, Attributes>
            {
                {
                    ClassStatBracket.PreMaster, new Attributes
                    {
                        Str = 120,
                        Int = 50,
                        Wis = 50,
                        Con = 80,
                        Dex = 100
                    }
                },
                {
                    ClassStatBracket.Master, new Attributes
                    {
                        Str = 180,
                        Int = 80,
                        Wis = 80,
                        Con = 120,
                        Dex = 150
                    }
                },
                {
                    ClassStatBracket.Grandmaster, new Attributes
                    {
                        Str = 215,
                        Int = 100,
                        Wis = 100,
                        Con = 150,
                        Dex = 180
                    }
                }
            }
        },
        {
            BaseClass.Monk, new Dictionary<ClassStatBracket, Attributes>
            {
                {
                    ClassStatBracket.PreMaster, new Attributes
                    {
                        Str = 100,
                        Int = 50,
                        Wis = 50,
                        Con = 120,
                        Dex = 80
                    }
                },
                {
                    ClassStatBracket.Master, new Attributes
                    {
                        Str = 150,
                        Int = 80,
                        Wis = 80,
                        Con = 180,
                        Dex = 120
                    }
                },
                {
                    ClassStatBracket.Grandmaster, new Attributes
                    {
                        Str = 180,
                        Int = 100,
                        Wis = 100,
                        Con = 215,
                        Dex = 150
                    }
                }
            }
        },
        {
            BaseClass.Rogue, new Dictionary<ClassStatBracket, Attributes>
            {
                {
                    ClassStatBracket.PreMaster, new Attributes
                    {
                        Str = 100,
                        Int = 50,
                        Wis = 50,
                        Con = 80,
                        Dex = 120
                    }
                },
                {
                    ClassStatBracket.Master, new Attributes
                    {
                        Str = 150,
                        Int = 80,
                        Wis = 80,
                        Con = 120,
                        Dex = 180
                    }
                },
                {
                    ClassStatBracket.Grandmaster, new Attributes
                    {
                        Str = 180,
                        Int = 100,
                        Wis = 100,
                        Con = 150,
                        Dex = 215
                    }
                }
            }
        },
        {
            BaseClass.Wizard, new Dictionary<ClassStatBracket, Attributes>
            {
                {
                    ClassStatBracket.PreMaster, new Attributes
                    {
                        Str = 50,
                        Int = 120,
                        Wis = 100,
                        Con = 80,
                        Dex = 50
                    }
                },
                {
                    ClassStatBracket.Master, new Attributes
                    {
                        Str = 80,
                        Int = 180,
                        Wis = 150,
                        Con = 120,
                        Dex = 80
                    }
                },
                {
                    ClassStatBracket.Grandmaster, new Attributes
                    {
                        Str = 100,
                        Int = 215,
                        Wis = 180,
                        Con = 150,
                        Dex = 100
                    }
                }
            }
        },
        {
            BaseClass.Priest, new Dictionary<ClassStatBracket, Attributes>
            {
                {
                    ClassStatBracket.PreMaster, new Attributes
                    {
                        Str = 50,
                        Int = 100,
                        Wis = 120,
                        Con = 80,
                        Dex = 50
                    }
                },
                {
                    ClassStatBracket.Master, new Attributes
                    {
                        Str = 80,
                        Int = 150,
                        Wis = 180,
                        Con = 120,
                        Dex = 80
                    }
                },
                {
                    ClassStatBracket.Grandmaster, new Attributes
                    {
                        Str = 100,
                        Int = 180,
                        Wis = 215,
                        Con = 150,
                        Dex = 100
                    }
                }
            }
        }
    };

    private readonly Dictionary<byte, Action<Aisling, long>> OptionActionMappings = new()
    {
        {
            1, (source, expCost) => ComplexActionHelper.BuyStatWithExp(source, Stat.STR, expCost)
        },
        {
            2, (source, expCost) => ComplexActionHelper.BuyStatWithExp(source, Stat.INT, expCost)
        },
        {
            3, (source, expCost) => ComplexActionHelper.BuyStatWithExp(source, Stat.WIS, expCost)
        },
        {
            4, (source, expCost) => ComplexActionHelper.BuyStatWithExp(source, Stat.CON, expCost)
        },
        {
            5, (source, expCost) => ComplexActionHelper.BuyStatWithExp(source, Stat.DEX, expCost)
        }
    };

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

        return currentStatValue * 75000 + 2000000;
    }

    private static ClassStatBracket GetCurrentStatBracket(Aisling source)
    {
        if (source.UserStatSheet.Master)
            return ClassStatBracket.Master;

        return source.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster) ? ClassStatBracket.Grandmaster : ClassStatBracket.PreMaster;
    }

    private bool IsStatCapped(Aisling source, Stats caps, byte optionIndex)
        => optionIndex switch
        {
            1 => source.UserStatSheet.Str >= caps.Str,
            2 => source.UserStatSheet.Int >= caps.Int,
            3 => source.UserStatSheet.Wis >= caps.Wis,
            4 => source.UserStatSheet.Con >= caps.Con,
            5 => source.UserStatSheet.Dex >= caps.Dex,
            _ => false
        };

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

        if (source.UserStatSheet.UnspentPoints > 19)
        {
            source.SendOrangeBarMessage("You cannot buy stats while having over 19 unspent stat points.");
            Subject.Close(source);

            return;
        }

        if (ClassStatCaps.TryGetValue(source.UserStatSheet.BaseClass, out var statCapsByBracket))
        {
            var currentBracket = GetCurrentStatBracket(source);

            if (statCapsByBracket.TryGetValue(currentBracket, out var currentCaps))
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