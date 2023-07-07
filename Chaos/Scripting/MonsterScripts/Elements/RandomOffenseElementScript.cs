using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Elements;

public class RandomOffenseElementScript : ConfigurableMonsterScriptBase
{
    private Element[] Elements { get; } =
    {
        Element.Fire,
        Element.Water,
        Element.Wind,
        Element.Earth
    };

    /// <inheritdoc />
    public RandomOffenseElementScript(Monster subject)
        : base(subject)
    {
        var element = Elements.PickRandom();
        Subject.StatSheet.SetOffenseElement(element);
    }
}