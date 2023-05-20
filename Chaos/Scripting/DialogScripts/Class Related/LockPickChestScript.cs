using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class LockPickChestScript : DialogScriptBase
{
    public LockPickChestScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass != BaseClass.Rogue)
        {
            Subject.Reply(source, "Only a Rogue may pick this lock.");
            return;
        }

        if (source.Inventory.HasCount("Lockpicks", 1)) 
            return;
       
        Subject.Reply(source, "Perhaps you'd need some keys to attempt to open this chest.");
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var numberguessed))
        {
            Subject.Reply(source, DialogString.UnknownInput.Value);

            return;
        }

        if (numberguessed is < 1 or >= 6)
        {
            Subject.Reply(source, "The chest seems to laugh as you input the incorrect code.");

            return;
        }

        var numberToWinChest = Randomizer.RollSingle(5);
        var prizeGold = Randomizer.RollSingle(10000);

        if (numberToWinChest.Equals(numberguessed))
        {
            source.SendServerMessage(ServerMessageType.OrangeBar1, $"You've unlocked the chest and received {prizeGold} gold!");
            source.TryGiveGold(prizeGold);
            source.Inventory.RemoveQuantity("Lockpicks", 1);

            switch (Subject.SourceEntity)
            {
                case MapEntity mapEntity:
                {
                    mapEntity.MapInstance.RemoveObject(mapEntity);
                    break;
                }
                case Item itemEntity:
                {
                    source.Inventory.RemoveQuantity(itemEntity.Slot, 1);
                    break;
                }
            }
        }
        else
        {
            var breakPick = Randomizer.RollChance(50);

            if (breakPick)
            {
                source.Inventory.RemoveQuantity("Lockpicks", 1);
                source.SendServerMessage(ServerMessageType.OrangeBar1, "You failed to pick the lock and your lockpicks broke!");
            } else
                source.SendServerMessage(ServerMessageType.OrangeBar1, "You failed to pick the lock!");
        }

        Subject.Close(source);
    }
}