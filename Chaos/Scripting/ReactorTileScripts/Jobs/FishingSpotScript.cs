using Chaos.Common.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Jobs;

public class FishingSpotScript : ReactorTileScriptBase
{
    private readonly IEffectFactory _effectFactory;

    public FishingSpotScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject) => _effectFactory = effectFactory;

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (!aisling.Inventory.HasCount("Fishing Bait", 1))
            return;
        
        var templateTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;
        if (templateTemplateKey?.EndsWith("FishingPole", StringComparison.Ordinal) is not true) 
            return;
        
        if (source.Effects.Contains("Fishing"))
            return;
        
        var effect = _effectFactory.Create("Fishing");
        aisling.Effects.Apply(aisling, effect);
    }
}