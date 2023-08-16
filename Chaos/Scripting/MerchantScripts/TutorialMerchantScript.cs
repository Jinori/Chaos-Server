using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

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

        if (!aisling.Trackers.Enums.TryGetValue<TutorialQuestStage>(out var stage))
            return;

        if (stage != TutorialQuestStage.GaveArmor)
            return;

        if (!message.EqualsI("hello"))
            return;

        var dialog = DialogFactory.Create("leia_2", Subject);
        dialog.Display(aisling);
    }
}