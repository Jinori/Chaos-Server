using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MapScripts.CthonicDemise;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using FluentAssertions;

namespace Chaos.Scripting.ItemScripts.MainStoryQuestItems;

public class Cdbell : ItemScriptBase
{
    private readonly Rectangle JaneRectangle = new(
        165,
        12,
        18,
        23);

    private readonly Rectangle JohnRectangle = new(
        174,
        76,
        29,
        22);

    private readonly Rectangle MaryRectangle = new(
        112,
        91,
        21,
        25);

    private readonly Rectangle MikeRectangle = new(
        6,
        40,
        19,
        14);

    private readonly IMonsterFactory MonsterFactory;

    private readonly Rectangle PamRectangle = new(
        5,
        106,
        30,
        16);

    private readonly Rectangle PhilRectangle = new(
        3,
        175,
        20,
        26);

    private readonly Rectangle RayRectangle = new(
        13,
        9,
        14,
        15);

    private readonly Rectangle RoyRectangle = new(
        151,
        148,
        21,
        30);

    private readonly ISimpleCache SimpleCache;

    private readonly Rectangle WandaRectangle = new(
        71,
        72,
        25,
        20);

    private readonly Rectangle WilliamRectangle = new(
        54,
        8,
        21,
        26);

    public Cdbell(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        MonsterFactory = monsterFactory;
    }

    private void Bell(Aisling source)
    {
        var sourcePoint = new Point(source.X, source.Y);
        var script = source.MapInstance.Script.As<CthonicDemiseScript>();

        // Check which rectangle the player is in and spawn the corresponding monster
        if (RayRectangle.Contains(sourcePoint))
        {
            if (script is { RaySpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.RaySpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monster1 = MonsterFactory.Create("darkmasterray", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (RoyRectangle.Contains(sourcePoint))
        {
            if (script is { RoySpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.RoySpawned = true;

            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterroy", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (JohnRectangle.Contains(sourcePoint))
        {
            if (script is { JohnSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.JohnSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly and then breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterjohn", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (JaneRectangle.Contains(sourcePoint))
        {
            if (script is { JaneSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.JaneSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterjane", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (MikeRectangle.Contains(sourcePoint))
        {
            if (script is { MikeSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.MikeSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmastermike", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (MaryRectangle.Contains(sourcePoint))
        {
            if (script is { MarySpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.MarySpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmastermary", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (PhilRectangle.Contains(sourcePoint))
        {
            if (script is { PhilSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.PhilSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterphil", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (PamRectangle.Contains(sourcePoint))
        {
            if (script is { PamSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.PamSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterpam", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (WilliamRectangle.Contains(sourcePoint))
        {
            if (script is { WilliamSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.WilliamSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterwilliam", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else if (WandaRectangle.Contains(sourcePoint))
        {
            if (script is { WandaSpawned: true })
            {
                source.SendOrangeBarMessage("The leader isn't here anymore.");

                return;
            }

            script.WandaSpawned = true;

            source.SendOrangeBarMessage("You ring the bell loudly then it breaks.");
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            var monsterSpawn = new Rectangle(source, 5, 5);
            var point1 = monsterSpawn.GetRandomPoint();
            var monster1 = MonsterFactory.Create("darkmasterwanda", source.MapInstance, point1);

            do
                point1 = monsterSpawn.GetRandomPoint();
            while (!source.MapInstance.IsWalkable(point1, CreatureType.Normal));

            source.MapInstance.AddEntity(monster1, point1);
        } else
            source.SendOrangeBarMessage("The bell echoes, but nothing happens.");
    }

    private bool CanUseBell(Aisling source)
    {
        var mapInstance = SimpleCache.Get<MapInstance>("cthonic_demise");
        source.Trackers.Enums.TryGetValue(out MainstoryMasterEnums stage);

        if (!source.MapInstance.Name.EqualsI(mapInstance.Name))
        {
            source.Inventory.RemoveQuantity(Subject.DisplayName, 1);
            source.SendOrangeBarMessage("The bell disintegrates.");

            return false;
        }

        if (stage is not MainstoryMasterEnums.StartedDungeon)
        {
            source.SendOrangeBarMessage("Only an Aisling who is on the quest can ring the bell.");

            return false;
        }

        if (source.MapInstance
                  .GetEntities<Monster>()
                  .Any(x => x.Template.Name.Contains("Dark Master")))
        {
            source.SendOrangeBarMessage("A leader is already somewhere in the map.");

            return false;
        }

        if (!source.IsAlive)
            return false;

        return true;
    }

    public override void OnUse(Aisling source)
    {
        if (CanUseBell(source))
            Bell(source);
    }
}