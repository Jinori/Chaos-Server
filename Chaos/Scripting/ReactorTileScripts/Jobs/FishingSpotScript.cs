using Chaos.Common.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Jobs;

public class FishingSpotScript : ReactorTileScriptBase
{
    private readonly IEffectFactory EffectFactory;

    public FishingSpotScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject) => EffectFactory = effectFactory;

    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var templateTemplateKey = aisling.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey;

        if (templateTemplateKey?.EndsWith("FishingPole", StringComparison.Ordinal) is true)
        {
            var effect = EffectFactory.Create("Fishing");
            aisling.Effects.Apply(aisling, effect);
        }
    }
}