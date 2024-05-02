using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.PentagramQuest;

public class ZombieFleshScript : ReactorTileScriptBase
{
    private static readonly Point WizardSpot = new Point(10, 13);
    private static readonly Point WarriorSpot = new Point(7, 7);
    private static readonly Point RogueSpot = new Point(11, 6);
    private static readonly Point PriestSpot = new Point(13, 10);
    private static readonly Point MonkSpot = new Point(6, 11);
    private readonly ISimpleCache SimpleCache;
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    public ZombieFleshScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory,
        ISimpleCache simpleCache)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
        SimpleCache = simpleCache;
    }

    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        if (source is not Aisling aisling)
            return;

        // Check if dropped item is Zombie Flesh
        if (groundItem.Name != "Zombie Flesh")
            return;

        // Check if player is a Wizard
        if (aisling.UserStatSheet.BaseClass != BaseClass.Wizard)
            return;

        // Check if player is in a group with exactly 5 members
        if (aisling.Group is not { Count: 5 })
        {
            aisling.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your group must be exactly 5 members to start the ritual.");

            return;
        }

        if (!CanStartRitual(aisling))
            return;

        var requiredClasses = new HashSet<BaseClass>
        {
            BaseClass.Warrior,
            BaseClass.Rogue,
            BaseClass.Wizard,
            BaseClass.Priest,
            BaseClass.Monk
        };

        var encounteredClasses = new HashSet<BaseClass>();

        // Check if group members have required classes
        foreach (var groupMember in aisling.Group)
        {
            var memberClass = groupMember.UserStatSheet.BaseClass;

            if (!requiredClasses.Contains(memberClass))
            {
                aisling.Client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    "Your group contains a class that is not required for the ritual.");

                return;
            }

            if (encounteredClasses.Contains(memberClass))
            {
                aisling.Client.SendServerMessage(
                    ServerMessageType.OrangeBar1,
                    "There are too many of one class in your group for the ritual.");

                return;
            }

            encounteredClasses.Add(memberClass);
        }

        // Check if group members are in the correct position
        foreach (var groupMember in aisling.Group)
        {
            if (!CheckPosition(groupMember))
            {
                foreach (var member in aisling.Group)
                {
                    member.SendOrangeBarMessage("Your group must be ready to perform the ritual.");
                }

                return;
            }
        }

        // Check quest stage and perform actions accordingly
        foreach (var groupMember in aisling.Group)
        {
            if (groupMember.Trackers.Enums.TryGetValue(out PentagramQuestStage stage))
            {
                var ani = new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 120
                };

                switch (stage)
                {
                    case PentagramQuestStage.SignedPact:
                    {
                        groupMember.Animate(ani);
                        groupMember.Trackers.Enums.Set(PentagramQuestStage.StartedRitual);
                        var clueitem = ItemFactory.Create("Pentagram");
                        var classDialog = DialogFactory.Create("pentaritualstart1", clueitem);
                        classDialog.Display(groupMember);
                        groupMember.SendOrangeBarMessage("You have started the ritual.");

                        break;
                    }

                    case PentagramQuestStage.CreatedPentagram:
                    case PentagramQuestStage.BossSpawned:
                    {
                        if (!CanStartBoss(groupMember, out var mapInstance))
                            return;
                        
                        groupMember.Trackers.Enums.Set(PentagramQuestStage.BossSpawning);
                        groupMember.SendOrangeBarMessage("The ritual has begun...");

                        break;
                    }
                }
            }
        }

        aisling.MapInstance.RemoveEntity(groundItem);
    }

    private bool CheckPosition(Aisling player) =>
        player.UserStatSheet.BaseClass switch
        {
            BaseClass.Rogue   => RogueSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Wizard  => WizardSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Priest  => PriestSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Warrior => WarriorSpot.Equals(new Point(player.X, player.Y)),
            BaseClass.Monk    => MonkSpot.Equals(new Point(player.X, player.Y)),
            _                 => false
        };

    private bool CanStartRitual(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage);
        
        if (source.Group == null)
        {
            source.SendOrangeBarMessage("You must be grouped to perform the ritual.");
            return false; 
        }

        foreach (var groupmember in source.Group!)
        {
            if (stage is not PentagramQuestStage.SignedPact and PentagramQuestStage.DefeatedBoss)
            {
                groupmember.SendOrangeBarMessage("You have not signed the pact to start the ritual.");
                return false;
            }
            
        }
        
        return true;
    }
     private bool CanStartBoss(Aisling source, out MapInstance mapInstance)
    {
        mapInstance = SimpleCache.Get<MapInstance>("hm_macabre_pentagram");
        source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage);

        if (MapHasMonsters(mapInstance))
        {
            source.SendOrangeBarMessage("The boss is already summoned.");
            return false;
        }

        if (source.Group == null)
        {
            source.SendOrangeBarMessage("You must be grouped to perform the ritual.");
            return false; 
        }

        foreach (var groupmember in source.Group!)
        {
            if (stage is not (PentagramQuestStage.CreatedPentagram or PentagramQuestStage.BossSpawned))
            {
                foreach (var groupmember2 in source.Group!)
                {
                    groupmember2.SendOrangeBarMessage("Someone inside the group hasn't created the pentagram.");
                }
                return false;
            }
        }
        return true;
    }
    
    private bool MapHasMonsters(MapInstance mapInstance)
    {
        var monstersOnMap = mapInstance.GetEntities<Monster>().Where(entity => !entity.Template.TemplateKey.Contains("Pet"));

        if (monstersOnMap.Any())
            return true;

        return false;
    }
}

