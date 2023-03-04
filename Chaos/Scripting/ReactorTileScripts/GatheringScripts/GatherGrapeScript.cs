using Chaos.Data;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts;

public class GatherGrapeScript : ReactorTileScriptBase
{
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public GatherGrapeScript(ReactorTile subject, IItemFactory itemFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {

        if (source is not Aisling aisling)
            return;

        var grape = ItemFactory.Create("Grape");
        var grapeCount = Random.Shared.Next(3,6);

        grape.Count = grapeCount;

        if (aisling.TryGiveItem(grape))
        {
            var animation = new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 20
            };
            
            aisling.Animate(animation);
            aisling.SendOrangeBarMessage("You gathered some grapes!");
        }
    }
}