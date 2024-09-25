using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class MysteriousArtifactScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public MysteriousArtifactScript(ReactorTile subject, IItemFactory itemFactory, IDialogFactory dialogFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        var hasStage = aisling.Trackers.Enums.TryGetValue(out MainStoryEnums stage);

        if (hasStage)
            return;

        if (IntegerRandomizer.RollChance(30))
        {
            Map.RemoveEntity(Subject);
            return;
        }

        if (!hasStage)
        {
            var artifact = ItemFactory.Create("mysteriousartifact");
            var dialog = DialogFactory.Create("mysteriousartifact_initial", artifact);
            aisling.GiveItemOrSendToBank(artifact);
            source.Trackers.Enums.Set(MainStoryEnums.ReceivedMA);
            dialog.Display(aisling);
            aisling.SendOrangeBarMessage("A mysterious artifact...");
            Map.RemoveEntity(Subject);
        }
    }
}