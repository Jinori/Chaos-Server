using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Creants.Shamensyth;

public class BurningGroundScript : ReactorTileScriptBase
{
    private readonly IIntervalTimer ApplicationTimer;
    private readonly IIntervalTimer Animationtimer;
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly Animation Animation;
    
    /// <inheritdoc />
    public BurningGroundScript(ReactorTile subject)
        : base(subject)
    {
        ApplicationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        Animationtimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();

        Animation = new Animation
        {
            TargetAnimation = 211,
            AnimationSpeed = 500
        };
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ApplicationTimer.Update(delta);
        Animationtimer.Update(delta);

        if (ApplicationTimer.IntervalElapsed)
        {

            var aislingsStandingOnTile = Subject.MapInstance
                                                .GetEntitiesAtPoints<Aisling>(Subject)
                                                .ToList();

            foreach (var aisling in aislingsStandingOnTile)
            {
                var healthDamage = MathEx.GetPercentOf<int>((int)aisling.UserStatSheet.EffectiveMaximumHp, 20);

                aisling.UserStatSheet.SubtractManaPct(20);
                
                ApplyDamageScript.ApplyDamage(
                    Subject.Owner!,
                    aisling,
                    this,
                    healthDamage,
                    Element.Fire);
                
                aisling.SendActiveMessage("You are standing on burning ground!");
            }
        }

        if (Animationtimer.IntervalElapsed)
            Subject.Animate(Animation);
    }
}