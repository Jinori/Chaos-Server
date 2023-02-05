using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel;
using Chaos.Scripts.SkillScripts.Abstractions;

namespace Chaos.Scripts.SkillScripts;

public class RemoveStatusScript : BasicSkillScriptBase
{
    private string? StatusToRemove { get; init; }

    public RemoveStatusScript(Skill subject)
        : base(subject) { }

    public override void OnUse(ActivationContext context)
    {
        if (StatusToRemove is not null)
            if (StatusToRemove.EqualsI("Beag Suain"))
            {
                if (context.Source.Status.HasFlag(Status.BeagSuain))
                    context.Source.Status &= ~Status.BeagSuain;

                context.SourceAisling?.Client.SendAttributes(StatUpdateType.Full);
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can walk again.");
            }
    }
}