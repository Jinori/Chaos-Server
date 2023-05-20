using Chaos.Common.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Mileth;

public class HeadToBeautyShopScript : DialogScriptBase
{
    public HeadToBeautyShopScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplayed(Aisling source)
    {
        if (!source.Trackers.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop))
        {
            source.Trackers.Flags.AddFlag(QuestFlag1.HeadedToBeautyShop);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Josephine's shop is located in the southern part of town.");
        }
    }
}