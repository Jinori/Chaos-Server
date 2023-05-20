using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class RandomSpellScript : MonsterScriptBase
{
    private readonly ISpellFactory SpellFactory;
    
    /// <inheritdoc />
    /// 


    private readonly string[] randomSpells =
    {
        "arcaneblast", "arcanebolt", "zap", "ardioc", "beagioc", "ioc", "morioc", "nuadhaich", "salvation",
        "ardcradh", "morcradh", "cradh", "beagcradh", "poison", "suain", "beagpramh", "pramh",
        "armachd", "innerfire", "mist", "battlecry", "rage", "warcry", "athar", "creag"
    };
    public RandomSpellScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;

        var spell = SpellFactory.CreateFaux(randomSpells.PickRandom());
        Spells.Add(spell);
    }
}