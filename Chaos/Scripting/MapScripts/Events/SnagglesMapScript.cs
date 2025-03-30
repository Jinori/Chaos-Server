using Chaos.Collections;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

public class SnagglesMapScript(MapInstance subject, IMonsterFactory monsterFactory) : MapScriptBase(subject)
{
    public override void OnEntered(Creature creature)
    {
        if (creature is not Aisling aisling)
            return;

        if (Subject.GetEntities<Monster>()
                   .Any(x => x.Template.TemplateKey == "snagglesthesweetsnatcher"))
            return;

        var monster = monsterFactory.Create("snagglesthesweetsnatcher", Subject, new Point(11, 11));
        Subject.AddEntity(monster, new Point(11, 11));
    }
}