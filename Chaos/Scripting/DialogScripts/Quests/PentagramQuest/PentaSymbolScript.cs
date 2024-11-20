using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest;

public class PentaSymbolScript(Dialog subject, IItemFactory itemFactory) : DialogScriptBase(subject)
{
    private readonly Dictionary<BaseClass, string> ClassToSymbol = new()
    {
        { BaseClass.Warrior, "pentawarriorsymbol1" },
        { BaseClass.Rogue, "pentaroguesymbol1" },
        { BaseClass.Wizard, "pentawizardsymbol1" },
        { BaseClass.Priest, "pentapriestsymbol1" },
        { BaseClass.Monk, "pentamonksymbol1" }
    };

    private readonly Dictionary<BaseClass, string> ClassToGem = new()
    {
        { BaseClass.Warrior, "Ruby" },
        { BaseClass.Rogue, "Emerald" },
        { BaseClass.Wizard, "Heartstone" },
        { BaseClass.Priest, "Sapphire" },
        { BaseClass.Monk, "Beryl" }
    };

    private readonly Dictionary<BaseClass, string> ClassToPentagramPiece = new()
    {
        { BaseClass.Warrior, "pentagrampiece5" },
        { BaseClass.Rogue, "pentagrampiece4" },
        { BaseClass.Wizard, "pentagrampiece1" },
        { BaseClass.Priest, "pentagrampiece3" },
        { BaseClass.Monk, "pentagrampiece2" }
    };

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "pentasymbol":
                AddOptionsForClass(source);
                break;
            case "pentasymbolend":
                GiveItemToClass(source);
                Subject.Close(source);
                source.Trackers.Enums.Set(PentagramQuestStage.FoundPentagramPiece);
                source.SendOrangeBarMessage("You have found your pentagram piece, but what is missing?");
                break;
            case "celestia_initial":
                HandleCelestiaInitial(source);
                break;
            case "empowerritualpiece3":
                AddEmpowermentOption(source);
                break;
            case "empowerritualpiecepriest":
            case "empowerritualpiecemonk":
            case "empowerritualpiecewarrior":
            case "empowerritualpiecewizard":
            case "empowerritualpiecerogue":
                source.Trackers.Enums.Set(PentagramQuestStage.EmpoweringPentagramPiece);
                break;
            case "empowerritualpiece4":
                CompleteEmpowerment(source);
                break;
            case "thorin_initial":
                HandleThorinInitial(source);
                break;
            case "craftpentagram4":
                CraftPentagram(source);
                break;
        }
    }

    private void AddOptionsForClass(Aisling source)
    {
        if (!ClassToSymbol.TryGetValue(source.UserStatSheet.BaseClass, out var optionKey))
            return;

        Subject.AddOption("Stare at the object...", optionKey);
    }

    private void GiveItemToClass(Aisling source)
    {
        if (!ClassToPentagramPiece.TryGetValue(source.UserStatSheet.BaseClass, out var itemKey))
            return;

        var item = itemFactory.Create(itemKey);
        source.GiveItemOrSendToBank(item);
    }

    private void HandleCelestiaInitial(Aisling source)
    {
        if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage))
            switch (stage)
            {
                case PentagramQuestStage.FoundPentagramPiece:
                    AddQuestDialogOption("empowerritualpiece1", "I found this.");

                    break;
                case PentagramQuestStage.EmpoweringPentagramPiece:
                    AddQuestDialogOption("empowerritualpiece4", "I have the Pristine Gem.");

                    break;
            }
    }

    private void AddQuestDialogOption(string dialogKey, string optionText)
    {
        var option = new DialogOption
        {
            DialogKey = dialogKey,
            OptionText = optionText
        };

        if (!Subject.HasOption(optionText))
            Subject.Options.Insert(0, option);
    }

    private void AddEmpowermentOption(Aisling source)
    {
        const string OPTION_TEXT = "What gem should I empower this with?";
        var dialogKey = $"empowerritualpiece{source.UserStatSheet.BaseClass.ToString().ToLower()}";

        var option = new DialogOption
        {
            DialogKey = dialogKey,
            OptionText = OPTION_TEXT
        };

        if (!Subject.HasOption(OPTION_TEXT))
            Subject.Options.Insert(0, option);
    }

    private void CompleteEmpowerment(Aisling source)
    {
        if (!ClassToGem.TryGetValue(source.UserStatSheet.BaseClass, out var itemKey))
            return;

        if (!ClassToPentagramPiece.TryGetValue(source.UserStatSheet.BaseClass, out var pentapiece))
            return;

        var item = $"Pristine {itemKey}";

        if (source.Inventory.HasCountByTemplateKey(pentapiece, 1) && source.Inventory.HasCount(item, 1))
        {
            source.Inventory.RemoveQuantityByTemplateKey(pentapiece, 1);
            source.Inventory.RemoveQuantity(item, 1);
            source.Trackers.Enums.Set(PentagramQuestStage.EmpoweredPentagramPiece);
            var empoweredPiece = itemFactory.Create($"{itemKey}PentagramPiece");
            source.GiveItemOrSendToBank(empoweredPiece);

            Subject.Reply(
                source,
                "All done, that was nerve wracking... If you and your group head to the forge, talk to Thorin, he might be able to put the pieces together.");
        }
        else
            Subject.Reply(source, $"Where's the {item} and Pentagram Piece that I need to do this procedure?");
    }

    private void HandleThorinInitial(Aisling source)
    {
        if (source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage) &&
            (stage == PentagramQuestStage.EmpoweredPentagramPiece))
            AddQuestDialogOption("craftpentagram1", "Will you help us craft the Pentagram?");
    }

    private void CraftPentagram(Aisling source)
    {
        if (source.Group == null)
        {
            Subject.Reply(source, "You're not in a group.");
            return;
        }

        if (source.Group.Any(x => !x.OnSameMapAs(source) || !x.WithinRange(source)))
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Your full group must be nearby.");
            Subject.Reply(source, "Your entire group is not here.");
            return;
        }

        var hasWizard = false;
        var hasMonk = false;
        var hasRogue = false;
        var hasWarrior = false;
        var hasPriest = false;

        foreach (var member in source.Group!)
        {
            switch (member.UserStatSheet.BaseClass.ToString())
            {
                case "Wizard":
                    hasWizard = true;
                    break;
                case "Monk":
                    hasMonk = true;
                    break;
                case "Rogue":
                    hasRogue = true;
                    break;
                case "Warrior":
                    hasWarrior = true;
                    break;
                case "Priest":
                    hasPriest = true;
                    break;
            }
        }

        if (!(hasWizard && hasMonk && hasRogue && hasWarrior && hasPriest))
        {
            Subject.Reply(source, "Your group is missing one or more required classes.");
            return;
        }

        foreach (var groupMember in source.Group!)
        {
            if (!groupMember.Trackers.Enums.TryGetValue(out PentagramQuestStage stage)
                || (stage != PentagramQuestStage.EmpoweredPentagramPiece))
            {
                Subject.Reply(source, "One of your members is missing the empowered ritual piece.");
                return;
            }

            if (!ClassToGem.TryGetValue(groupMember.UserStatSheet.BaseClass, out var itemKey))
                return;

            var pieceKey = $"{itemKey} Pentagram Piece";

            if (!groupMember.Inventory.HasCount(pieceKey, 1))
            {
                Subject.Close(source);
                source.SendOrangeBarMessage($"Your group's missing the {pieceKey}.");
                return;
            }
        }

        var pentagram = itemFactory.Create("Pentagram");
        var wizardName = source.Group.FirstOrDefault(x => x.UserStatSheet.BaseClass == BaseClass.Wizard);

        foreach (var groupMember in source.Group)
        {
            if (!ClassToGem.TryGetValue(groupMember.UserStatSheet.BaseClass, out var itemKey))
                return;

            groupMember.Inventory.RemoveQuantityByTemplateKey($"{itemKey}PentagramPiece", 1);

            if (groupMember.UserStatSheet.BaseClass == BaseClass.Wizard)
            {
                groupMember.GiveItemOrSendToBank(pentagram);
                groupMember.SendOrangeBarMessage("Thorin hands you the crafted Pentagram.");
            }
            else
                groupMember.SendOrangeBarMessage($"You watch Thorin hand the crafted pentagram to {wizardName?.Name}.");

            groupMember.Trackers.Enums.Set(PentagramQuestStage.CreatedPentagram);
        }

        Subject.Reply(source,
            $"Thorin hands the Pentagram to {wizardName?.Name}, then addresses the group.\nGood luck to you all, you'll need it.");
    }
}