using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Pet;

public class PetFollowScript : MonsterScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    /// <inheritdoc />
    public PetFollowScript(Monster subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject) =>
        ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!ShouldMove || (Target != null))
            return;

        const double TIME_SPAN_SECONDS = 3;
        var now = DateTime.UtcNow;
        var substringEndIndex = Subject.Name.IndexOf("'s Gloop", StringComparison.Ordinal);
        var substring = Subject.Name[..substringEndIndex];

        var player = ClientRegistry.Where(x => x.Aisling.IsAlive && (x.Aisling.Name == substring))
                                   .Select(x => x.Aisling)
                                   .FirstOrDefault();

        if (player is not null)
        {
            if (Map != player.MapInstance)
            {
                Subject.TraverseMap(player.MapInstance, player);
                return;
            }
            
            var item = Subject.MapInstance.GetEntitiesWithinRange<GroundItem>(player)
                              .FirstOrDefault(x => now - x.Creation > TimeSpan.FromSeconds(TIME_SPAN_SECONDS));
        
            if (item is not null)
                return;

            var playerDistance = player.DistanceFrom(Subject);
            switch (playerDistance)
            {
                case > 4 and < 13:
                    Subject.Pathfind(new Point(player.X, player.Y));
                    break;
                case >= 13:
                    Subject.WarpTo(player);
                    break;
            }
        }
        
        Subject.MoveTimer.Reset();
    }
}