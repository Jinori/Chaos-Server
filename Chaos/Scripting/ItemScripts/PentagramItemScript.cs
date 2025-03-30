using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class PentagramItemScript : ItemScriptBase
{
    private static readonly Point WizardSpot = new(10, 13);
    private static readonly Point WarriorSpot = new(7, 7);
    private static readonly Point RogueSpot = new(11, 6);
    private static readonly Point PriestSpot = new(13, 10);
    private static readonly Point MonkSpot = new(6, 11);
    private static readonly Point PentagramSpot = new(5, 5);
    private readonly ISimpleCache _simpleCache;

    public PentagramItemScript(Item subject, ISimpleCache simpleCache)
        : base(subject)
        => _simpleCache = simpleCache;

    private bool CanStartBoss(Aisling source, out MapInstance mapInstance)
    {
        mapInstance = _simpleCache.Get<MapInstance>("hm_macabre_pentagram");
        source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage);

        if (!source.IsAlive || !source.Inventory.Contains("Pentagram") || !source.MapInstance.Name.EqualsI(mapInstance.Name))
            return false;

        if (MapHasMonsters(mapInstance))
        {
            source.SendOrangeBarMessage("The boss is already summoned.");

            return false;
        }

        if (source.Group != null)
        {
            switch (source.Group!)
            {
                case { Count: > 5 }:
                    source.SendOrangeBarMessage("There are too many of you to perform the ritual.");

                    return false;
                case { Count: < 5 }:
                    source.SendOrangeBarMessage("You're missing someone, they must be here with you.");

                    return false;
            }

            foreach (var groupmember in source.Group!)
                if (stage is not (PentagramQuestStage.CreatedPentagram or PentagramQuestStage.BossSpawned))
                {
                    groupmember.SendOrangeBarMessage("Someone inside the group hasn't created the pentagram.");

                    return false;
                }

            foreach (var groupMember in source.Group!)
                if (!CheckPosition(groupMember))
                {
                    foreach (var member in source.Group)
                        member.SendOrangeBarMessage("Your group must be ready to perform the ritual.");

                    return false;
                }
        }

        return true;
    }

    private bool CheckPosition(Aisling player)
        => player.UserStatSheet.BaseClass switch
        {
            BaseClass.Rogue   => RogueSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Wizard  => WizardSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Priest  => PriestSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Warrior => WarriorSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Monk    => MonkSpot.Equals(new Point(player.X, player.Y)),
            _                 => false
        };

    private bool MapHasMonsters(MapInstance mapInstance)
    {
        foreach (var entity in mapInstance.GetEntities<Creature>())
            if (entity is Monster)
                return true;

        return false;
    }

    public override void OnUse(Aisling source)
    {
        if (!CanStartBoss(source, out var mapInstance))
            return;

        foreach (var groupmember in source.Group!)
        {
            groupmember.Trackers.Enums.Set(PentagramQuestStage.BossSpawning);
            groupmember.SendOrangeBarMessage("The ritual has begun.");
        }
    }
}