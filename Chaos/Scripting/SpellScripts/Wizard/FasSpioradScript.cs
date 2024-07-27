using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts.Wizard;

public class FasSpioradScript : ConfigurableSpellScriptBase,
                                 SpellComponent<Creature>.ISpellComponentOptions
{
    /// <inheritdoc />
    public FasSpioradScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var effect1 = EffectFactory.Create("PreventHeal");
        
            var healthSacrificed = context.Source.StatSheet.CurrentHp * .60;
            var manaToReplenish = healthSacrificed * 1.10;

            if (context.Source.StatSheet.CurrentHp >= 1)
            {
                context.Source.StatSheet.SubtractHp((int)healthSacrificed);
                context.Source.StatSheet.AddMp((int)manaToReplenish);
                context.Source.Effects.Apply(context.Source, effect1);
                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
                context.SourceAisling?.SendOrangeBarMessage($"You replenished {(int)manaToReplenish} mana using Fas Spiorad!");
            }
            else
            {
                context.SourceAisling?.SendOrangeBarMessage("You do not have the health to convert to mana.");
            }
    }

    #region ScriptVars
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public bool SingleTarget { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public string? EffectKey { get; init; }

    public int? EffectApplyChance { get; init; }

    /// <inheritdoc />
    public TimeSpan? EffectDurationOverride { get; init; }
    /// <inheritdoc />
    public IEffectFactory EffectFactory { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public bool IgnoreMagicResistance { get; init; }
    #endregion

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
}