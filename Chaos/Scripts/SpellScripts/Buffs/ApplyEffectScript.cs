using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Buffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyEffectScript : BasicSpellScriptBase
{
    protected IEffectFactory EffectFactory { get; }

    /// <inheritdoc />
    public ApplyEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        if (EffectKey is "stench")
        {
            if (context.SourceAisling!.Effects.Contains("stench"))
            {
                context.SourceAisling?.Effects.Terminate("stench");
                return;
            }
        }
        
        if (ManaSpent.HasValue)
        {
            //Require mana
            if (context.Source.StatSheet.CurrentMp < ManaSpent.Value)
            {
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You do not have enough mana for this cast.");
                return;
            }
            //Subtract mana and update user
            context.Source.StatSheet.SubtractMp(ManaSpent.Value);
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Vitality);
        }
        
        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);

        foreach (var target in targets.TargetEntities)
        {
            var effect = EffectFactory.Create(EffectKey);
            target.Effects.Apply(context.Source, effect);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }
    
    #region ScriptVars
    protected int? ManaSpent { get; init; }
    protected string EffectKey { get; init; } = null!;
    #endregion
}