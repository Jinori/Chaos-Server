using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;

namespace Chaos.Scripts.ItemScripts;

public class MessagePopupScript : ConfigurableItemScriptBase
{
    protected string Message { get; init; } = null!;
    protected bool ShowToNearby { get; init; }

    public MessagePopupScript(Item subject)
        : base(subject) { }

    public override void OnUse(Aisling source)
    {
        if (!ShowToNearby)
            source.Client.SendServerMessage(ServerMessageType.WoodenBoard, Message);
        else
            foreach (var aisling in source.MapInstance.GetEntitiesWithinRange<Aisling>(source))
                aisling.Client.SendServerMessage(ServerMessageType.WoodenBoard, Message);
    }
}