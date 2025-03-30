using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public sealed class RandomSkillScript : MonsterScriptBase
{
    private static readonly string[] AllSkills =
    [
        "beagsuain",
        "shockwave",
        "sapNeedle",
        "devour",
        "clawfist",
        "madsoul",
        "ambush",
        "multistrike",
        "crasher",
        "assassinstrike",
        "frostbomb",
        "dracotailkick",
        "seismickick"
    ];

    private readonly ISkillFactory SkillFactory;

    /// <inheritdoc />
    public RandomSkillScript(Monster subject, ISkillFactory skillFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;

        // Pick a random number of spells between 2 and 5
        var numberOfSkills = new Random().Next(0, 7);

        // Pick the specified number of random spells from AllSpells
        var randomSkills = PickRandomSkills(AllSkills, numberOfSkills);

        foreach (var spellKey in randomSkills)
        {
            var skill = SkillFactory.Create(spellKey);
            Skills.Add(skill);
        }
    }

    private static IEnumerable<string> PickRandomSkills(string[] allSkills, int numberOfSkills)
        => AllSkills.OrderBy(_ => Guid.NewGuid())
                    .Take(numberOfSkills);
}