using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Time;

namespace Chaos.Scripting.FunctionalScripts.LevelUp;

public class DefaultLevelUpScript : ScriptBase, ILevelUpScript
{
    public ILevelUpFormula LevelUpFormula { get; set; } = LevelUpFormulae.Default;

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultLevelUpScript));
    private readonly IItemFactory ItemFactory;

    public DefaultLevelUpScript(IItemFactory itemFactory)
    {
        ItemFactory = itemFactory;
    }

    /// <inheritdoc />
    public static ILevelUpScript Create() => FunctionalScriptRegistry.Instance.Get<ILevelUpScript>(Key);

    /// <inheritdoc />
    public virtual void LevelUp(Aisling aisling)
    {
        aisling.UserStatSheet.AddLevel();
        aisling.SendOrangeBarMessage("You level up!");
        aisling.UserStatSheet.GivePoints(2);

        if (aisling.UserStatSheet.Level < WorldOptions.Instance.MaxLevel)
        {
            var newTnl = LevelUpFormula.CalculateTnl(aisling);
            aisling.UserStatSheet.AddTnl(newTnl);
        }

        if (aisling.UserStatSheet.Level == WorldOptions.Instance.MaxLevel)
        {
            if (aisling.Trackers.Counters.TryGetValue("deathcounter", out var deathcount) && deathcount < 1)
            {
                aisling.Legend.AddOrAccumulate(new LegendMark(
                    "Denied death to the 99th Insight",
                    "notdying",
                    MarkIcon.Victory,
                    MarkColor.Green,
                    1,
                    GameTime.Now));

                var halo = ItemFactory.Create("halo");

                aisling.GiveItemOrSendToBank(halo);
                
                aisling.SendOrangeBarMessage("You've received a unique Legend Mark and accessory");
            }
        }

        var levelUpAttribs = LevelUpFormula.CalculateAttributesIncrease(aisling);

        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 79
        };

        aisling.UserStatSheet.Add(levelUpAttribs);
        aisling.UserStatSheet.SetMaxWeight(LevelUpFormula.CalculateMaxWeight(aisling));
        aisling.Animate(ani);
        aisling.Client.SendAttributes(StatUpdateType.Full);

        if (aisling.UserStatSheet.BaseClass is BaseClass.Priest)
            switch (aisling.StatSheet.Level)
            {
                case 10:
                    SetPetEnumAndMessage(aisling, PetSkillsAvailable.Level10);

                    break;
                case 25:
                    SetPetEnumAndMessage(aisling, PetSkillsAvailable.Level25);

                    break;
                case 40:
                    SetPetEnumAndMessage(aisling, PetSkillsAvailable.Level40);

                    break;
                case 55:
                    SetPetEnumAndMessage(aisling, PetSkillsAvailable.Level55);
                    
                    break;
                case 80:
                    SetPetEnumAndMessage(aisling, PetSkillsAvailable.Level85);

                    break;
            }
    }

    private void SetPetEnumAndMessage(Aisling aisling, PetSkillsAvailable tag)
    {
        aisling.Trackers.Flags.AddFlag(tag);
        aisling.SendActiveMessage("{=oA new pet skill is now available!");
    }
}