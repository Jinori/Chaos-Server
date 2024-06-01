using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts;

internal class DmgTrinketScript : DialogScriptBase
{
    /// <inheritdoc />
    public DmgTrinketScript(Dialog subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    public readonly IEffectFactory EffectFactory;
    private Animation DmgAnimation { get; } = new()
    {
        TargetAnimation = 967,
        AnimationSpeed = 100
    };
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "dmgtrinket_dmgbuffyourselfyes":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dmgTrinket", out var dmgTime))
                {
                    Subject.Reply(source, $"The mystical energies need time to restore. You must wait {dmgTime.Remaining.ToReadableString()} before attempting to buff again.");
                    return;
                }
                DamageBoost(source, source);
                break;
            }
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.Equals("dmgtrinket_dmgbuffanother", StringComparison.CurrentCultureIgnoreCase))
        {
            if (source.Trackers.TimedEvents.HasActiveEvent("dmgTrinket", out var dmgTime))
            {
                Subject.Reply(source, $"The mystical energies need time to restore. You must wait {dmgTime.Remaining.ToReadableString()} before attempting to buff another.");
                return;
            }
            if (!TryFetchArgs<string>(out var name))
            {
                Subject.ReplyToUnknownInput(source);

                return;
            }

            var aisling = source.MapInstance.GetEntitiesWithinRange<Aisling>(source, 12).FirstOrDefault(x => x.Name == name);

            if (aisling != null)
                DamageBoost(source, aisling);
            else
                source.SendActiveMessage("No aisling by that name can be sensed in your vicinity.");   
        }
    }

    public void DamageBoost(Aisling source, Aisling target)
    {
        source.Trackers.TimedEvents.AddEvent("dmgTrinket", TimeSpan.FromHours(3), true);

        var effect = EffectFactory.Create("dmgtrinket");


        if (source.Name == target.Name)
            target.Effects.Apply(target, effect);

        if (source.Name != target.Name)
            target.Effects.Apply(target, effect);
        
        target.Animate(DmgAnimation, target.Id);
    }
}