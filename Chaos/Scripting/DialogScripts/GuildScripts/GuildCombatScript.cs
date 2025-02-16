using System.Reactive.Subjects;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildCombatScript : DialogScriptBase
{
    private IMonsterFactory MonsterFactory;
    
    public GuildCombatScript(Dialog subject, IMonsterFactory monsterFactory) : base(subject)
    {
        MonsterFactory = monsterFactory;
    }

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "skillcombat_cleave":
            {
                ClearMonsters(source);
                WarpToPosition(source);
                var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(64, 29));
                var mob2 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(63, 28));
                var mob3 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(63, 30));
                source.MapInstance.AddEntity(mob1, new Point(64, 29));
                source.MapInstance.AddEntity(mob2, new Point(63, 28));
                source.MapInstance.AddEntity(mob3, new Point(63, 30));
                Subject.Close(source);
                break;
            }

            case "skillcombat_threeinrowfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(64, 29));
                var mob2 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(64, 28));
                var mob3 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(64, 30));
                source.MapInstance.AddEntity(mob1, new Point(64, 29));
                source.MapInstance.AddEntity(mob2, new Point(64, 28));
                source.MapInstance.AddEntity(mob3, new Point(64, 30));
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_twoinfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(64, 29));
                var mob2 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(65, 29));
                source.MapInstance.AddEntity(mob1, new Point(64, 29));
                source.MapInstance.AddEntity(mob2, new Point(65, 29));
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_oneinfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, new Point(64, 29));
                source.MapInstance.AddEntity(mob1, new Point(64, 29));
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_allaround":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 1
                };

                var points = AoeShape.AllAround.ResolvePoints(options);

                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
        }
    }

    private void WarpToPosition(Aisling source)
    {
        source.WarpTo(new Point(63, 29));
    }
    
    private void ClearMonsters(Aisling source)
    {
        var dummies = source.MapInstance.GetEntities<Monster>().ToList();

        foreach (var monster in dummies)
        {
            source.MapInstance.RemoveEntity(monster);     
        }
    }
}