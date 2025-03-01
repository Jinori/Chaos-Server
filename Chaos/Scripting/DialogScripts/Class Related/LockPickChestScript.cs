using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class LockPickChestScript(Dialog subject) : DialogScriptBase(subject)
{
    private const int LOCKPICK_FAILURE_CHANCE = 38;

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass != BaseClass.Rogue)
        {
            Subject.Reply(source, "The chest chuckles, 'Only a deft Rogue could pick my insides!'");
            return;
        }

        if (!source.Inventory.HasCount("Lockpicks", 1))
        {
            Subject.Reply(source, "The chest smirks, 'What did you hope to tickle my tumbler with a feather?'");
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var numberGuessed) || numberGuessed < 1 || numberGuessed >= 6)
        {
            Subject.Reply(source, "The chest cackles, 'You thought that would work? Try harder!'");
            return;
        }

        AttemptToPickLock(source, numberGuessed);
    }

    private void AttemptToPickLock(Aisling source, int numberGuessed)
    {
        var numberToWinChest = IntegerRandomizer.RollSingle(5);
        if (numberToWinChest.Equals(numberGuessed))
        {
            AwardPrize(source);
            RemoveChest(source);
        }
        else
            HandleFailedAttempt(source);

        Subject.Close(source);
    }

    private void AwardPrize(Aisling source)
    {
        var prizeGold = CalculatePrize(source.StatSheet.Level);
        source.SendServerMessage(ServerMessageType.OrangeBar1, $"You find a prize of {prizeGold} gold!");
        source.TryGiveGold(prizeGold);
        source.Inventory.RemoveQuantity("Lockpicks", 1);
    }

    private int CalculatePrize(int level) => IntegerRandomizer.RollSingle(level / 10 * 3000 + 3000);

    private void HandleFailedAttempt(Aisling source)
    {
        if (IntegerRandomizer.RollChance(LOCKPICK_FAILURE_CHANCE))
        {
            source.Inventory.RemoveQuantity("Lockpicks", 1);
            source.SendServerMessage(ServerMessageType.OrangeBar1, "The lock defeats your efforts and claims your lockpick as a trophy!");
        }
        else
            source.SendServerMessage(ServerMessageType.OrangeBar1, "You fail to pick the lock. The chest giggles!");
    }

    private void RemoveChest(Aisling source)
    {
        switch (Subject.DialogSource)
        {
            case MapEntity mapEntity:
                mapEntity.MapInstance.RemoveEntity(mapEntity);
                break;
            case Item itemEntity:
                source.Inventory.RemoveQuantity(itemEntity.Slot, 1);
                break;
        }
    }
}
