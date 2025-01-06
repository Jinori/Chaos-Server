using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class LightPotionScript : ItemScriptBase
{
    public LightPotionScript(Item subject)
        : base(subject) { }

    public override bool CanUse(Aisling source)
    {
        if (source.IsDead)
        {
            source.SendOrangeBarMessage("You must be alive to administer this potion.");

            return false;
        }

        return true;
    }

    public override void OnUse(Aisling source)
    {
        var targetPosition = source.DirectionalOffset(source.Direction); // Translate(direction, distance)

        // Check if there is an Aisling (player) at the target position
        var aisling = source.MapInstance
                            .GetEntities<Aisling>()
                            .FirstOrDefault(x => x.IsAlive && x.Effects.Contains("Stoned") && x.WithinRange(targetPosition, 0));

        if (aisling != null)
        {
            var ani = new Animation
            {
                TargetAnimation = 78,
                AnimationSpeed = 200
            };
            aisling.Animate(ani);
            aisling.Effects.Terminate("Stoned");
            aisling.SendOrangeBarMessage($"{source.Name} administered a Light Potion on you.");
            source.SendOrangeBarMessage($"You administered a Light Potion on {aisling.Name}.");
        }

        source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
    }
}