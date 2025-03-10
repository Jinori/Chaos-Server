using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.DeepCrypt;

public class DCHardLockedChestScript(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private readonly IItemFactory ItemFactory = itemFactory;
    
    private static readonly List<KeyValuePair<string, decimal>> ChestPrizes =
    [
        new("shinguards", 10),
        new("mythrildice", 6),
        new("blooddiamond", 3),
        new("radiantpearl", 15),
        new("eclipsepearl", 15),
        new("largejewelcraftingbox", 15),
        new("largeenchantingbox", 15),
        new("artisanweaponsmithingbox", 15),
        new("artisanarmorsmithingbox", 15),
        new("artisanalchemybox", 15)
    ];
    
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
    
    private void OpenChest(Aisling source)
    {
        source.Inventory.RemoveQuantityByTemplateKey("hardcrypttreasurekey", 1);
        source.SendServerMessage(ServerMessageType.OrangeBar1, "You opened the chest!");
        AwardPrize(source);
        RemoveChest();
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
            case "dchardlockedchest_initial":
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
                        DialogKey = "dchardpicklock_initial",
                        OptionText = "Attempt to pick the lock"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    if (source.Inventory.HasCount("Crypt Treasure Key (Hard)", 1))
                    {
                        var option1 = new DialogOption
                        {
                            DialogKey = "dchardopenchest_initial",
                            OptionText = "Open with Key"
                        };

                        if (!Subject.HasOption(option1.OptionText))
                            Subject.Options.Insert(0, option1);
                    }
                } else
                {
                    if (source.Inventory.HasCount("Crypt Treasure Key (Hard)", 1))
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "dchardopenchest_initial",
                            OptionText = "Open with Key"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    } else
                        Subject.Reply(source, "You have no way of opening me!");
                }
            }

                break;

            case "dchardopenchest_initial":
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
            case "dchardpicklock_initial":
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