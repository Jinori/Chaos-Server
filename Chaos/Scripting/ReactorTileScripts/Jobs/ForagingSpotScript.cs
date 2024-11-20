using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Jobs;

public class ForagingSpotScript(ReactorTile subject, IEffectFactory effectFactory) : ReactorTileScriptBase(subject)
{
    public override void OnWalkedOn(Creature source)
    {
        if (source is Aisling aisling)
        {
            HandleAisling(aisling);
        }
    }

    private void HandleAisling(Aisling aisling)
    {
        if (!IsUsingForagingGlove(aisling))
        {
            aisling.SendOrangeBarMessage("If you plan on foraging, a glove is needed.");
            return;
        }

        if (!AlreadyFishing(aisling))
            StartFishing(aisling);
    }

    private bool IsUsingForagingGlove(Aisling aisling)
    {
        var weaponTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;
        return weaponTemplateKey?.EndsWith("glove", StringComparison.Ordinal) == true;
    }

    private bool AlreadyFishing(Aisling aisling) => aisling.Effects.Contains("Foraging");

    private void StartFishing(Aisling aisling)
    {
        var effect = effectFactory.Create("Foraging");
        aisling.Effects.Apply(aisling, effect);
    }
}