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

        if (!ShouldFollow)
            return;
        
        var substringEndIndex = Subject.Name.IndexOf("'s Gloop", StringComparison.Ordinal);
        var substring = Subject.Name.Substring(0, substringEndIndex);
        
        var player = ClientRegistry
            .Select(c => c.Aisling);

        foreach (var aisling in player)
        {
            if (aisling.Name == substring)
            {
                if (Map != aisling.MapInstance)
                    Map.RemoveObject(Subject);
                
                if (aisling.DistanceFrom(Subject) > 4 && Map == aisling.MapInstance) 
                    Subject.Pathfind(new Point(aisling.X, aisling.Y));
            }
        }
    }
}