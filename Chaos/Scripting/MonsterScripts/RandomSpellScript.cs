using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public sealed class RandomSpellScript : MonsterScriptBase
{
    private static readonly string[] AllSpells =
    {
        "ardioc", "beagioc", "ioc", "morioc", "nuadhaich", "salvation",
        "poison", "suain", "beagpramh", "pramh",
        "armachd", "innerfire", "mist", "battlecry", "rage", "warcry",
        "ardathar", "athar", "atharlamh", "atharmeall", "morathar",
        "ardcreag", "creag", "creaglamh", "creagmeall", "morcreag",
        "ardsal", "morsal", "salmeall",
        "ardsrad", "morsrad", "sradmeall",
        "arcaneblast", "arcanebolt", "zap"
    };
    private readonly ISpellFactory SpellFactory;

    public RandomSpellScript(Monster subject, ISpellFactory spellFactory)
        : base(subject)
    {
        SpellFactory = spellFactory;

        var spell = SpellFactory.CreateFaux(AllSpells.PickRandom());
        Spells.Add(spell);
    }
}