using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RandomSkillScript : MonsterScriptBase
{
    private readonly ISkillFactory SkillFactory;

    /// <inheritdoc />
    public RandomSkillScript(Monster subject, ISkillFactory skillFactory)
        : base(subject)
    {
        SkillFactory = skillFactory;

        var skillsToRandomize = new List<string>
            { "beagsuain", "windblade", "sapNeedle", "stab", "clawfist", "eaglestrike", "ambush" };

        var skill = SkillFactory.CreateFaux(skillsToRandomize.PickRandom());
        Skills.Add(skill);
    }
}