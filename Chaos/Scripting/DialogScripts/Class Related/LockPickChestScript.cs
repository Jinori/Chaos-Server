using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class LockPickChestScript : DialogScriptBase
{
    private const int LOCKPICK_FAILURE_CHANCE = 38;
    private readonly ILogger<LockPickChestScript> Logger;

    public LockPickChestScript(Dialog subject, ILogger<LockPickChestScript> logger) : base(subject) => Logger = logger;

    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass != BaseClass.Rogue)
        {
            Subject.Reply(source, "The chest chuckles, 'Only a deft Rogue could pick my insides!'");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} attempted to interact with a lockpick chest but is not a Rogue", source.Name);

            return;
        }

        if (!source.Inventory.HasCount("Lockpicks", 1))
        {
            Subject.Reply(source, "The chest smirks, 'What did you hope to tickle my tumbler with a feather?'");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} attempted to pick a chest but had no lockpicks", source.Name);
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!Subject.MenuArgs.TryGet<int>(0, out var numberGuessed) || (numberGuessed < 1) || (numberGuessed >= 6))
        {
            Subject.Reply(source, "The chest cackles, 'You thought that would work? Try harder!'");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogWarning("{@AislingName} submitted an invalid lockpick number: {Guess}", source.Name, numberGuessed);

            return;
        }

        AttemptToPickLock(source, numberGuessed);
    }

    private void AttemptToPickLock(Aisling source, int numberGuessed)
    {
        var numberToWinChest = IntegerRandomizer.RollSingle(5);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
              .WithProperty(source)
              .LogInformation("{@AislingName} is attempting to pick a chest. Guessed: {Guess}, Correct: {Correct}", source.Name, numberGuessed, numberToWinChest);

        if (numberToWinChest == numberGuessed)
        {
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} successfully opened a lockpick chest", source.Name);

            AwardPrize(source);
            RemoveChest(source);
        }
        else
        {
            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} failed to open the chest", source.Name);

            HandleFailedAttempt(source);
        }

        Subject.Close(source);
    }

    private void AwardPrize(Aisling source)
    {
        var prizeGold = CalculatePrize(source.StatSheet.Level);
        source.SendServerMessage(ServerMessageType.OrangeBar1, $"You find a prize of {prizeGold} gold!");
        source.TryGiveGold(prizeGold);
        source.Inventory.RemoveQuantity("Lockpicks", 1);

        Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
              .WithProperty(source)
              .LogInformation("{@AislingName} received {GoldAmount} gold from a chest", source.Name, prizeGold);
    }

    private void HandleFailedAttempt(Aisling source)
    {
        if (IntegerRandomizer.RollChance(LOCKPICK_FAILURE_CHANCE))
        {
            source.Inventory.RemoveQuantity("Lockpicks", 1);
            source.SendServerMessage(ServerMessageType.OrangeBar1, "The lock defeats your efforts and claims your lockpick as a trophy!");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} lost a lockpick on failed chest attempt", source.Name);
        }
        else
        {
            source.SendServerMessage(ServerMessageType.OrangeBar1, "You fail to pick the lock. The chest giggles!");

            Logger.WithTopics(Topics.Entities.Aisling, Topics.Actions.LockPick)
                  .WithProperty(source)
                  .LogInformation("{@AislingName} failed to pick the lock but did not lose a lockpick", source.Name);
        }
    }

    private int CalculatePrize(int level)
        => IntegerRandomizer.RollSingle(level / 10 * 3000 + 3000);

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
