using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Debuffs;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class AoSithScript : BasicSpellScriptBase
{
    /// <inheritdoc />
    public AoSithScript(Spell subject)
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
            foreach (var effect in target.Effects)
                target.Effects.Dispel(effect.Name);
        }

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}.");
    }
    
    #region ScriptVars
    protected int? ManaSpent { get; init; }
    #endregion
}