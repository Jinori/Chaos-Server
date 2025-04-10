﻿using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox.Jewelcrafting;

public class InitiateJewelcraftingBoxScript : ItemScriptBase
{
    private static readonly Random Random = new();

    private readonly IItemFactory _itemFactory;

    public InitiateJewelcraftingBoxScript(Item subject, IItemFactory itemFactory)
        : base(subject)
        => _itemFactory = itemFactory;

    public override void OnUse(Aisling source)
    {
        source.Inventory.RemoveQuantity(Subject.Slot, 1);

        // Generate random quantities of raw gems
        var rubyAmount = Random.Next(2, 7); // 3-7
        var sapphireAmount = Random.Next(2, 7); // 3-7
        var emeraldAmount = Random.Next(2, 7); // 3-7
        var berylAmount = Random.Next(2, 7); // 3-7
        var heartstoneAmount = Random.Next(2, 7); // 3-7

        // Create items
        var ruby = _itemFactory.Create("rawruby");
        var sapphire = _itemFactory.Create("rawsapphire");
        var emerald = _itemFactory.Create("rawemerald");
        var beryl = _itemFactory.Create("rawberyl");
        var heartstone = _itemFactory.Create("rawheartstone");

        for (var i = 0; i < rubyAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawruby"));

        for (var i = 0; i < sapphireAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawsapphire"));

        for (var i = 0; i < emeraldAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawemerald"));

        for (var i = 0; i < berylAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawberyl"));

        for (var i = 0; i < heartstoneAmount; i++)
            source.GiveItemOrSendToBank(_itemFactory.Create("rawheartstone"));

        // Add items to the player's inventory
        source.GiveItemOrSendToBank(ruby);
        source.GiveItemOrSendToBank(sapphire);
        source.GiveItemOrSendToBank(emerald);
        source.GiveItemOrSendToBank(beryl);
        source.GiveItemOrSendToBank(heartstone);

        var totalgems = rubyAmount + heartstoneAmount + sapphireAmount + emeraldAmount + berylAmount + 5;

        // Notify the player
        source.SendOrangeBarMessage($"You received {totalgems} raw gems!");
    }
}