using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.MerchantScripts;

public class PFQuestMerchant : MerchantScriptBase
{
    private readonly IDialogFactory DialogFactory;

    /// <inheritdoc />
    public PFQuestMerchant(Merchant subject, IDialogFactory dialogFactory)
        : base(subject) =>
        DialogFactory = dialogFactory;

    /// <inheritdoc />
    public override void OnPublicMessage(Creature source, string message)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "torbjorn":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("porteforest_initial", Subject);
                    dialog.Display(aisling);

                    return;
                }

                if (stage == PFQuestStage.StartedPFQuest)
                {
                    var dialog = DialogFactory.Create("porteforest_rootturnin", Subject);
                    dialog.Display(aisling);
                }

                if (stage == PFQuestStage.TurnedInRoots)

                {
                    var dialog = DialogFactory.Create("porteforest_initial", Subject);
                    dialog.Display(aisling);
                }
            }

                break;

            case "bertil":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Enums.TryGetValue(out PFQuestStage stage);

                if (stage == PFQuestStage.TurnedInRoots)
                {
                    var dialog = DialogFactory.Create("porteforest_initial2", Subject);
                    dialog.Display(aisling);

                    return;
                }

                if (stage == PFQuestStage.WolfManes)
                {
                    var dialog = DialogFactory.Create("porteforest_wolfmanes", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
        }
    }
}