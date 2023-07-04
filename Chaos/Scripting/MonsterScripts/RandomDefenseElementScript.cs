using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RandomDefenseElementScript : ConfigurableMonsterScriptBase
{
    private Element[] Elements { get; } = Array.Empty<Element>();

    /// <inheritdoc />
    public RandomDefenseElementScript(Monster subject)
        : base(subject)
    {
        var element = Elements.PickRandom();
        Subject.StatSheet.SetDefenseElement(element);
    }
}