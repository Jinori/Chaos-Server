using Chaos.Models.World;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.BulletinBoardScripts;

public sealed class BountyBoardScript : ReactorTileScriptBase
{
    private readonly IDialogFactory DialogFactory;
    private readonly IMerchantFactory MerchantFactory;

    /// <inheritdoc />
    public BountyBoardScript(ReactorTile subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        var point = new Point(source.X, source.Y);
        var board = MerchantFactory.Create("blank_merchant", Subject.MapInstance, point);
        var dialog = DialogFactory.Create("bountyboard_initial", board);
        dialog.Display(source);
    }
}