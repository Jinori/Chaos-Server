using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.DeepCrypt;

public class DCEasyLockedChestScript(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private static readonly List<KeyValuePair<string, decimal>> ChestPrizes =
    [
        new("leafnecklace", 3),
        new("salveearrings", 8),
        new("powerearrings", 8),
        new("hybrasylbattleaxe", 2),
        new("nunchaku", 2),
        new("oakstaff", 2),
        new("nagetierdagger", 2),
        new("staffofwisdom", 2),
        new("largejewelcraftingbox", 15),
        new("largeenchantingbox", 15),
        new("artisanweaponsmithingbox", 15),
        new("artisanarmorsmithingbox", 15),
        new("artisanalchemybox", 15)
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
            case "dceasylockedchest_initial":
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
                        DialogKey = "dceasypicklock_initial",
                        OptionText = "Attempt to pick the lock"
                    };

                    if (!Subject.HasOption(option.OptionText))
                        Subject.Options.Insert(0, option);

                    if (source.Inventory.HasCount("Crypt Treasure Key (Easy)", 1))
                    {
                        var option1 = new DialogOption
                        {
                            DialogKey = "dceasyopenchest_initial",
                            OptionText = "Open with Key"
                        };

                        if (!Subject.HasOption(option1.OptionText))
                            Subject.Options.Insert(0, option1);
                    }
                } else
                {
                    if (source.Inventory.HasCount("Crypt Treasure Key (Easy)", 1))
                    {
                        var option = new DialogOption
                        {
                            DialogKey = "dceasyopenchest_initial",
                            OptionText = "Open with Key"
                        };

                        if (!Subject.HasOption(option.OptionText))
                            Subject.Options.Insert(0, option);
                    } else
                        Subject.Reply(source, "You have no way of opening me!");
                }
            }

                break;

            case "dceasyopenchest_initial":
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
            case "dceasypicklock_initial":
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
        source.Inventory.RemoveQuantityByTemplateKey("easycrypttreasurekey", 1);
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