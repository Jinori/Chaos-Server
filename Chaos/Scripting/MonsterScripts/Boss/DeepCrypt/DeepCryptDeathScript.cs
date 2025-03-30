using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.DeepCrypt;

// ReSharper disable once ClassCanBeSealed.Global
public class DeepCryptDeathScript : MonsterScriptBase
{
    private readonly IReactorTileFactory ReactorTileFactory;
    protected IExperienceDistributionScript ExperienceDistributionScript { get; set; }

    /// <inheritdoc />
    public DeepCryptDeathScript(Monster subject, IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        ReactorTileFactory = reactorTileFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var aislings = Subject.MapInstance
                              .GetEntities<Aisling>()
                              .ToList();

        foreach (var aisling in aislings)
        {
            aisling.SendOrangeBarMessage("You've cleared Deep Crypt! An escape portal opens.");
            aisling.Trackers.Counters.AddOrIncrement("deepcryptcompletions");
        }

        var player = aislings.FirstOrDefault();

        if (player == null)
            return;

        if (Subject.MapInstance.Template.TemplateKey != "20117")
            return;

        var portalSpawn = new Rectangle(player, 6, 6);

        var outline = portalSpawn.GetOutline()
                                 .ToList();
        Point point;

        do
            point = outline.PickRandom();
        while (!Subject.MapInstance.IsWalkable(point, collisionType: player.Type));

        var reactortile = ReactorTileFactory.Create("deepcryptescapeportal", Subject.MapInstance, Point.From(point));

        Subject.MapInstance.SimpleAdd(reactortile);
    }
}