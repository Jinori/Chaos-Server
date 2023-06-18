using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts
{
    public class CraftingTileScript : ReactorTileScriptBase
    {
        private readonly IDialogFactory DialogFactory;
        private readonly IMerchantFactory MerchantFactory;

        private readonly Dictionary<string, CraftingDetails?> CraftDetails;

        /// <inheritdoc />
        public CraftingTileScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
            : base(subject)
        {
            MerchantFactory = merchantFactory;
            DialogFactory = dialogFactory;

            CraftDetails = new Dictionary<string, CraftingDetails?>
            {
                ["mileth_kitchen"] = new("stove_merchant", "cooking_initial", Hobbies.Cooking, Crafts.None),
                ["tagor_forge"] = new("anvil_merchant", "weaponsmithing_initial", null, Crafts.Weaponsmithing),
                ["piet_alchemy_lab"] = new("table_merchant", "alchemy_initial", null, Crafts.Alchemy),
                ["mileth_inn"] = new("sewing_merchant", "armorsmithing_initial", null, Crafts.Armorsmithing),
                ["undine_enchanted_haven"] = new("crystalball_merchant", "enchanting_initial", null, Crafts.Enchanting),
                ["rucesion_jeweler"] = new("jewelerbench_merchant", "jewelcrafting_initial", null, Crafts.Jewelcrafting),
            };
        }

        /// <inheritdoc />
        public override void OnWalkedOn(Creature source)
        {
            if (source is not Aisling aisling)
                return;

            aisling.Trackers.Enums.TryGetValue(out Crafts stage);

            if (CraftDetails.TryGetValue(aisling.MapInstance.InstanceId, out var details))
            {
                if (details is not null)
                {
                    if (!aisling.IsAdmin)
                    {
                        if ((details.RequiredHobby != null) && !aisling.Trackers.Flags.HasFlag(details.RequiredHobby.Value))
                        {
                            aisling.SendOrangeBarMessage($"You do not know {details.RequiredHobby.Value.ToString().ToLower()}.");
                            return;
                        }
                    
                        if ((details.RequiredCraft != Crafts.None) && (!aisling.Trackers.Enums.TryGetValue(out stage) || (stage != details.RequiredCraft)))
                        {
                            aisling.SendOrangeBarMessage($"You know nothing of {details.RequiredCraft.ToString().ToLower()}.");
                            return;
                        }
                    }

                    var merchant = MerchantFactory.Create(details.Merchant, source.MapInstance, new Point(6, 6));
                    var dialog = DialogFactory.Create(details.DialogKey, merchant);
                    dialog.Display(aisling);   
                }
            }
        }

        private sealed class CraftingDetails
        {
            public CraftingDetails(string merchant, string dialogKey, Hobbies? requiredHobby,
                Crafts requiredCraft
            )
            {
                Merchant = merchant;
                DialogKey = dialogKey;
                RequiredHobby = requiredHobby;
                RequiredCraft = requiredCraft;
            }
            
            public string Merchant { get; }
            public string DialogKey { get; }
            public Hobbies? RequiredHobby { get; }
            public Crafts RequiredCraft { get; }

        }
    }
}
