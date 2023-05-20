using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

[SuppressMessage("ReSharper", "UnusedVariable"), SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault"),
 SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
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

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("porteforest_initial", Subject);
                    dialog.Display(aisling);

                    return;
                }

                if (stage == PFQuestStage.StartedPFQuest)
                {
                    var dialog = DialogFactory.Create("porteforest_initial", Subject);
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

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);
                
                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("bertil_pf", Subject);
                    dialog.Display(aisling);
                }

                switch (stage)
                {
                    case PFQuestStage.TurnedInRoots:
                    {
                        var dialog = DialogFactory.Create("porteforest_initial2", Subject);
                        dialog.Display(aisling);

                        return;
                    }
                    case PFQuestStage.WolfManes:
                    {
                        var dialog = DialogFactory.Create("porteforest_initial2", Subject);
                        dialog.Display(aisling);

                        break;
                    }
                }

                break;
            }

            case "alvar":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("alvar_pf", Subject);
                    dialog.Display(aisling);
                }
            }

                break;

            case "berg":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("berg_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "eeva":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("eeva_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "fisk":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("fisk_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "goran":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("goran_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "gudny":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("gudny_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "hadrian":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("hadrian_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "valdemar":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("valdemar_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
            case "viveka":
            {
                if (source is not Aisling aisling)
                    return;

                if (aisling.UserStatSheet.Level is <= 10 or >= 42)
                    return;

                if (!message.EqualsI("Porte Forest"))
                    return;

                var hasStage = aisling.Trackers.Enums.TryGetValue(out PFQuestStage stage);

                if (!hasStage || (stage == PFQuestStage.None))
                {
                    var dialog = DialogFactory.Create("viveka_pf", Subject);
                    dialog.Display(aisling);
                }

                break;
            }
        }
    }
}