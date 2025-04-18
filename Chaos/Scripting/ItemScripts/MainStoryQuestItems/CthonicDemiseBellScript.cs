using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MapScripts.CthonicDemise;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts.MainStoryQuestItems;

public class CthonicDemiseBellScript(Item subject, ISimpleCache simpleCache, IMonsterFactory monsterFactory)
    : ItemScriptBase(subject)
{
    public Attributes BonusAttributes = null!;
    
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

    private void Bell(Aisling source)
    {
        var s = source.MapInstance.Script.As<CthonicDemiseScript>()!;

        SpawnLeader(source, RayRectangle,     "darkmasterray",     ref s.RaySpawned);
        SpawnLeader(source, RoyRectangle,     "darkmasterroy",     ref s.RoySpawned);
        SpawnLeader(source, JohnRectangle,    "darkmasterjohn",    ref s.JohnSpawned);
        SpawnLeader(source, JaneRectangle,    "darkmasterjane",    ref s.JaneSpawned);
        SpawnLeader(source, MikeRectangle,    "darkmastermike",    ref s.MikeSpawned);
        SpawnLeader(source, MaryRectangle,    "darkmastermary",    ref s.MarySpawned);
        SpawnLeader(source, PhilRectangle,    "darkmasterphil",    ref s.PhilSpawned);
        SpawnLeader(source, PamRectangle,     "darkmasterpam",     ref s.PamSpawned);
        SpawnLeader(source, WilliamRectangle, "darkmasterwilliam", ref s.WilliamSpawned);
        SpawnLeader(source, WandaRectangle,   "darkmasterwanda",   ref s.WandaSpawned);
    }

    private bool CanUseBell(Aisling source)
    {
        var mapInstance = simpleCache.Get<MapInstance>("cthonic_demise");
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
    
    private void SpawnLeader(
        Aisling source,
        Rectangle zone,
        string bossId,
        ref bool flagAlreadySpawned)
    {
        var script = source.MapInstance.Script.As<CthonicDemiseScript>()!;

        if (!zone.Contains(new Point(source.X, source.Y)))
            return;                                

        if (flagAlreadySpawned)
        {
            source.SendOrangeBarMessage("The leader isn't here anymore.");
            return;
        }

        /* 1) Mark & message */
        flagAlreadySpawned = true;
        source.SendOrangeBarMessage("You ring the bell loudly.");

        /* 2) Determine landing tile */
        var spawnArea = new Rectangle(source, 5, 5);
        Point p;
        do p = spawnArea.GetRandomPoint();
        while (!source.MapInstance.IsWalkable(p, CreatureType.Normal));

        var boss = monsterFactory.Create(bossId, source.MapInstance, p);

        /* --- bonus calculation ------------------------------------- */
        var bonusPct   = Math.Min(script.BossesSpawned * 0.05f, 0.50f);   // 0 % .. 50 %
        if (bonusPct > 0)
        {
            var b = boss.StatSheet;      // shorthand

            // Build an additive Attributes block = base × bonusPct
            var bonus = new Attributes
            {
                Dmg              = (short)(b.Dmg              * bonusPct),
                Hit              = (short)(b.Hit              * bonusPct),
                AtkSpeedPct      = (short)(b.AtkSpeedPct      * bonusPct),
                MaximumHp        = (int)  (b.MaximumHp        * bonusPct),
                MaximumMp        = (int)  (b.MaximumMp        * bonusPct),
                Str              = (short)(b.Str              * bonusPct),
                Dex              = (short)(b.Dex              * bonusPct),
                Con              = (short)(b.Con              * bonusPct),
                Wis              = (short)(b.Wis              * bonusPct),
                Int              = (short)(b.Int              * bonusPct),
                MagicResistance  = (short)(b.MagicResistance  * bonusPct),
            };

            b.AddBonus(bonus);
        }
        
        boss.StatSheet.SetHealthPct(100);
        boss.StatSheet.SetManaPct(100);
        source.MapInstance.AddEntity(boss, p);
        script.BossesSpawned++;                   
    }


    public override void OnUse(Aisling source)
    {
        if (CanUseBell(source))
            Bell(source);
    }
}