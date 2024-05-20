using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Class_Related;

public class PetCollarScript(
    Dialog subject,
    IMonsterFactory monsterFactory,
    ISkillFactory skillFactory,
    ISpellFactory spellFactory
)
    : DialogScriptBase(subject)
{
    
    private readonly HashSet<string> ArenaKeys = new(StringComparer.OrdinalIgnoreCase) { "arena_battle_ring", "arena_lava", "arena_lavateams", "arena_colorclash", "arena_escort"};
    
    
    public override void OnDisplaying(Aisling source)
    {
        if (source.UserStatSheet.BaseClass is not BaseClass.Priest)
        {
            Subject.Reply(source, "You attempt to use the collar but it burns your hands, forcing you to drop it.");
            RemoveExistingPets(source);
            source.Inventory.RemoveQuantity("Pet Collar", 1);
            return;
        }
        
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "petcollar_home":
                RemoveExistingPets(source);
                break;
            case "petcollar_summonpet":
                HandleSummonPet(source);
                break;
        }
    }

    private void RemoveExistingPets(Aisling source)
    {
        var pets = source.MapInstance.GetEntities<Monster>().Where(x => x.Script.Is<PetScript>() && x.Name.Contains(source.Name));

        var nopetSummoned = !source.MapInstance.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>() && x.Name.Contains(source.Name));

        if (nopetSummoned)
        {
            Subject.Reply(source, "Your pet is not currently summoned");
            source.SendOrangeBarMessage("Your pet is not currently summoned.");
            return;
        }
        
        foreach (var pet in pets)
        {
            pet.MapInstance.RemoveEntity(pet);
            source.Trackers.TimedEvents.AddEvent("PetDeath", TimeSpan.FromMinutes(5), true);
        }
    }

    private void HandleSummonPet(Aisling source)
    {
        var petSummoned = source.MapInstance.GetEntities<Monster>().Any(x => x.Script.Is<PetScript>() && x.Name.Contains(source.Name));

        if (petSummoned)
        {
            source.SendOrangeBarMessage($"Your pet is currently summoned.");
            return;
        }
        
        if (source.Trackers.TimedEvents.HasActiveEvent("PetDeath", out var timedEvent))
        {
            source.SendActiveMessage($"Your pet recently came home. Please wait {timedEvent.Remaining.Humanize()}.");
            return;
        }
        
        source.Trackers.Enums.TryGetValue(out SummonChosenPet petKey);

        if (petKey == SummonChosenPet.None)
        {
            source.SendActiveMessage("You have not selected a pet type with Areini!");
            return;
        }

        SummonPet(source, petKey);
    }

    private void SummonPet(Aisling source, SummonChosenPet petKey)
    {        
        if (ArenaKeys.Contains(source.MapInstance.LoadedFromInstanceId))
        {
            source.SendOrangeBarMessage("You cannot summon pets on an arena map.");
            return;
        }
        var newMonster = monsterFactory.Create(petKey + "pet", source.MapInstance, source);
        InitializeNewPet(source, newMonster);
        source.MapInstance.AddEntity(newMonster, source);
    }

    private void InitializeNewPet(Aisling source, Monster newMonster)
    {
        newMonster.Name = $"{source.Name}'s {newMonster.Name}";
        newMonster.PetOwner = source;
        var attributes = new Attributes
        {
            Ac = 100 - source.StatSheet.Level,
            Con = source.StatSheet.EffectiveCon + source.StatSheet.Level,
            Dex = source.StatSheet.EffectiveDex + source.StatSheet.Level,
            Int = source.StatSheet.EffectiveInt + source.StatSheet.Level,
            Str = source.StatSheet.EffectiveStr + source.StatSheet.Level,
            Wis = source.StatSheet.EffectiveWis + source.StatSheet.Level,
            AtkSpeedPct = source.StatSheet.Level / 2,
            MaximumHp = source.StatSheet.Level * 1000 / 7 + 1000,
            MaximumMp = source.StatSheet.Level * 500 / 7 + 1000
        };

        newMonster.StatSheet.SetOffenseElement(Elements.PickRandom());
        newMonster.StatSheet.SetDefenseElement(source.UserStatSheet.DefenseElement);
        newMonster.StatSheet.SetLevel(source.StatSheet.Level);
        newMonster.StatSheet.AddBonus(attributes);
        newMonster.StatSheet.SetHealthPct(100);
        newMonster.StatSheet.SetManaPct(100);

        AssignPetSkillsAndSpells(source, newMonster);
    }

    private void AssignPetSkillsAndSpells(Aisling source, Monster newMonster)
    {
        if (source.Trackers.Enums.TryGetValue(out Level10PetSkills level10Skill))
            switch (level10Skill)
            {
                case Level10PetSkills.RabidBite:
                    newMonster.Skills.Add(skillFactory.Create("poisonpunch"));

                    break;
                case Level10PetSkills.Growl:
                    newMonster.Spells.Add(spellFactory.Create("howl"));

                    break;
                case Level10PetSkills.QuickAttack:
                    newMonster.Skills.Add(skillFactory.Create("stab"));

                    break;
            }

        if (source.Trackers.Enums.TryGetValue(out Level25PetSkills level25Skill))
            switch (level25Skill)
            {
                case Level25PetSkills.PawStrike:
                    newMonster.Skills.Add(skillFactory.Create("mantiskick"));

                    break;
                case Level25PetSkills.Enrage:
                    newMonster.Skills.Add(skillFactory.Create("clawfist"));

                    break;
                case Level25PetSkills.WindStrike:
                    newMonster.Skills.Add(skillFactory.Create("windblade"));

                    break;
            }

        if (source.Trackers.Enums.TryGetValue(out Level40PetSkills level40Skill))
            switch (level40Skill)
            {
                case Level40PetSkills.Blitz:
                    newMonster.Skills.Add(skillFactory.Create("blitz"));

                    break;
                case Level40PetSkills.Slobber:
                    newMonster.Skills.Add(skillFactory.Create("clobber"));

                    break;
                case Level40PetSkills.DoubleLick:
                    newMonster.Skills.Add(skillFactory.Create("doublepunch"));

                    break;
            }

        if (source.Trackers.Enums.TryGetValue(out Level55PetSkills level55Skill))
            switch (level55Skill)
            {
                case Level55PetSkills.Frenzy:
                    newMonster.Spells.Add(spellFactory.Create("focus"));

                    break;
                case Level55PetSkills.Spit:
                    newMonster.Spells.Add(spellFactory.Create("arcanebolt"));

                    break;
                case Level55PetSkills.Evade:
                    newMonster.Spells.Add(spellFactory.Create("dodge"));

                    break;
            }
        
        if (source.Trackers.Enums.TryGetValue(out Level80PetSkills level80Skills))
            switch (level80Skills)
            {
                case Level80PetSkills.ChitinChew:
                    newMonster.Skills.Add(skillFactory.Create("armorbreak"));

                    break;
                case Level80PetSkills.SnoutStun:
                    newMonster.Skills.Add(skillFactory.Create("groundstomp"));

                    break;
                case Level80PetSkills.EssenceLeechLick:
                    newMonster.Skills.Add(skillFactory.Create("sapneedle"));

                    break;
            }
    }
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!optionIndex.HasValue)
            return;

        var templateKey = Subject.Template.TemplateKey.ToLower();
        var optionText = Subject.GetOptionText((int)optionIndex);

        switch (templateKey)
        {
            case "petcollar_changeappearance":
                HandleChangeAppearance(source, optionText);
                break;
            case "petcollar_combatstance":
                HandleCombatStance(source, optionText);
                break;
            case "petcollar_petfollow":
                HandlePetFollow(source, optionText);
                break;
        }
    }

    private void HandleChangeAppearance(Aisling source, string? optionText)
    {
        if (Enum.TryParse(optionText?.Replace(" ", ""), true, out SummonChosenPet petType))
        {
            source.Trackers.Enums.Set(petType);
            source.SendActiveMessage($"You've chosen {petType} as your pet's appearance!");
        }
        else
            source.SendActiveMessage("Invalid pet type selected!");
    }

    private void HandleCombatStance(Aisling source, string? optionText)
    {
        if (Enum.TryParse(optionText?.Replace(" ", ""), true, out PetMode petMode))
        {
            source.Trackers.Enums.Set(petMode);
            source.SendActiveMessage($"Pet combat stance set to {petMode}.");
        }
        else
        {
            source.SendActiveMessage("Invalid combat stance selected!");
        }
    }

    private void HandlePetFollow(Aisling source, string? optionText)
    {
        var cleanedOptionText = NormalizeFollowModeText(optionText);

        if (Enum.TryParse(cleanedOptionText, out PetFollowMode followMode))
        {
            source.Trackers.Enums.Set(followMode);
            source.SendActiveMessage($"Your pet will now {GetFollowModeDescription(followMode)}.");
        }
        else
        {
            source.SendActiveMessage("Invalid follow mode selected!");
        }
    }

    private string? NormalizeFollowModeText(string? optionText) =>
        optionText switch
        {
            "At Feet"       => "AtFeet",
            "Follow Behind" => "FollowAtDistance",
            "Stay"    => "DontMove",
            "Wander"        => "Wander",
            _               => optionText?.Replace(" ", "") 
        };

    private string GetFollowModeDescription(PetFollowMode mode) =>
        mode switch
        {
            PetFollowMode.AtFeet           => "stay close to you",
            PetFollowMode.Wander           => "wander freely",
            PetFollowMode.DontMove         => "remain stationary",
            PetFollowMode.FollowAtDistance => "follow you at a distance",
            _                              => "have an unknown behavior"
        };

    private Element[] Elements { get; } =
    [
        Element.Fire,
        Element.Water,
        Element.Wind,
        Element.Earth
    ];
}
