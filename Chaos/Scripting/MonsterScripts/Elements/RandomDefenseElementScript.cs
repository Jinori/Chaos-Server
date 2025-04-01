using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Elements;

public class RandomDefenseElementScript : ConfigurableMonsterScriptBase
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    private Element[] Elements { get; init; } = Array.Empty<Element>();

    /// <inheritdoc />
    public RandomDefenseElementScript(Monster subject)
        : base(subject)
    {
        var element = Elements.PickRandom();
        Subject.StatSheet.SetDefenseElement(element);
    }
}