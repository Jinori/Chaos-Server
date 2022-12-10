using Chaos.Common.Definitions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.ReactorTileScripts
{
    public class FishingSpotScript : ReactorTileScriptBase
    {
        private readonly IEffectFactory EffectFactory;

        public FishingSpotScript(ReactorTile subject, IEffectFactory effectFactory) : base(subject)
        {
            EffectFactory = effectFactory;
        }

        public override void OnWalkedOn(Creature source)
        {
            if (source is not Aisling)
                return;
            var aisling = source as Aisling;


            if (aisling?.Equipment[EquipmentSlot.Weapon]?.Template.TemplateKey.EndsWith("FishingPole") is true)
            {
                var effect = EffectFactory.Create("Fishing");
                aisling.Effects.Apply(aisling, effect);
            }
        }
    }
}
