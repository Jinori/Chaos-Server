using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

public class TutorialMerchantScript : MerchantScriptBase
{
    private readonly IDialogFactory DialogFactory;

    /// <inheritdoc />
    public TutorialMerchantScript(Merchant subject, IDialogFactory dialogFactory)
        : base(subject) =>
        DialogFactory = dialogFactory;

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        if (source is not Aisling aisling)
            return;

        if (!aisling.Flags.HasFlag(TutorialQuestFlag.GaveStickAndArmor))
            return;

        if (aisling.Flags.HasFlag(TutorialQuestFlag.GaveAssailAndSpell))
            return;

        if (!message.EqualsI("hello"))
            return;

        var dialog = DialogFactory.Create("leia_2", Subject);
        dialog.Display(aisling);
    }
}