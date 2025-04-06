using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Arena.Arena_Modules;

public class TypingArenaMapScript(MapInstance subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache) : MapScriptBase(subject)
{
    private static readonly List<Point> PlayerTiles =
    [
        new(6, 12),
        new(6, 6),
        new(14, 6),
        new(14, 12)
    ];

    private static readonly List<int> AllowedSprites =
    [
        300,
        301,
        302,
        303,
        304,
        328
    ];

    private static readonly HashSet<int> ExcludedSprites =
    [
        0,
        12,
        18,
        45,
        83,
        139,
        140,
        151,
        174,
        191,
        190,
        192,
        193,
        194,
        196,
        202,
        203,
        206,
        207,
        210,
        212,
        213,
        214,
        215,
        216,
        217,
        218,
        219, // 212 through 219
        230,
        231,
        232,
        233,
        234,
        235,
        236,
        237,
        238, // 230 through 238
        245,
        254,
        267,
        288,
        289,
        290,
        291,
        292,
        293,
        294,
        295,
        296,
        297,
        298,
        299, // 288 through 299
        306,
        315,
        316,
        317,
        318,
        325,
        326,
        327,
        340,
        374,
        375,
        376,
        377,
        378,
        379,
        380, // 374 through 380
        402,
        403,
        404,
        411,
        417,
        437,
        493,
        523,
        531,
        532,
        533,
        542,
        543,
        544
    ];

    private readonly HashSet<Point> DangerTiles =
    [
        new(10, 1),
        new(9, 1)
    ];

    private readonly Random Random = new();

    private readonly List<Point> SpawnPoints = new Rectangle(
            6,
            19,
            9,
            1).GetPoints()
              .ToList();

    private readonly IIntervalTimer SpawnTimer = new IntervalTimer(TimeSpan.FromSeconds(8));

    // Word pool for Typing Monsters (500+ words, 5-10 characters each)
    private readonly List<string> WordPool =
    [
        "attack",
        "battle",
        "defend",
        "shield",
        "strike",
        "parry",
        "dodge",
        "charge",
        "damage",
        "counter",
        "block",
        "slash",
        "pierce",
        "combat",
        "onslaught",
        "skirmish",
        "warrior",
        "blitz",
        "fortify",
        "engage",

        // Fantasy & RPG Terms
        "sorcery",
        "arcane",
        "mystic",
        "conjure",
        "summon",
        "phantom",
        "wraith",
        "ethereal",
        "incant",
        "sigil",
        "eldritch",
        "runes",
        "ritual",
        "mana",
        "spellcast",
        "invoke",
        "alchemy",
        "transmute",
        "hex",
        "enchant",

        // Weapons & Equipment
        "dagger",
        "sword",
        "bow",
        "quiver",
        "hammer",
        "gauntlet",
        "crossbow",
        "waraxe",
        "katana",
        "longsword",
        "halberd",
        "lance",
        "flail",
        "buckler",
        "greaves",
        "plate",
        "cuirass",
        "helm",
        "scabbard",
        "bracer",

        // Elemental & Magic-Related Words
        "fireball",
        "blizzard",
        "tornado",
        "earthquake",
        "inferno",
        "lightning",
        "thunder",
        "storm",
        "frostbite",
        "eruption",
        "maelstrom",
        "whirlwind",
        "avalanche",
        "cyclone",
        "tsunami",
        "pyroclasm",
        "gale",
        "ember",
        "glacier",
        "vortex",

        // Mythical Creatures
        "dragon",
        "griffin",
        "phoenix",
        "basilisk",
        "chimera",
        "golem",
        "minotaur",
        "hydra",
        "gargoyle",
        "harpy",
        "sphinx",
        "wyvern",
        "behemoth",
        "leviathan",
        "kraken",
        "specter",
        "lich",
        "shade",
        "banshee",
        "daemon",

        // Exploration & Adventure
        "journey",
        "quest",
        "expedition",
        "explore",
        "venture",
        "pilgrimage",
        "seek",
        "discover",
        "traverse",
        "wander",
        "wayfarer",
        "traveler",
        "odyssey",
        "saga",
        "legend",
        "folklore",
        "parchment",
        "artifact",
        "map",
        "compass",

        // Status Effects & Conditions
        "poison",
        "curse",
        "hexed",
        "stunned",
        "weaken",
        "cripple",
        "blind",
        "silence",
        "paralyze",
        "petrify",
        "affliction",
        "decay",
        "disease",
        "drained",
        "haunted",
        "tainted",
        "wither",
        "frozen",
        "burning",
        "plague",

        // Strategy & Tactics
        "ambush",
        "tactic",
        "strategy",
        "flank",
        "maneuver",
        "retreat",
        "assault",
        "barrage",
        "encircle",
        "siege",
        "fortress",
        "stronghold",
        "defense",
        "breach",
        "counter",
        "position",
        "formation",
        "resist",
        "endurance",
        "survival",

        // Light vs. Dark Themes
        "radiance",
        "luminous",
        "glow",
        "brilliance",
        "halo",
        "beacon",
        "sanctuary",
        "divine",
        "blessing",
        "seraphic",
        "shadow",
        "darkness",
        "gloom",
        "shroud",
        "veil",
        "phantasm",
        "eclipse",
        "midnight",
        "twilight",
        "abyssal",

        // Nature & Environment
        "forest",
        "grove",
        "meadow",
        "river",
        "ocean",
        "desert",
        "glacier",
        "volcano",
        "cavern",
        "mountain",
        "summit",
        "valley",
        "marsh",
        "swamp",
        "rainforest",
        "tundra",
        "island",
        "lake",
        "breeze",
        "tempest",

        // Special Abilities & Powers
        "teleport",
        "levitate",
        "absorb",
        "reflect",
        "counter",
        "regenerate",
        "mimic",
        "enhance",
        "intensify",
        "focus",
        "channel",
        "unleash",
        "dispel",
        "banish",
        "revive",
        "empower",
        "fortify",
        "accelerate",
        "concentrate",
        "awaken",

        // Miscellaneous Fantasy & Adventure Words
        "realm",
        "kingdom",
        "citadel",
        "sanctum",
        "throne",
        "oracle",
        "prophecy",
        "fate",
        "destiny",
        "omen",
        "scroll",
        "grimoire",
        "codex",
        "mystery",
        "tome",
        "ritual",
        "alchemy",
        "herb",
        "brew",
        "elixir",

        // More Combat & War-related Words
        "onslaught",
        "vanguard",
        "strike",
        "invasion",
        "crusade",
        "raid",
        "skirmish",
        "triumph",
        "conquer",
        "dominate",
        "resistance",
        "siege",
        "warcry",
        "glory",
        "victory",
        "rampage",
        "overthrow",
        "uprising",
        "rebellion",
        "slaughter",

        // Miscellaneous Words
        "balance",
        "chaos",
        "order",
        "time",
        "immortal",
        "ether",
        "cosmic",
        "void",
        "relic",
        "eclipse",
        "horizon",
        "legacy",
        "reverence",
        "divination",
        "oracle",
        "cataclysm",
        "arcadia",
        "guardian",
        "watcher",
        "monarch"
    ];

    private bool Sorted;
    public int WaveCount { get; set; }
    private bool WinnerDeclared;

    private void CalculateWinner()
    {
        var players = Subject.GetEntities<Aisling>()
                             .ToList();

        // Dictionary to store team scores
        var teamScores = new Dictionary<ArenaTeam, int>();

        foreach (var player in players)
        {
            // Retrieve player's kill count (default to 0 if not found)
            var playerKills = player.Trackers.Counters.TryGetValue("TypingMonsterKill", out var counter) ? counter : 0;

            // Get player's team
            if (player.Trackers.Enums.TryGetValue(typeof(ArenaTeam), out var value) && value is ArenaTeam arenaTeam)
            {
                if (!teamScores.ContainsKey(arenaTeam))
                    teamScores[arenaTeam] = 0;

                // Add player's kills to their team's total
                teamScores[arenaTeam] += playerKills;
            }
        }

        if (teamScores.Count == 0)
        {
            foreach (var player in players)
                player.SendActiveMessage("No teams were found, or no kills were recorded.");

            return;
        }

        // Determine the winning team
        var winningTeam = teamScores.OrderByDescending(t => t.Value)
                                    .First()
                                    .Key;
        var winningScore = teamScores[winningTeam];

        // Announce the winner
        foreach (var player in players)
        {
            var message = $"The winning team is {winningTeam} with {winningScore} total kills!";
            player.SendActiveMessage(message);
            player.Trackers.Counters.Remove("TypingMonsterKill", out _);
            var mapInstance = simpleCache.Get<MapInstance>("arena_underground");
            player.TraverseMap(mapInstance, new Point(13, 12));
        }

        WinnerDeclared = true;
    }

    private void EndGame(Monster monster)
    {
        var players = Subject.GetEntities<Aisling>()
                             .ToList();

        var creatures = Subject.GetEntities<Monster>()
                               .ToList();

        foreach (var player in players)
            player.SendActiveMessage($"A monster has reached the door on wave {monster.TypingWave}!");

        foreach (var creature in creatures)
            Subject.RemoveEntity(creature);

        WaveCount = 0;
        CalculateWinner();
    }

    private Point GetRandomSpawnLocation() => SpawnPoints[Random.Next(SpawnPoints.Count)];

    private string GetRandomWord() => WordPool[Random.Next(WordPool.Count)];

    private int GetValidSprite() => AllowedSprites[Random.Next(AllowedSprites.Count)];

    private int GetValidSpriteOld()
    {
        int sprite;

        do
            sprite = IntegerRandomizer.RollSingle(560);
        while (ExcludedSprites.Contains(sprite));

        return sprite;
    }

    public void SortAislingsIntoTiles()
    {
        var players = Subject.GetEntities<Aisling>()
                             .ToList();

        foreach (var player in players)
            player.WarpTo(new Point(10, 11));
    }

    public void SortAislingsIntoTilesTeamBased()
    {
        // Get all Aislings with valid ArenaTeam assignments
        var aislingTeams = Subject.GetEntities<Aisling>()
                                  .Select(
                                      x => new
                                      {
                                          Aisling = x,
                                          Team = x.Trackers.Enums.TryGetValue(typeof(ArenaTeam), out var value)
                                                 && value is ArenaTeam arenaTeam
                                              ? arenaTeam
                                              : (ArenaTeam?)null
                                      })
                                  .Where(x => x.Team.HasValue) // Ensure valid teams only
                                  .Select(
                                      x => new
                                      {
                                          x.Aisling,
                                          Team = (ArenaTeam?)x.Team!.Value
                                      })
                                  .ToList();

        // Group by ArenaTeam
        var teamGroups = aislingTeams.GroupBy(x => x.Team)
                                     .ToDictionary(
                                         g => g.Key,
                                         g => g.Select(a => a.Aisling)
                                               .ToList());

        // Flatten teams into a mixed queue for balanced distribution
        var mixedAislingQueue = teamGroups.Values
                                          .SelectMany(x => x.OrderBy(_ => Random.Next())) // Shuffle players across teams
                                          .ToList();

        // Shuffle the player tiles to avoid stacking
        var shuffledTiles = PlayerTiles.OrderBy(_ => Random.Next())
                                       .ToList();

        var index = 0;

        // Assign players to tiles in a mixed order
        foreach (var aisling in mixedAislingQueue)
        {
            var tile = shuffledTiles[index % PlayerTiles.Count]; // Cycle through tiles
            aisling.WarpTo(new Point(tile.X, tile.Y));
            index++;
        }
    }

    public override void Update(TimeSpan delta)
    {
        if (!Subject.GetEntities<Aisling>()
                    .Any())
        {
            if (WinnerDeclared)
                Subject.Destroy();

            return;
        }

        if (!Sorted)
        {
            SortAislingsIntoTiles();
            Sorted = true;
        }

        var monsterAtDoor = Subject.GetEntities<Monster>()
                                   .FirstOrDefault(
                                       monster => (monster.Template.TemplateKey == "typingMonster")
                                                  && DangerTiles.Contains(new Point(monster.X, monster.Y)));

        if (monsterAtDoor != null)
        {
            EndGame(monsterAtDoor);

            return;
        }

        SpawnTimer.Update(delta);

        if (!SpawnTimer.IntervalElapsed)
            return;

        WaveCount += 1;

        var players = Subject.GetEntities<Aisling>()
                             .ToList();

        foreach (var player in players)
            player.SendPersistentMessage($"Wave: {WaveCount}.");

        var monstersToSpawn = 3 + WaveCount / 2;

        for (var i = 0; i < monstersToSpawn; i++)
        {
            var spawnPoint = GetRandomSpawnLocation();
            var randomWord = GetRandomWord();
            var point = new Point(spawnPoint.X, spawnPoint.Y);

            var typingMonster = monsterFactory.Create("typingmonster", Subject, point);

            typingMonster.TypingWave = WaveCount;
            typingMonster.TypingWord = randomWord;
            typingMonster.Sprite = (ushort)GetValidSprite();
            Subject.AddEntity(typingMonster, point);
        }
    }
}