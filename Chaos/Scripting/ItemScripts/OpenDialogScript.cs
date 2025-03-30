using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class OpenDialogScript : ConfigurableItemScriptBase
{
    private readonly IDialogFactory DialogFactory;
    protected string DialogKey { get; init; } = null!;

    /// <inheritdoc />
    public OpenDialogScript(Item subject, IDialogFactory dialogFactory)
        : base(subject)
        => DialogFactory = dialogFactory;

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        var dialog = DialogFactory.Create(DialogKey, Subject);
        dialog.Display(source);
    }
}