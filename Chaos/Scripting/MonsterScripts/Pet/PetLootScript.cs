using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

// ReSharper disable once ClassCanBeSealed.Global
public class PetLootScript : MonsterScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    /// <inheritdoc />
    public PetLootScript(Monster subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject) =>
        ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!ShouldMove || (Target != null))
            return;

        var now = DateTime.UtcNow;
        const double TIME_SPAN_SECONDS = 3;
        var substringEndIndex = Subject.Name.IndexOf("'s Gloop", StringComparison.Ordinal);
        var substring = Subject.Name.Substring(0, substringEndIndex);

        var player = ClientRegistry.Where(x => x.Aisling.IsAlive && (x.Aisling.Name == substring))
                                   .Select(x => x.Aisling)
                                   .FirstOrDefault();

        if (player is null)
            return;

        var item = Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(player)
                          .FirstOrDefault(x => now - x.Creation > TimeSpan.FromSeconds(TIME_SPAN_SECONDS));

        if (item is null)
            return;

        var itemDistance = Subject.DistanceFrom(item);

        switch (itemDistance)
        {
            case > 1 and < 13:
                Subject.Pathfind(item);

                break;
            case >= 13:
                Subject.WarpTo(item);

                break;
            case <= 1:
            {
                Map.RemoveEntity(item);
                var amount = item.Item.Template.SellValue / 75;
                Subject.Gold += amount;
                player.SendActiveMessage($"Gloop munched {amount} gold. Total gold so far is {Subject.Gold} gold.");

                break;
            }
        }
    }
}