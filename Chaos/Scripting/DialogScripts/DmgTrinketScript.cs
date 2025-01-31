using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

internal class DmgTrinketScript : DialogScriptBase
{
    
    /// <inheritdoc />
    public DmgTrinketScript(Dialog subject, IEffectFactory effectFactory, ISimpleCache simpleCache)
        : base(subject)
    {
        EffectFactory = effectFactory;
        SimpleCache = simpleCache;
    }

    private readonly ISimpleCache SimpleCache;
    private readonly IEffectFactory EffectFactory;
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
            case "dmgtrinket_initial":
            {
                if (!source.Legend.ContainsKey("wpnsmth") && !source.IsGodModeEnabled())
                {
                    source.Inventory.RemoveQuantityByTemplateKey("dmgtrinket", 1);
                    source.SendOrangeBarMessage("The Claiomh Thugann Anam breaks in your undefined hands.");
                    Subject.Close(source);
                }

                break;
            }
            
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
            
            case "dmgtrinket_portalforge":
            {
                var targetMap = SimpleCache.Get<MapInstance>("tagor_forge");
                source.TraverseMap(targetMap, new Point(6, 6));
                source.SendActiveMessage("Claiomh Thugann Anam's blade beams with energy.");

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

    private void DamageBoost(Aisling source, Aisling target)
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