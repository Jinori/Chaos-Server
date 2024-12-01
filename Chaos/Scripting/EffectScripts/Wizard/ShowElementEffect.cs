using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts.Wizard;

public class ShowElementEffect : ContinuousAnimationEffectBase
{
    private string ElementChant = "None";
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(9);
    protected Point Point { get; set; }

    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 295
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(3), false);

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromSeconds(3), false);

    public override byte Icon => 19;
    public override string Name => "Show Element";

    public override void OnApplied()
    {
        base.OnApplied();

        if (Subject is Aisling)
            return;

        // Check the subject's defense element and set the corresponding animation and chant
        switch (Subject.StatSheet.DefenseElement)
        {
            case Element.Earth:
                Animation.TargetAnimation = 401;
                ElementChant = "Earth";

                break;

            case Element.Water:
                Animation.TargetAnimation = 402;
                ElementChant = "Water";

                break;

            case Element.Fire:
                Animation.TargetAnimation = 404;
                ElementChant = "Fire";

                break;

            case Element.Wind:
                Animation.TargetAnimation = 403;
                ElementChant = "Wind";

                break;

            case Element.Darkness:
                Animation.TargetAnimation = 76;
                ElementChant = "Dark";

                break;

            case Element.Holy:
                Animation.TargetAnimation = 277;
                ElementChant = "Light";

                break;

            case Element.Metal:
                Animation.TargetAnimation = 237;
                ElementChant = "Metal";

                break;

            case Element.Wood:
                Animation.TargetAnimation = 235;
                ElementChant = "Wood";

                break;

            case Element.Undead:
                Animation.TargetAnimation = 233;
                ElementChant = "Nature";

                break;

            case Element.None:
                Animation.TargetAnimation = 363;
                ElementChant = "None";

                break;
        }

        // Perform the initial chant and animation
        Subject.Animate(Animation);
        Subject.Chant(ElementChant);
    }

    public override void OnDispelled() => OnTerminated();

    protected override void OnIntervalElapsed()
    {
        // Repeat the animation and chant at each interval
        Subject.Animate(Animation);
        Subject.Chant(ElementChant);
    }

    public override bool ShouldApply(Creature source, Creature target)
    {
        if (target is Aisling { IsDead: true })
            return false;

        if (target.Effects.Contains("Show Element"))
        {
            target.Effects.Dispel("Show Element");

            return true;
        }

        return true;
    }
}