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

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        if (string.Equals(Subject.Template.TemplateKey, "petcollar_home", StringComparison.OrdinalIgnoreCase))
        {
            var monsters = source.MapInstance.GetEntities<Monster>();

            foreach (var monster in monsters)
                if (monster.Name.Contains(source.Name))
                    monster.MapInstance.RemoveEntity(monster);
        }        
    }

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

                return;
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
            }

        if (string.Equals(Subject.Template.TemplateKey, "petcollar_petfollow", StringComparison.OrdinalIgnoreCase))
            if (optionIndex.HasValue)
            {
                var optionText = Subject.GetOptionText((int)optionIndex - 1);

                if (!string.IsNullOrEmpty(optionText))
                {
                    var chosenMode = ParseFollowMode(optionText);

                    if (chosenMode.HasValue)
                        switch (chosenMode.Value)
                        {
                            case PetFollowMode.Wander:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage($"You tell your pet it can {chosenMode.Value}.");
                                Subject.Reply(source, "Your pet will now wander around.");

                                break;
                            case PetFollowMode.AtFeet:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage("You tell your pet to stay close.");
                                Subject.Reply(source, "Your pet will now stay near your side.");

                                break;
                            case PetFollowMode.FollowAtDistance:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage("You tell your pet to follow behind you.");
                                Subject.Reply(source, "Your pet will now follow you at a short distance.");

                                break;
                            case PetFollowMode.DontMove:
                                source.Trackers.Enums.Set(chosenMode.Value);
                                source.SendActiveMessage("You tell your pet not to move.");
                                Subject.Reply(source, "Your pet will now stay unless provoked.");

                                break;
                        }
                }
            }
    }

    private PetFollowMode? ParseFollowMode(string optionText) =>
        optionText.Replace(" ", "") switch
        {
            "AtFeet"       => PetFollowMode.AtFeet,
            "FollowBehind" => PetFollowMode.FollowAtDistance,
            "Stay"         => PetFollowMode.DontMove,
            "Wander"       => PetFollowMode.Wander,
            _              => null
        };
}