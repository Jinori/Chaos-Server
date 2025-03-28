﻿using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts.CraftingMaterialBox
{
    public class SmallEnchantingBoxScript : ItemScriptBase
    {
        private static readonly Random Random = new();
        private readonly IItemFactory _itemFactory;

        public SmallEnchantingBoxScript(Item subject, IItemFactory itemFactory)
            : base(subject)
            => _itemFactory = itemFactory;

        public override void OnUse(Aisling source)
        {
            source.Inventory.RemoveQuantity(Subject.Slot, 1);

            // Generate a random number of essences to give between 3 and 8
            var totalEssences = Random.Next(2, 8); // Random number between 3 and 8

            // List of essence types to distribute
            var essences = new[] {
                "essenceofaquaedon", "essenceofgeolith", "essenceofignatar", 
                "essenceofmiraelis", "essenceofserendael", "essenceofskandara", 
                "essenceoftheselene", "essenceofzephyra"
            };

            // Distribute totalEssences randomly across the available essence types
            for (int i = 0; i < totalEssences; i++)
            {
                // Pick a random essence type
                var randomEssence = essences[Random.Next(essences.Length)];

                // Create and give the item
                source.GiveItemOrSendToBank(_itemFactory.Create(randomEssence));
            }

            // Notify the player
            source.SendOrangeBarMessage($"You received {totalEssences} random essences of the gods!");
        }
    }
}