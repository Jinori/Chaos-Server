using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;

namespace Chaos.Scripts.SpellScripts.Debuffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class RemoveEffectScript : BasicSpellScriptBase
{
    /// <inheritdoc />
    public RemoveEffectScript(Spell subject)
        : base(subject)
    {
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
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
            if (EffectKey.EqualsI("dinarcoli"))
            {
                if (target.Effects.Contains("pramh"))
                    target.Effects.Dispel("pramh");
                if (target.Effects.Contains("beagpramh"))
                    target.Effects.Dispel("beagpramh");
            } 
            else
            {
                if (target.Effects.Contains(EffectKey))
                    target.Effects.Dispel(EffectKey);
            }
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}.");
    }
    
    #region ScriptVars
    protected int? ManaSpent { get; init; }
    protected string EffectKey { get; init; } = null!;
    #endregion
}