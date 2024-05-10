using System.Collections.Immutable;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Scripting.EffectScripts.HideEffects;

namespace Chaos.Scripting.Behaviors;

public class VisibilityBehavior
{
    private const int HIDE_THRESHOLD_SECONDS = 4;

    private readonly HashSet<string> BossKeys =
    [
        ..new[]
        {
            "crypt_boss5",
            "crypt_boss10",
            "crypt_boss15",
            "undead_king",
            "banshee",
            "souleater",
            "karlopos_king_octopus",
            "karlopos_queen_octopus",
            "bee1_boss",
            "bee2_boss",
            "bee3_boss",
            "bunny1_boss",
            "bunny2_boss",
            "bunny3_boss",
            "frog1_boss",
            "frog2_boss",
            "frog3_boss",
            "gargoyle1_boss",
            "gargoyle2_boss",
            "gargoyle3_boss",
            "grimlock1_boss",
            "grimlock2_boss",
            "grimlock3_boss",
            "horse1_boss",
            "horse2_boss",
            "horse3_boss",
            "kobold1_boss",
            "kobold2_boss",
            "kobold3_boss",
            "mantis1_boss",
            "mantis2_boss",
            "mantis3_boss",
            "wolf1_boss",
            "wolf2_boss",
            "wolf3_boss",
            "zombie1_boss",
            "zombie2_boss",
            "zombie3_boss",
            "pf_giant_mantis",
            "undine_field_carnun",
            "dragonscale_boss",
            "wilderness_abomination"
        }.Select(key => key.ToLower())
    ];
    
    private readonly ImmutableList<string> SeeTrueHiddenEffects = ImmutableList.Create(
        EffectBase.GetEffectKey(typeof(SeeTrueHideEffect)));

    public virtual bool CanSee(Creature creature, VisibleEntity entity) =>
        entity.Visibility switch
        {
            VisibilityType.Normal     => true,
            VisibilityType.Hidden     => CanSeeHidden(creature, entity),
            VisibilityType.TrueHidden => CanSeeTrueHidden(creature, entity),
            _                         => false
        };

    private bool CanSeeHidden(Creature creature, VisibleEntity entity) {
        var isInSameGroup = IsInSameGroup(creature, entity);
        var hasSeeHideEffect = HasSeeHideEffect(creature, entity);
        var canBossSee = CanBossSee(creature, entity);

        if (creature is Monster && entity is Aisling aisling)
        {
            var hasRecentlyHidden = HasRecentlyHidden(aisling);
            return hasRecentlyHidden;
        }

        return isInSameGroup || hasSeeHideEffect || canBossSee;
    }

    private bool HasSeeHideEffect(Creature creature, VisibleEntity entity) =>
        creature is Aisling && entity is Aisling && creature.Effects.Contains("See Hide");
    
    private bool CanSeeTrueHidden(Creature creature, VisibleEntity entity) =>
        IsInSameGroup(creature, entity) || CanBossSee(creature, entity) ||
        SeeTrueHiddenEffects.Any(creature.Effects.Contains);

    private bool IsInSameGroup(Creature creature, VisibleEntity entity) =>
        creature is Aisling aisling && entity is Aisling targetAisling &&
        (aisling.Group?.Contains(targetAisling) == true);

    private bool CanBossSee(Creature creature, VisibleEntity entity) =>
        creature is Monster monster && entity is Aisling &&
        BossKeys.Contains(monster.Template.TemplateKey.ToLower());

    private bool HasRecentlyHidden(Creature creature) => creature.Effects.Contains("Hide") && creature.Trackers.Counters.CounterLessThanOrEqualTo("HideSec", HIDE_THRESHOLD_SECONDS);
}
