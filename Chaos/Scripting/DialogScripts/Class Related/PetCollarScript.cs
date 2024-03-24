using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class PetCollarScript : DialogScriptBase
{
    /// <inheritdoc />
    public PetCollarScript(Dialog subject)
        : base(subject)
    { }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (string.Equals(Subject.Template.TemplateKey, "petcollar_changeappearance", StringComparison.OrdinalIgnoreCase))
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
                        case SummonChosenPet.Smoldy:
                        case SummonChosenPet.Penguin:
                            source.Trackers.Enums.Set(chosenPet);
                            source.SendActiveMessage($"You've chosen {chosenPet}!");

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                    source.SendActiveMessage("Something went wrong!");
            }

        if (string.Equals(Subject.Template.TemplateKey, "petcollar_combatstance", StringComparison.OrdinalIgnoreCase))
            if (optionIndex.HasValue)
            {
                var optionText = Subject.GetOptionText((int)optionIndex - 1);
                if (Enum.TryParse<PetMode>(optionText, true, out var chosenMode))
                    switch (chosenMode)
                    {
                        case PetMode.Defensive:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behaviors.");
                            Subject.Reply(source, "Your pet will watch your back for anything aggressive to you.");
                            break;
                        case PetMode.Offensive:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behavior.");
                            Subject.Reply(source, "Your pet will attack anything that comes within range.");
                            break;
                        case PetMode.Assist:
                            source.Trackers.Enums.Set(chosenMode);
                            source.SendActiveMessage($"You've chosen {chosenMode} pet behavior.");
                            Subject.Reply(source, "Your pet will attack the most aggressive monster in group range.");
                            break;
                    }
                else
                    source.SendActiveMessage("Something went wrong!");
            }
    }
}