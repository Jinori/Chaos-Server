using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
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
    private const string BURNING_GROUND_REMINDER_EVENT_ID = nameof(BURNING_GROUND_REMINDER_EVENT_ID);
    private readonly Animation Animation;

    private readonly IIntervalTimer Animationtimer;

    //burning ground for shamensyth creant fight
    //will damage aislings by 20% hp and 10% mp per second
    //will heal shamensyth by 2% per second
    //will heal shamensythFireElemental by 5% per second
    //removable via strong water spells (tidalbreeze, ardsal, morsal, aoe versions of ard/mor sal)

    private readonly IIntervalTimer ApplicationTimer;
    private readonly IApplyDamageScript ApplyDamageScript;

    /// <inheritdoc />
    public BurningGroundScript(ReactorTile subject)
        : base(subject)
    {
        ApplicationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(100));
        Animationtimer = new IntervalTimer(TimeSpan.FromMilliseconds(700));
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();

        Animation = new Animation
        {
            TargetAnimation = 731,
            AnimationSpeed = 250
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
                if (aisling.IsDead)
                    continue;

                var healthDamage = MathEx.GetPercentOf<int>((int)aisling.UserStatSheet.EffectiveMaximumHp, 2);

                aisling.UserStatSheet.SubtractManaPct(1);

                ApplyDamageScript.ApplyDamage(
                    Subject.Owner!,
                    aisling,
                    this,
                    healthDamage,
                    Element.Fire);

                aisling.SendActiveMessage("You are standing on burning ground!");
            }

            var monstersStandingOnTile = Subject.MapInstance
                                                .GetEntitiesAtPoints<Monster>(Subject)
                                                .ToList();

            foreach (var monster in monstersStandingOnTile)
            {
                var templateKey = monster.Template.TemplateKey;

                if (!templateKey.EqualsI("shamensyth") && !templateKey.EqualsI("shamensythFireElemental"))
                    continue;

                var healPct = templateKey switch
                {
                    "shamensyth"              => 2,
                    "shamensythFireElemental" => 5,
                    _                         => 0
                };

                monster.StatSheet.AddHealthPct(healPct);

                if (!monster.Trackers.TimedEvents.HasActiveEvent(BURNING_GROUND_REMINDER_EVENT_ID, out _))
                {
                    monster.Trackers.TimedEvents.AddEvent(BURNING_GROUND_REMINDER_EVENT_ID, TimeSpan.FromSeconds(20), true);
                    monster.Say("Ahhh, flames of renewal!");
                }
            }
        }

        if (Animationtimer.IntervalElapsed)
            Subject.Animate(Animation);
    }
}