using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

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