using Chaos.Objects.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class DefaultScript : MerchantScriptBase
{
    private readonly IDialogFactory DialogFactory;

    /// <inheritdoc />
    public DefaultScript(IDialogFactory dialogFactory, Merchant subject)
        : base(subject) =>
        DialogFactory = dialogFactory;

    /// <inheritdoc />
    public override void OnClicked(Aisling source)
    {
        if (string.IsNullOrWhiteSpace(InitialDialogKey))
            return;

        var dialog = DialogFactory.Create(InitialDialogKey, Subject);
        dialog.Display(source);
    }
}