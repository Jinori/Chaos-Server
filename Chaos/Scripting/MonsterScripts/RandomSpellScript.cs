using Chaos.Common.Utilities;
using Chaos.Objects.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RandomSpellScript : MonsterScriptBase
{
    private readonly ISpellFactory SpellFactory;
    
    /// <inheritdoc />
    public RandomSpellScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;
        var spellsToRandomize = new List<string>() { "poison", "zap", "battleCry", "rage", "beagcradh", "cradh", "preventaffliction", "beagpramh" };

        var spell = SpellFactory.CreateFaux(spellsToRandomize.PickRandom());
        Spells.Add(spell);
    }
}