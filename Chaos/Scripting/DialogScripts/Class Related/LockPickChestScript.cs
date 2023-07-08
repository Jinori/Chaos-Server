using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class LockPickChestScript : DialogScriptBase
{
    public LockPickChestScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass != BaseClass.Rogue)
        {
            Subject.Reply(source, "The chest chuckles at your touch, 'Only a deft Rogue could pick my insides!'");

            return;
        }

        if (!source.Inventory.HasCount("Lockpicks", 1))
            Subject.Reply(source, "The chest smirks, 'What did you hope to tickle my tumbler with, a feather?'");
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var numberguessed))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (numberguessed is < 1 or >= 6)
        {
            Subject.Reply(source, "The chest cackles, 'You thought that would work? Try harder!'");

            return;
        }

        var numberToWinChest = IntegerRandomizer.RollSingle(5);

        var prizeGold = source.StatSheet.Level switch
        {
            <= 10 => IntegerRandomizer.RollSingle(10000),
            <= 20 => IntegerRandomizer.RollSingle(20000),
            <= 30 => IntegerRandomizer.RollSingle(30000),
            <= 40 => IntegerRandomizer.RollSingle(40000),
            <= 50 => IntegerRandomizer.RollSingle(50000),
            _     => IntegerRandomizer.RollSingle(60000) // default case
        };

        if (numberToWinChest.Equals(numberguessed))
        {
            source.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"The chest groans as its lock gives way. Inside, you find a glittering prize of {prizeGold} gold!");

            source.TryGiveGold(prizeGold);
            source.Inventory.RemoveQuantity("Lockpicks", 1);

            switch (Subject.DialogSource)
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
            var breakPick = IntegerRandomizer.RollChance(50);

            if (breakPick)
            {
                source.Inventory.RemoveQuantity("Lockpicks", 1);

                source.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    "The lock defeats your clumsy efforts and claims your lockpick as a trophy. Better luck next time!");
            }
            else
                source.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    "You fail to pick the lock. The chest giggles, 'That tickled, try again!'");
        }

        Subject.Close(source);
    }
}