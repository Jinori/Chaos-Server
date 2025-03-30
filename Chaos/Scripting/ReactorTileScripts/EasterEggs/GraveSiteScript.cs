using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.EasterEggs;

public sealed class GraveSiteScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IItemFactory ItemFactory;
    private readonly IMonsterFactory MonsterFactory;

    /// <inheritdoc />
    public GraveSiteScript(
        ReactorTile subject,
        IItemFactory itemFactory,
        IDialogFactory dialogFactory,
        IMonsterFactory monsterFactory)
        : base(subject)
    {
        ItemFactory = itemFactory;
        DialogFactory = dialogFactory;
        MonsterFactory = monsterFactory;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        if (!source.Trackers.Flags.HasFlag(Definitions.EasterEggs.GraveSite))
        {
            var point = new Point(source.X, source.Y);
            var monster = MonsterFactory.Create("ww_faerie", Subject.MapInstance, point);
            var dialog = DialogFactory.Create("bellagrave_initial", monster);
            dialog.Display(source);

            return;
        }

        if (source.Trackers.Flags.HasFlag(Definitions.EasterEggs.GraveSite))
        {
            source.SendOrangeBarMessage("You've already found Bella's Grave.");

            return;
        }

        source.Trackers.Flags.AddFlag(Definitions.EasterEggs.GraveSite);
        var toydoll = ItemFactory.Create("faewings");
        source.TryGiveGamePoints(5);
        source.TryGiveGold(20000);
        source.GiveItemOrSendToBank(toydoll);
        source.SendOrangeBarMessage("Do not share Easter Eggs with others.");
    }
}