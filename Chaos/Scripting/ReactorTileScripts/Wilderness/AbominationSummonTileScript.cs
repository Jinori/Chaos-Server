using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Wilderness;

public class AbominationSummonTileScript : ReactorTileScriptBase
{
    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public AbominationSummonTileScript(ReactorTile subject, IMonsterFactory monsterFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out IceWallQuest stage);

        if (hasStage && stage is not IceWallQuest.KillBoss)
            return;

        if (Subject.MapInstance.GetEntities<Monster>().Any(x => x.Template.TemplateKey.EqualsI("wilderness_frozen_cave")))
        {
            aisling.SendOrangeBarMessage("The abomination is already lurking about.");
            return;
        }
        
        if (aisling.Trackers.TimedEvents.HasActiveEvent("abominationcd", out _))
        {
            aisling.SendOrangeBarMessage("You look around and don't see anything.");
            return;
        }

        if (aisling.Inventory.HasCountByTemplateKey("charm", 1))
        {
            var point = new Point(7, 4);
            
            var abomination = MonsterFactory.Create("wilderness_abomination", Subject.MapInstance, point);
            
            aisling.SendOrangeBarMessage("The snow around you shakes and you hear a loud roar...");
            Subject.MapInstance.AddEntity(abomination, point);
            aisling.Trackers.TimedEvents.AddEvent("abominationcd", TimeSpan.FromHours(8), true);
        }
    }
}