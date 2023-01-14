using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Buffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyGroupEffectScript : BasicSpellScriptBase
{
    protected readonly IEffectFactory EffectFactory;
    protected string EffectKey { get; init; } = null!;

    /// <inheritdoc />
    public ApplyGroupEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

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
        
        var group = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint));
        
        if (group != null)
        {
            foreach (var member in group)
            {
                var effect = EffectFactory.Create(EffectKey);
                member.Effects.Apply(member, effect);
            }   
        }
        else
        {
            var effect = EffectFactory.Create(EffectKey);
            context.Source.Effects.Apply(context.Source, effect);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }
    
    #region ScriptVars
    protected int? ManaSpent { get; init; }
    #endregion
}