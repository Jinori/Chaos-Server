using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class AreiniPetsScript : DialogScriptBase
{
    public AreiniPetsScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (string.Equals(Subject.Template.TemplateKey, "areini_initial", StringComparison.OrdinalIgnoreCase)
            && (source.UserStatSheet.BaseClass != BaseClass.Priest))
        {
            RemoveOption("Learn Summon Pet");
            RemoveOption("Change Pet");
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (string.Equals(Subject.Template.TemplateKey, "areini_changepet", StringComparison.OrdinalIgnoreCase))
            if (optionIndex.HasValue)
            {
                var optionText = Subject.GetOptionText((int)optionIndex - 1);

                if (Enum.TryParse<SummonChosenPet>(optionText, true, out var chosenPet))
                    switch (chosenPet)
                    {
                        case SummonChosenPet.None:
                            break;
                        case SummonChosenPet.Gloop:
                        case SummonChosenPet.Bunny:
                        case SummonChosenPet.Faerie:
                        case SummonChosenPet.Dog:
                        case SummonChosenPet.Ducklings:
                        case SummonChosenPet.Cat:
                            source.Trackers.Enums.Set(chosenPet);
                            source.SendActiveMessage($"You've chosen {chosenPet}!");

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                    source.SendActiveMessage("Something went wrong!");
            }
    }

    private void RemoveOption(string optionName)
    {
        var optionIndex = Subject.GetOptionIndex(optionName);

        if (optionIndex.HasValue)
            Subject.Options.RemoveAt(optionIndex.Value);
    }
}