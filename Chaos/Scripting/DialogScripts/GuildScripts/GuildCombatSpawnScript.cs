using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Direction = Chaos.Geometry.Abstractions.Definitions.Direction;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildCombatSpawnScript(Dialog subject, IMonsterFactory monsterFactory) 
    : DialogScriptBase(subject)
{
    private static readonly Point WarpPosition = new(63, 29);
    private const string WALL_MONSTER_KEY = "carnun_wall";


    private static readonly Dictionary<string, (string BossKey, Point SpawnPosition)> BossSpawnData = new()
    {
        { "huntcombat_darkcarnun", ("guildcarnun", new Point(64, 29)) },
        { "huntcombat_firelord", ("firelord_boss", new Point(70, 35)) },
        { "huntcombat_icebeast", ("icebeast_boss", new Point(50, 20)) }
    };

    public override void OnDisplaying(Aisling source)
    {
        var templateKey = Subject.Template.TemplateKey.ToLower();

        if (BossSpawnData.TryGetValue(templateKey, out var bossData))
            HandleBossSpawn(source, bossData.BossKey, bossData.SpawnPosition);

        WarpPlayer(source);
        Subject.Close(source);
    }

    private void HandleBossSpawn(Aisling source, string bossKey, Point spawnPosition)
    {
        ClearAllMonsters(source);

        var combatArea = DetermineCombatArea(bossKey);
        CreateCombatArena(source, combatArea);

        SpawnBoss(source, bossKey, spawnPosition);
    }

    private static Rectangle DetermineCombatArea(string bossKey) =>
        bossKey switch
        {
            "guildcarnun"   => new Rectangle(59, 25, 11, 10),
            "firelord_boss" => new Rectangle(65, 30, 15, 12),
            "icebeast_boss" => new Rectangle(45, 15, 12, 10),
            _               => new Rectangle(60, 25, 10, 10)
        };

    private void CreateCombatArena(Aisling source, Rectangle area)
    {
        var wallPositions = GetWallPositions(area);
        SpawnWalls(source, wallPositions);
    }

    private static HashSet<Point> GetWallPositions(Rectangle rect)
    {
        var wallPositions = new HashSet<Point>();

        for (var x = rect.Left; x <= rect.Right; x++)
        {
            wallPositions.Add(new Point(x, rect.Top)); 
            wallPositions.Add(new Point(x, rect.Bottom));
        }

        for (var y = rect.Top + 1; y < rect.Bottom; y++)
        {
            wallPositions.Add(new Point(rect.Left, y));
            wallPositions.Add(new Point(rect.Right, y));
        }

        return wallPositions;
    }

    private void SpawnWalls(Aisling source, IEnumerable<Point> positions)
    {
        foreach (var position in positions)
        {
            var wall = monsterFactory.Create(WALL_MONSTER_KEY, source.MapInstance, position);
            source.MapInstance.AddEntity(wall, position);
        }
    }

    private void SpawnBoss(Aisling source, string bossKey, Point spawnPosition)
    {
        var boss = monsterFactory.Create(bossKey, source.MapInstance, spawnPosition);
        source.MapInstance.AddEntity(boss, spawnPosition);
    }

    private void WarpPlayer(Aisling source)
    {
        source.WarpTo(WarpPosition);
        source.Turn(Direction.Right);
    }

    private static void ClearAllMonsters(Aisling source)
    {
        var monsters = source.MapInstance.GetEntities<Monster>().ToList();
        foreach (var monster in monsters)
        {
            source.MapInstance.RemoveEntity(monster);
        }
    }
}
