using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Jobs;

public class FishingSpotScript(ReactorTile subject, IEffectFactory effectFactory) : ReactorTileScriptBase(subject)
{
    private bool AlreadyFishing(Aisling aisling) => aisling.Effects.Contains("Fishing");

    private void HandleAisling(Aisling aisling)
    {
        if (!IsUsingFishingPole(aisling))
        {
            aisling.SendOrangeBarMessage("If you plan on fishing, a rod is needed.");

            return;
        }

        if (!HasBait(aisling))
        {
            aisling.SendOrangeBarMessage("You mumble and regret leaving your bait behind.");

            return;
        }

        if (!AlreadyFishing(aisling))
            StartFishing(aisling);
    }

    private bool HasBait(Aisling aisling) => aisling.Inventory.HasCount("Fishing Bait", 1);

    private bool IsUsingFishingPole(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;

        return weaponTemplateKey?.EndsWith("FishingPole", StringComparison.Ordinal) == true;
    }

    public override void OnWalkedOn(Creature source)
    {
        if (source is Aisling aisling)
            HandleAisling(aisling);
    }

    private void StartFishing(Aisling aisling)
    {
        var effect = effectFactory.Create("Fishing");
        aisling.Effects.Apply(aisling, effect);
    }
}