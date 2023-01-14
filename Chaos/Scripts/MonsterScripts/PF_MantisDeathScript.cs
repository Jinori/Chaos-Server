using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Formulae;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts.Components;

// ReSharper disable once ClassCanBeSealed.Global
public class PF_MantisDeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public PF_MantisDeathScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var rewardTarget = Subject.AggroList
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Map.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null);


        if (rewardTarget is not null)
        {
            if (rewardTarget.Flags.HasFlag (QuestFlag1.IsabelleQuest))
            {
                rewardTarget.Flags.RemoveFlag(QuestFlag1.IsabelleQuest);
                rewardTarget.Flags.AddFlag(QuestFlag1.IsabelleMantisDead);
            }
            
        }
    }
}