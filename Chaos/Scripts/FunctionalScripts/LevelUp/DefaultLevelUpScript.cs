using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Formulae;
using Chaos.Formulae.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Services.Servers.Options;

namespace Chaos.Scripts.FunctionalScripts.LevelUp;

public class DefaultLevelUpScript : ScriptBase, ILevelUpScript
{
    public ILevelUpFormula LevelUpFormula { get; set; }

    /// <inheritdoc />
    public static string Key { get; } = GetScriptKey(typeof(DefaultLevelUpScript));

    public DefaultLevelUpScript() => LevelUpFormula = LevelUpFormulae.Default;

    /// <inheritdoc />
    public static ILevelUpScript Create() => FunctionalScriptRegistry.Instance.Get<ILevelUpScript>(Key);

    /// <inheritdoc />
    public virtual void LevelUp(Aisling aisling)
    {
        aisling.SendOrangeBarMessage("You level up!");
        aisling.UserStatSheet.IncrementLevel();
        aisling.UserStatSheet.GivePoints(2);

        if (aisling.UserStatSheet.Level < WorldOptions.Instance.MaxLevel)
        {
            var newTnl = LevelUpFormula.CalculateTnl(aisling);
            aisling.UserStatSheet.AddTNL(newTnl);
        }

        var levelUpAttribs = LevelUpFormula.CalculateAttributesIncrease(aisling);
        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 79,
        };
        aisling.UserStatSheet.Add(levelUpAttribs);
        aisling.UserStatSheet.SetMaxWeight(LevelUpFormula.CalculateMaxWeight(aisling));
        aisling.Animate(ani);
        aisling.Client.SendAttributes(StatUpdateType.Full);
    }
}