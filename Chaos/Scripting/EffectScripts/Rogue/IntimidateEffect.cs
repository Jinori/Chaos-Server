using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.Rogue;

public class IntimidateEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(2);

    /// <inheritdoc />
    public override byte Icon => byte.MaxValue;

    /// <inheritdoc />
    public override string Name => "Intimidate";

    /// <inheritdoc />
    public override void OnApplied()
    {
        var directionOfSource = Source.DirectionalRelationTo(Subject);

        if (directionOfSource is Direction.Invalid)
            return;

        var awayFromSource = directionOfSource.Reverse();
        Subject.Turn(awayFromSource, true);
    }
}