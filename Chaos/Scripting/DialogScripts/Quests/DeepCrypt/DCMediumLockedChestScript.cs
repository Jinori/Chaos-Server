using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.DeepCrypt;

public class DCMediumLockedChestScript(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private static readonly List<KeyValuePair<string, decimal>> ChestPrizes =
    [
        // Group 1: 7 “other” items => weight = 4
        new("blackstonering", 4),
        new("ringoffire", 4),
        new("largeemeraldring", 4),
        new("scurvygauntlet", 4),
        new("scurvygreaves", 4),
        new("reinforcedspikedgreaves", 4),
        new("amorphousgauntlet", 4),

        // Group 2: 2 pearls => weight = 11
        new("radiantpearl", 11),
        new("eclipsepearl", 11),

        // Group 3: 5 boxes => weight = 10
        new("advancedjewelcraftingbox", 10),
        new("advancedenchantingbox", 10),
        new("advancedweaponsmithingbox", 10),
        new("advancedarmorsmithingbox", 10),
        new("advancedalchemybox", 10)
    ];

    private readonly IItemFactory ItemFactory = itemFactory;

    private void AttemptToPickLock(Aisling source, int numberGuessed)
    {
        var numberToWinChest = IntegerRandomizer.RollSingle(4);

        if (numberToWinChest.Equals(numberGuessed))
        {
            source.SendServerMessage(ServerMessageType.OrangeBar1, "You opened the chest!");
            AwardPrize(source);
            RemoveChest();
        } else
            HandleFailedAttempt(source);

        Subject.Close(source);
    }

    private void AwardPrize(Aisling source)
    {
        var prizeItem = ChestPrizes.PickRandomWeighted();
        var prize = ItemFactory.Create(prizeItem);
        source.GiveItemOrSendToBank(prize);
        source.SendOrangeBarMessage($"You received {prize.DisplayName} from the Chest!");
    }

    private void HandleFailedAttempt(Aisling source)
    {
        RemoveChest();
        source.Inventory.RemoveQuantityByTemplateKey("lockpicks", 1);
        source.SendServerMessage(ServerMessageType.OrangeBar1, "The lock defeats your efforts and disappears!");
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "dcmediumlockedchest_initial":
            {
                if (Subject.DialogSource is MapEntity chestEntity)
                    if (!source.WithinRange(chestEntity, 1))
                    {
                        source.SendOrangeBarMessage("You are too far away from the chest!");
                        Subject.Close(source);

                        return;
                    }

                if ((source.UserStatSheet.BaseClass == BaseClass.Rogue) && source.Inventory.HasCountByTemplateKey("lockpicks", 1))
                {
                    var option = new DialogOption
                    {
                        DialogKey = "dcmediumpicklock_initial",
                        OptionText = "Attempt to pick the lock"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    if (source.Inventory.HasCount("Crypt Treasure Key (Medium)", 1))
                    {
                        var option1 = new DialogOption
                        {
                            DialogKey = "dcmediumopenchest_initial",
                            OptionText = "Open with Key"
                        };

                        if (!Subject.HasOption(option1.OptionText))
                            Subject.Options.Insert(0, option1);
                    }
                } else
                {
                    if (source.Inventory.HasCount("Crypt Treasure Key (Medium)", 1))
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "dcmediumopenchest_initial",
                            OptionText = "Open with Key"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    } else
                        Subject.Reply(source, "You have no way of opening me!");
                }
            }

                break;

            case "dcmediumopenchest_initial":
            {
                OpenChest(source);

                break;
            }
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "dcmediumpicklock_initial":
            {
                if (!Subject.MenuArgs.TryGet<int>(0, out var numberGuessed) || (numberGuessed < 1) || (numberGuessed >= 5))
                {
                    Subject.Reply(source, "The chest cackles, 'You thought that would work? Try harder!'");

                    return;
                }

                AttemptToPickLock(source, numberGuessed);

                break;
            }
        }
    }

    private void OpenChest(Aisling source)
    {
        source.Inventory.RemoveQuantityByTemplateKey("mediumcrypttreasurekey", 1);
        source.SendServerMessage(ServerMessageType.OrangeBar1, "You opened the chest!");
        AwardPrize(source);
        RemoveChest();
        Subject.Close(source);
    }

    private void RemoveChest()
    {
        switch (Subject.DialogSource)
        {
            case MapEntity mapEntity:
                mapEntity.MapInstance.RemoveEntity(mapEntity);

                break;
        }
    }
}