using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Items.AlchemyPotions;

public class AmnesiaEffect : ContinuousAnimationEffectBase
{
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
    
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 42
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));

    protected IReadOnlyCollection<string> ConflictingEffectNames { get; } = new[]
    {
        "Amnesia"
    };
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(700));
    /// <inheritdoc />
    public override byte Icon => 15;
    /// <inheritdoc />
    public override string Name => "Amnesia";

    protected void OnApply()
    {
        if (Subject is not Monster monster)
            return;
        
        if (BossKeys.Contains(monster.Template.TemplateKey.ToLower())) 
            return;

        monster.ResetAggro();
    }

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        if (Subject is not Monster monster)
            return;
        
        if (BossKeys.Contains(monster.Template.TemplateKey.ToLower())) 
            return;

        monster.ResetAggro();
    }
}