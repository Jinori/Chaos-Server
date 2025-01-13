using Chaos.Collections;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

internal class RepairTrinketScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public RepairTrinketScript(Dialog subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
    }

    private Animation RepairAnimation { get; } = new()
    {
        TargetAnimation = 595,
        AnimationSpeed = 100
    };
    
    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "repairtrinket_initial":
            {
                if (!source.Legend.ContainsKey("armsmith") && !source.IsGodModeEnabled())
                {
                    source.Inventory.RemoveQuantityByTemplateKey("repairtrinket", 1);
                    source.SendOrangeBarMessage("The Deisiu Earrai breaks in your undefined hands.");
                    Subject.Close(source);
                }

                break;
            }
            
            case "repairtrinket_repairowngearyes":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("repairTrinket", out var repairTime))
                {
                    Subject.Reply(source, $"The mystical energies need time to restore. You must wait {repairTime.Remaining.ToReadableString()} before attempting another repair.");
                    return;
                }
                RepairItems(source, source);
                break;
            }
            
                    
            case "repairtrinket_portalascent":
            {
                var targetMap = SimpleCache.Get<MapInstance>("wilderness_armorsmithing");
                source.TraverseMap(targetMap, new Point(8, 6));
                source.SendActiveMessage("The Deisui Earrai gleams with mystical energy.");
                break;
            }
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.Equals("repairtrinket_repairanothersgear", StringComparison.CurrentCultureIgnoreCase))
        {
            if (source.Trackers.TimedEvents.HasActiveEvent("repairTrinket", out var repairTime))
            {
                Subject.Reply(source, $"The mystical energies need time to restore. You must wait {repairTime.Remaining.ToReadableString()} before attempting another repair.");
                return;
            }
            if (!TryFetchArgs<string>(out var name))
            {
                Subject.ReplyToUnknownInput(source);

                return;
            }

            var aisling = source.MapInstance.GetEntitiesWithinRange<Aisling>(source, 12).FirstOrDefault(x => x.Name == name);

            if (aisling != null)
                RepairItems(source, aisling);
            else
                source.SendActiveMessage("No aisling by that name can be sensed in your vicinity.");   
        }
    }

    public void RepairItems(Aisling source, Aisling aisling)
    {
       source.Trackers.TimedEvents.AddEvent("repairTrinket", TimeSpan.FromHours(6), true);
        
        foreach (var repair in aisling.Equipment)
            if ((repair.Template.MaxDurability > 0) && (repair.CurrentDurability != repair.Template.MaxDurability))
            {
                repair.CurrentDurability = repair.Template.MaxDurability;
                repair.LastWarningLevel = 100;
            }

        foreach (var repair in aisling.Inventory)
            if ((repair.Template.MaxDurability > 0) && (repair.CurrentDurability != repair.Template.MaxDurability))
                aisling.Inventory.Update(
                    repair.Slot,
                    _ =>
                    {
                        repair.CurrentDurability = repair.Template.MaxDurability;
                        repair.LastWarningLevel = 100;
                    });
        
        if (source.Name == aisling.Name) 
            aisling.SendActiveMessage($"You have successfully restored your own items.");
        if (source.Name != aisling.Name)
            aisling.SendActiveMessage($"{source.Name}'s mastery of smithing has restored your items.");
        
        aisling.Animate(RepairAnimation, aisling.Id);
    }
}