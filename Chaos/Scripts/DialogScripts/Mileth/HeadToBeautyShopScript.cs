using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;

namespace Chaos.Scripts.DialogScripts.Mileth;

public class HeadToBeautyShopScript : DialogScriptBase
{
    public HeadToBeautyShopScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplayed(Aisling source)
    {
        if (!source.Flags.HasFlag(QuestFlag1.HeadedToBeautyShop))
        {
            source.Flags.AddFlag(QuestFlag1.HeadedToBeautyShop);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Josephine's shop is located in the southern part of town.");
        }
    }
}