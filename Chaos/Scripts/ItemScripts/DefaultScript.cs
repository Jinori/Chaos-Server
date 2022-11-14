using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class DefaultScript : ItemScriptBase
{
    public DefaultScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source) => source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You can't use that");
}