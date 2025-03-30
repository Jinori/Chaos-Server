using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class PFPendantScript(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
    : ItemScriptBase(subject)
{
    private bool CanSpawnMantis(Aisling source, out MapInstance mapInstance)
    {
        mapInstance = simpleCache.Get<MapInstance>("PF_peak");

        if (!source.IsAlive || !source.Inventory.Contains("Turuc Pendant") || !source.MapInstance.Name.EqualsI(mapInstance.Name))
            return false;
        
        var mantisIsSpawned = source.MapInstance.GetEntities<Monster>().Any(x => x.Template.TemplateKey.EqualsI("PF_giant_mantis"));

        if (mantisIsSpawned)
        {
            source.SendOrangeBarMessage("The Giant Mantis is already on the map.");

            return false;
        }

        return true;
    }

    private Monster CreateMantis(MapInstance mapInstance, Aisling source) =>
        monsterFactory.Create("pf_giant_mantis", mapInstance, Point.From(source));

    private Animation GetMonsterAnimation() => new()
        { AnimationSpeed = 100, TargetAnimation = 97 };

    private Animation GetPlayerAnimation() => new()
        { AnimationSpeed = 100, TargetAnimation = 160 };

    private Point GetSpawnPoint(Creature mantis, MapEntity source)
    {
        var monsterSpawn = new Rectangle(source, 10, 10);
        var outline = monsterSpawn.GetOutline().ToList();
        Point point;

        do
            point = outline.PickRandom();
        while (!source.MapInstance.IsWalkable(point, collisionType: mantis.Type));

        return point;
    }

    public override void OnUse(Aisling source)
    {
        if (!CanSpawnMantis(source, out var mapInstance))
            return;

        SpawnMantis(source, mapInstance);
    }

    private void SpawnMantis(Aisling source, MapInstance mapInstance)
    {
        var mantis = CreateMantis(mapInstance, source);
        var point = GetSpawnPoint(mantis, source);

        var groupLevel = source.MapInstance.GetEntities<Aisling>()
            .Where(aisling => !aisling.Trackers.Enums.HasValue(GodMode.Yes))
            .Select(aisling => aisling.StatSheet.Level)
            .ToList();

        var attrib = new Attributes
        {
            Con = (int)groupLevel.Average(),
            Dex = (int)groupLevel.Average(),
            Int = (int)groupLevel.Average(),
            Str = (int)groupLevel.Average(),
            Wis = (int)groupLevel.Average(),
            AtkSpeedPct = groupLevel.Count * 8 + 3,
            MaximumHp = (int)(mantis.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 200),
            MaximumMp = (int)(mantis.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 200),
            SkillDamagePct = (int)(mantis.StatSheet.SkillDamagePct + groupLevel.Average() / 3 + groupLevel.Count + 5),
            SpellDamagePct = (int)(mantis.StatSheet.SpellDamagePct + groupLevel.Average() / 3 + groupLevel.Count + 5)
        };
        
        mantis.StatSheet.AddBonus(attrib);
        // Add HP and MP to the mantis
        mantis.StatSheet.SetHealthPct(100);
        mantis.StatSheet.SetManaPct(100);
        mantis.SetLocation(point);

        source.MapInstance.AddEntity(mantis, point);
        mantis.Animate(GetMonsterAnimation());

        foreach (var player in mapInstance.GetEntities<Aisling>())
        {
            player.SendOrangeBarMessage("You feel the ground shake, a quick shadow overcasts you");
            player.Animate(GetPlayerAnimation());
        }
    }
}