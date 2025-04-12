using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.DubhaimCastle;

public class DubhaimAltarScript(ReactorTile subject, IItemFactory itemFactory, IMonsterFactory monsterFactory)
    : ReactorTileScriptBase(subject)
{
    private readonly IMonsterFactory MonsterFactory = monsterFactory;
    
    private static readonly Dictionary<string, SummonInfo> SUMMONABLES = new()
    {
        // itemTemplateKey        monsterTemplateKey   needsGrandmaster?   on‑spawn text
        ["blood_diamond"]  = new SummonInfo("dc_kindlefiend",  true,  "Kindlefiend is born from the Blood Diamond!"), 
    };

    private readonly record struct SummonInfo(string MonsterKey,
        bool RequiresGrandmaster,
        string SpawnMessage);

    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        if (source is not Aisling aisling)
            return;
        
        if (!SUMMONABLES.TryGetValue(groundItem.Item.Template.TemplateKey, out var summon))
            return;

        if (summon.RequiresGrandmaster &&
            !aisling.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
        {
            ReturnItemWithMessage("You must be a Grand Master to do what you were about to do.");
            return;
        }

        if (aisling.MapInstance.GetEntities<Monster>()
                   .Any(m => m.Template.TemplateKey == summon.MonsterKey))
        {
            ReturnItemWithMessage($"{summon.MonsterKey} is already summoned.");
            return;
        }
        
        var point   = new Point(aisling.X - IntegerRandomizer.RollSingle(3),
            aisling.Y - IntegerRandomizer.RollSingle(3));

        var monster = MonsterFactory.Create(summon.MonsterKey, aisling.MapInstance, point);
        aisling.MapInstance.AddEntity(monster, point);
        aisling.SendOrangeBarMessage(summon.SpawnMessage);

        aisling.MapInstance.RemoveEntity(groundItem);

        void ReturnItemWithMessage(string msg)
        {
            aisling.GiveItemOrSendToBank(itemFactory.Create(groundItem.Item.Template.TemplateKey));
            aisling.MapInstance.RemoveEntity(groundItem);
            aisling.SendOrangeBarMessage(msg);
        }
    }
}