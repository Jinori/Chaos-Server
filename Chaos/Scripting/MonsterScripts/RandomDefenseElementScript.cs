using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.World;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RandomDefenseElementScript : ConfigurableMonsterScriptBase
{
    protected Element[] Elements { get; init; } = Array.Empty<Element>();

    /// <inheritdoc />
    public RandomDefenseElementScript(Monster subject)
        : base(subject)
    {
        var element = Elements.PickRandom();

        Subject.StatSheet.SetDefenseElement(element);
    }
}