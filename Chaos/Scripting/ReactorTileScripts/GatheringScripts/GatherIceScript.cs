using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.GatheringScripts;

public class GatherIceScript : ReactorTileScriptBase
{
    private readonly TimeSpan _cooldownDuration = TimeSpan.FromMinutes(30);
    private readonly IItemFactory _itemFactory;
    private DateTime _lastActivationTime = DateTime.MinValue;

    /// <inheritdoc />
    public GatherIceScript(ReactorTile subject, IItemFactory itemFactory, ISimpleCache simpleCache)
        : base(subject) =>
        _itemFactory = itemFactory;

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (DateTime.Now - _lastActivationTime < _cooldownDuration)
            return;

        var ice = _itemFactory.Create("ice");
        const int ICE_COUNT = 1;
        ice.Count = ICE_COUNT;

        var animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 20
        };

        aisling.Animate(animation);
        aisling.GiveItemOrSendToBank(ice);
        aisling.SendOrangeBarMessage("You shuffle around and notice a solid block of Ice!");
        _lastActivationTime = DateTime.Now;
    }
}