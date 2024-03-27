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
            RemoveOption("Pet Level 10 Ability");
            RemoveOption("Pet Level 25 Ability");
            RemoveOption("Pet Level 40 Ability");
            RemoveOption("Pet Level 55 Ability");
            RemoveOption("Change Pet");
        }
        if (string.Equals(Subject.Template.TemplateKey, "areini_initial", StringComparison.OrdinalIgnoreCase)
            && (source.UserStatSheet.BaseClass == BaseClass.Priest))
        {
            if (source.SkillBook.Contains("Summon Pet"))
            {
                RemoveOption("Learn Summon Pet");
            }
            // Check and remove dialog options based on pet skill level
            if (source.Trackers.Flags.TryGetFlag(out PetSkillsAvailable flag) && flag != PetSkillsAvailable.Level10)
            {
                RemoveOption("Pet Level 10 Ability");
            }

            if (flag != PetSkillsAvailable.Level25)
            {
                RemoveOption("Pet Level 25 Ability");
            }
            if (flag != PetSkillsAvailable.Level40)
            {
                RemoveOption("Pet Level 40 Ability");
            }
            if (flag != PetSkillsAvailable.Level55)
            {
                RemoveOption("Pet Level 55 Ability");
            }
            // Add similar checks for other levels if necessary
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
                        case SummonChosenPet.Smoldy:
                        case SummonChosenPet.Penguin:
                            source.Trackers.Enums.Set(chosenPet);
                            RemoveExistingPets(source);
                            source.SendActiveMessage($"You've chosen {chosenPet}!");

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                else
                    source.SendActiveMessage("Something went wrong!");
            }

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "areini_pickdoublelick":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level40);
                    source.Trackers.Enums.Set(Level40PetSkills.DoubleLick);
                    source.SendActiveMessage("Double Lick ability learned!");
                }

                break;
            }

            case "areini_pickslobber":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level40);
                    source.Trackers.Enums.Set(Level40PetSkills.Slobber);
                    source.SendActiveMessage("Slobber ability learned!");
                }

                break;
            }

            case "areini_pickblitz":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level40);
                    source.Trackers.Enums.Set(Level40PetSkills.Blitz);
                    source.SendActiveMessage("Blitz ability learned!");
                }

                break;
            }
            
            case "areini_pickpawstrike":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level25);
                    source.Trackers.Enums.Set(Level25PetSkills.PawStrike);
                    source.SendActiveMessage("Paw Strike ability learned!");
                }

                break;
            }

            case "areini_pickenrage":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level25);
                    source.Trackers.Enums.Set(Level25PetSkills.Enrage);
                    source.SendActiveMessage("Enrage ability learned!");
                }

                break;
            }

            case "areini_pickwindstrike":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level25);
                    source.Trackers.Enums.Set(Level25PetSkills.WindStrike);
                    source.SendActiveMessage("Wind Strike ability learned!");
                }

                break;
            }

            case "areini_pickrabidbite":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level10);
                    source.Trackers.Enums.Set(Level10PetSkills.RabidBite);
                    source.SendActiveMessage("Rabid Bite ability learned!");
                }

                break;
            }
            case "areini_pickgrowl":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level10);
                    source.Trackers.Enums.Set(Level10PetSkills.Growl);
                    source.SendActiveMessage("Growl ability learned!");
                }

                break;
            }
            case "areini_pickquickattack":
            {
                if (optionIndex != 2)
                {
                    RemoveExistingPets(source);
                    source.Trackers.Flags.AddFlag(PetSkillsChosen.Level10);
                    source.Trackers.Enums.Set(Level10PetSkills.QuickAttack);
                    source.SendActiveMessage("Quick Attack ability learned!");
                }

                break;
            }
        }
    }

    private void RemoveExistingPets(Aisling source)
    {
        var monsters = source.MapInstance.GetEntities<Monster>();

        foreach (var monster in monsters)
            if (monster.Name.Contains(source.Name))
                monster.MapInstance.RemoveEntity(monster);
    }

    private void RemoveOption(string optionName)
    {
        var optionIndex = Subject.GetOptionIndex(optionName);

        if (optionIndex.HasValue)
            Subject.Options.RemoveAt(optionIndex.Value);
    }
}