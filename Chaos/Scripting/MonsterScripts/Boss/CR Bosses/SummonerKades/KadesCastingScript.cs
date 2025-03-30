using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.CR_Bosses.SummonerKades;

// ReSharper disable once ClassCanBeSealed.Global
public class KadesCastingScript : MonsterScriptBase
{
    /// <inheritdoc />
    public KadesCastingScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Target is not { IsAlive: true } || !ShouldUseSpell || !Target.WithinRange(Subject))
            return;

        if (Subject.MapInstance
                   .GetEntities<Aisling>()
                   .Any(
                       x => x.Trackers.Enums.HasValue(SummonerBossFight.FirstStage)
                            || x.Trackers.Enums.HasValue(SummonerBossFight.SecondStage)
                            || x.Trackers.Enums.HasValue(SummonerBossFight.ThirdStage)
                            || x.Trackers.Enums.HasValue(SummonerBossFight.FourthStage)
                            || x.Trackers.Enums.HasValue(SummonerBossFight.FifthStage)))
            return;

        Spells.ShuffleInPlace();

        var spell = Spells.Where(
                              spell => Subject.CanUse(
                                  spell,
                                  Target,
                                  null,
                                  out _))
                          .PickRandomWeightedSingleOrDefault(7);

        if (spell is not null && Subject.TryUseSpell(spell, Target.Id))
        {
            Subject.WanderTimer.Reset();
            Subject.MoveTimer.Reset();
            Subject.SkillTimer.Reset();
        }
    }
}