using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Elements;

public class RandomOffenseElementScript : ConfigurableMonsterScriptBase
{
    private Element[] Elements { get; init; } = Array.Empty<Element>();

    /// <inheritdoc />
    public RandomOffenseElementScript(Monster subject)
        : base(subject)
    {
        var element = Elements.PickRandom();
        Subject.StatSheet.SetOffenseElement(element);
    }
}