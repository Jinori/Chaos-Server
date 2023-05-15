using System.Net;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.SkillScripts;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.Foods;

public class FoodEffectScript : ConfigurableItemScriptBase
{
    protected string? EffectName { get; init; }
    protected readonly IEffectFactory EffectFactory;
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IApplyHealScript ApplyHealScript;
    private readonly IItemFactory ItemFactory;
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public FoodEffectScript(Item subject, IEffectFactory effectFactory, IItemFactory itemFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ItemFactory = itemFactory;
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ManaCostComponent = new ManaCostComponent();
    }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        if (EffectName != null)
        {
            var effect = EffectFactory.Create(EffectName);

            if ((EffectName != null) && source.IsAlive && source.Effects.TryGetEffect(EffectName, out var foodeffect))
            {
                source.SendOrangeBarMessage("You already have this effect.");
                return;
            }
            source.Effects.Apply(source, effect);
        }
        var hpAmt = HealthAmount ?? 0;

        if (HealthPercent.HasValue)
                    hpAmt += MathEx.GetPercentOf<int>((int)source.StatSheet.EffectiveMaximumHp, HealthPercent.Value);

        switch (hpAmt)
        {
            
            case < 0:
                ApplyDamageScript.ApplyDamage(
                    source,
                    source,
                    this,
                    hpAmt);

                break;
            case > 0:
                ApplyHealScript.ApplyHeal(
                    source,
                    source,
                    this,
                    hpAmt);

                break;
        }

        if (HealthAmount.HasValue && ((source.StatSheet.CurrentHp + HealthAmount.Value) > 0))
            source.UserStatSheet.AddHp(HealthAmount.Value);

        if (HealthPercent.HasValue)
            source.UserStatSheet.AddHealthPct(HealthPercent.Value);

        if (ManaAmount.HasValue)
            source.UserStatSheet.AddMp(ManaAmount.Value);

        if (ManaPercent.HasValue)
            source.UserStatSheet.AddManaPct(ManaPercent.Value);

        source.SendOrangeBarMessage($"You consume a {Subject.DisplayName}.");
        source.Client.SendAttributes(StatUpdateType.Vitality);
        source.Inventory.RemoveQuantity(Subject.Template.TemplateKey, 1);
    }
    
    #region ScriptVars

    private int? HealthAmount { get; init; }
    private int? HealthPercent { get; init; }
    private int? ManaAmount { get; init; }
    private int? ManaPercent { get; init; }
    
    #endregion
}