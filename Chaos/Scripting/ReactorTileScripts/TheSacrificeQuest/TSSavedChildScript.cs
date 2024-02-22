using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class TSSavedChildScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public TSSavedChildScript(ReactorTile subject, IMerchantFactory merchantFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        MerchantFactory = merchantFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out TheSacrificeQuestStage stage);

        if (!hasStage || stage is not TheSacrificeQuestStage.RescueChildren)
            return;

        if (aisling.Trackers.Flags.HasFlag(SavedChild.savedchild))
            return;
        
        var randomNumber = new Random().Next(1, 101);

        if (randomNumber < 75)
        {
            Map.RemoveEntity(Subject);
            return;
        }
        
        var npcpoint = new Point(aisling.X, aisling.Y);
        
            var child = MerchantFactory.Create("tschild", source.MapInstance, npcpoint);
            var dialog = DialogFactory.Create("tschild_initial", child);
            dialog.Display(aisling);
            aisling.Trackers.Flags.AddFlag(SavedChild.savedchild);
            aisling.SendOrangeBarMessage("You stumbled across a child hiding, bring the child back to Chloe.");
    }
}