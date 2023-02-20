using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class RemoveStatusScript : BasicSkillScriptBase
{
    private string? StatusToRemove { get; init; }

    public RemoveStatusScript(Skill subject)
        : base(subject) { }

    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        
        if (StatusToRemove is null) 
            return;

        if (StatusToRemove.EqualsI("BeagSuain"))
        {
            var ani = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 3
            };
            context.Source.Animate(ani, context.Source.Id);
            if (!context.Source.Status.HasFlag(Status.BeagSuain)) 
                return;
            context.Source.Effects.Dispel("BeagSuain");
            context.Source.Status &= ~Status.BeagSuain;
            context.SourceAisling?.Client.SendAttributes(StatUpdateType.Full);
            context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can walk again.");
        }
    }
}