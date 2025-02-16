using System.Reactive.Subjects;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Discord;
using Direction = Chaos.Geometry.Abstractions.Definitions.Direction;

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
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 1,
                    Direction = source.Direction
                };

                var points = AoeShape.Cleave.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }

            case "skillcombat_frontalcone_range3":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 3,
                    Direction = source.Direction
                };

                var points = AoeShape.FrontalCone.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_frontalcone_frontandcascaderange2":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X + 1, source.Y),
                    Range = 2,
                    Direction = source.Direction
                };

                var points = AoeShape.FrontalCone.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_frontaldiamond_frontandcascaderange4":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X + 1, source.Y),
                    Range = 4,
                    Direction = source.Direction
                };

                var points = AoeShape.FrontalDiamond.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_frontaldiamond_range3":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 3,
                    Direction = source.Direction
                };

                var points = AoeShape.FrontalDiamond.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_threeinrowfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 1,
                    Direction = source.Direction
                };

                var points = AoeShape.FrontalCone.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_fourinfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 4,
                    Direction = source.Direction
                };

                var points = AoeShape.Front.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_twoinfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 2,
                    Direction = source.Direction
                };

                var points = AoeShape.Front.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_oneinfront":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 1,
                    Direction = source.Direction
                };

                var points = AoeShape.Front.ResolvePoints(options);
                
                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }

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
                    mob1.Direction = Direction.Right;
                    source.MapInstance.AddEntity(mob1, point);
                }
                
                Subject.Close(source);
                break;
            }
            
            case "skillcombat_allaround2":
            {

                ClearMonsters(source);
                WarpToPosition(source);
                
                var options = new AoeShapeOptions
                {
                    Source = new Point(source.X, source.Y),
                    Range = 2
                };

                var points = AoeShape.AllAround.ResolvePoints(options);

                foreach (var point in points)
                {
                    if (point == new Point(source.X, source.Y))
                        continue;
                    
                    var mob1 = MonsterFactory.Create("trainingdummy0", source.MapInstance, point);
                    mob1.Direction = Direction.Right;
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
        source.Turn(Direction.Right);
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