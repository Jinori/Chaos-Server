using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts;

internal class RepairTrinket : DialogScriptBase
{
    /// <inheritdoc />
    public RepairTrinket(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "repairtrinket_repairowngearyes":
            {
                if (source.Trackers.TimedEvents.HasActiveEvent("repairTrinket", out var repairTime))
                {
                    Subject.Reply(source, $"You must wait {repairTime.Remaining.ToReadableString()} to repair again.");
                    return;
                }
                RepairItems(source, source);
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
                Subject.Reply(source, $"You must wait {repairTime.Remaining.ToReadableString()} to repair again.");
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
                source.SendActiveMessage("An aisling with that name is not near.");   
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
            aisling.SendActiveMessage($"You have repaired your own items.");
        if (source.Name != aisling.Name)
            aisling.SendActiveMessage($"{source.Name} has repaired your items.");
    }
}