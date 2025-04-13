#region
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Scripting.WorldScripts.WorldBuffs.Religion;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.SpellScripts.Buffs;

public class ApplyEffectScript : ConfigurableSpellScriptBase,
                                 SpellComponent<Creature>.ISpellComponentOptions,
                                 ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    public List<string>? EffectKeysToBreak { get; set; }

    public int? HealthCost { get; init; }
    public decimal PctHealthCost { get; init; }

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
    
    private readonly IStorage<ReligionBuffs> ReligionBuffStorage;

    /// <inheritdoc />
    public ApplyEffectScript(Spell subject, IEffectFactory effectFactory, IStorage<ReligionBuffs> religionBuffStorage)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ReligionBuffStorage = religionBuffStorage;
    } 

    private readonly List<string> CradhEffectKeys = [ "Dia Cradh", "Ard Cradh", "Mor Cradh", "Cradh", "Beag Cradh"];
    private bool HasReligionBuff(string buffName)
    {
        if (ReligionBuffStorage.Value.ActiveBuffs.Any(buff => buff.BuffName.EqualsI(buffName)))
            return true;

        return false;
    }
    
    /// <inheritdoc />
    public override bool CanUse(SpellContext context)
    {
        if (Subject.Template.Name.Equals("quake", StringComparison.CurrentCultureIgnoreCase)
            || Subject.Template.Name.Equals("vortex", StringComparison.CurrentCultureIgnoreCase)
            || Subject.Template.Name.Equals("dark storm", StringComparison.CurrentCultureIgnoreCase))
            if (context.TargetCreature is Monster monster && !monster.Script.Is<PetScript>())
                return false;

        
        if (context.TargetCreature is Aisling && CradhEffectKeys.ContainsI(Subject.Template.Name) && HasReligionBuff(ReligionScriptBase.THESELENE_GLOBAL_BUFF_NAME))
            return false;
        
        return true;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
        => new ComponentExecutor(context).WithOptions(this)
                                         .ExecuteAndCheck<SpellComponent<Creature>>()
                                         ?.Execute<ApplyEffectAbilityComponent>();

    #region ScriptVars
    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    /// <inheritdoc />
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

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
}