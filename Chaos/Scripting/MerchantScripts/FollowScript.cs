using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class FollowScript : ConfigurableMerchantScriptBase
{
    private readonly IIntervalTimer FollowTimer;
    protected int FollowIntervalMs { get; init; }
    private MapInstance Map => Subject.MapInstance;
    private bool ShouldFollow => FollowTimer.IntervalElapsed;
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private int TotalGold { get; set; }


    /// <inheritdoc />
    public FollowScript(Merchant subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject)
    {
        FollowTimer = new IntervalTimer(TimeSpan.FromMilliseconds(FollowIntervalMs));
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        FollowTimer.Update(delta);
        var now = DateTime.UtcNow;
        const double TIME_SPAN_SECONDS = 3;
        
        if (!ShouldFollow)
            return;
        
        var substringEndIndex = Subject.Name.IndexOf("'s Gloop", StringComparison.Ordinal);
        var substring = Subject.Name.Substring(0, substringEndIndex);

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
            {
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
                        Map.RemoveObject(item);
                        var amount = item.Item.Template.SellValue / 75;
                        TotalGold += amount;
                        player.TryGiveGold(amount);
                        player.SendActiveMessage($"Glupe munched {amount} gold. That's {TotalGold} total!");
                        break;
                    }
                }
            }
            if (item is null)
            {
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
        }
    }
}