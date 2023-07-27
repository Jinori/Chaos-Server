using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MapScripts.Abstractions;

namespace Chaos.Scripting.MapScripts;

public class HideAndSeekGameScript : MapScriptBase
{
    private bool AislingsTouching;
    private IApplyDamageScript ApplyDamageScript { get; }
    private Aisling? TouchOne;
    private Aisling? TouchTwo;

    private Animation BlowupAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 49
    };
    
    /// <inheritdoc />
    public HideAndSeekGameScript(MapInstance subject)
        : base(subject) =>
        ApplyDamageScript = ApplyAttackDamageScript.Create();

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        if (!AislingsTouching)
        {
            var aislings = Subject.GetEntities<Aisling>().Where(x => x.IsAlive).ToList();

            foreach (var aisling in aislings)
            {
                var otherAisling = aislings.FirstOrDefault(other => (!other.Equals(aisling)) && (aisling.DistanceFrom(other) <= 1));

                if (otherAisling != null)
                {
                    TouchOne = aisling;
                    TouchTwo = otherAisling;
                    AislingsTouching = true;
                    break;
                }
            }
        }
        else
        {
            if ((TouchOne != null) && (TouchTwo != null))
            {
                // Aislings are caught touching and map will shrink
                ApplyDamageScript.ApplyDamage(
                    TouchOne,
                    TouchTwo,
                    this,
                    500000);

                ApplyDamageScript.ApplyDamage(
                    TouchTwo,
                    TouchOne,
                    this,
                    500000);
            
                Subject.ShowAnimation(BlowupAnimation.GetPointAnimation(TouchOne));
                Subject.ShowAnimation(BlowupAnimation.GetPointAnimation(TouchTwo));
            
                TouchOne = null;
                TouchTwo = null;
                AislingsTouching = false;   
            }
        }
    }
}