using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class StatBuyingScript : DialogScriptBase
{
    
    private readonly ILogger<StatBuyingScript> Logger;
    
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
        // Add other classes here
    };

    public readonly Dictionary<byte, Action<Aisling, Attributes>> OptionActionMappings;

    public StatBuyingScript(Dialog subject, ILogger<StatBuyingScript> logger)
        : base(subject)
    {
        OptionActionMappings = new Dictionary<byte, Action<Aisling, Attributes>>
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

        Logger = logger;
    }

    private void IncreaseAttribute(
        Aisling source,
        string attribute,
        Attributes cost,
        Action<Attributes> update
    )
    {
        update(cost);
        source.UserStatSheet.Add(cost);
        source.SendOrangeBarMessage($"{attribute} increased by one.");
        
        Logger.WithTopics(
                  Topics.Entities.Aisling, Topics.Entities.Dialog, Topics.Actions.Reward)
              .WithProperty(Subject)
              .LogInformation("{@AislingName} has bought a {@Attribute} stat", source.Name, attribute);
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

        var statBuyCost = new Attributes { MaximumHp = -150 };

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

        if (source.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
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