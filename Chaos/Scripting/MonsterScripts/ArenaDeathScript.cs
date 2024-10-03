using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class ArenaDeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ArenaDeathScript(Monster subject)
        : base(subject) { }

    private readonly Animation DeathAnimation = new()
    {
        SourceAnimation = 82,
        AnimationSpeed = 100
    };
    
    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveEntity(Subject))
            return;

        Map.ShowAnimation(DeathAnimation.GetPointAnimation(Subject));
        
        Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var playersOnArenaMap = Map.GetEntities<Aisling>().Where(x => !x.IsGodModeEnabled()).ToList();
        
        foreach (var item in Subject.Items)
        {
            var randomMember = playersOnArenaMap.PickRandom();
            randomMember.GiveItemOrSendToBank(item);
            randomMember.SendOrangeBarMessage($"You received {item.DisplayName} from {Subject.Name}");
        }
    }
}