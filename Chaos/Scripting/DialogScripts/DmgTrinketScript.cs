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
            
            case "dmgtrinket_dmgbuffgroupyes":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dmgTrinket", out var dmgTime))
                {
                    Subject.Reply(source, $"The mystical energies need time to restore. You must wait {dmgTime.Remaining.ToReadableString()} before attempting to buff again.");
                    return;
                }

                if (source.Group == null)
                {
                    Subject.Reply(source, "You're not in a group.");
                    return;
                }

                source.Trackers.TimedEvents.AddEvent("dmgTrinket", TimeSpan.FromHours(4), true);
                
                foreach (var player in source.Group)
                    DamageBoost(player);

                break;
            }
            
            case "dmgtrinket_dmgbuffyourselfyes":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("dmgTrinket", out var dmgTime))
                {
                    Subject.Reply(source, $"The mystical energies need time to restore. You must wait {dmgTime.Remaining.ToReadableString()} before attempting to buff again.");
                    return;
                }
                
                source.Trackers.TimedEvents.AddEvent("dmgTrinket", TimeSpan.FromHours(2), true);
                DamageBoost(source);
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
    

    private void DamageBoost(Aisling target)
    {
        var effect = EffectFactory.Create("dmgtrinket");
        target.Effects.Apply(target, effect);
        target.Animate(DmgAnimation, target.Id);
    }
}