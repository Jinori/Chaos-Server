using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts
{
    public sealed class RandomSpellScript : MonsterScriptBase
    {
        private static readonly string[] AllSpells =
        [
            "ardioc", "morioc", "nuadhaich", "salvation",
            "aoardcradh", "aomorcradh", "aodall", "aopoison", "aosuain", "dinarcoli",
            "poison", "suain", "beagpramh", "pramh",
            "armachd", "innerfire", "dodge", "battlecry", "rage", "warcry", "asgallfaileas", 
            "thunderstance", "wrath", "focus", "laststand", "earthenstance",
            "ardathar",
            "ardcreag",
            "ardsal",
            "ardsrad",
            "zap"
        ];

        private readonly ISpellFactory _spellFactory;

        public RandomSpellScript(Monster subject, ISpellFactory spellFactory)
            : base(subject)
        {
            _spellFactory = spellFactory;

            // Pick a random number of spells between 2 and 5
            var numberOfSpells = new Random().Next(2, 6);

            // Pick the specified number of random spells from AllSpells
            var randomSpells = PickRandomSpells(AllSpells, numberOfSpells);

            foreach (var spellKey in randomSpells)
            {
                var spell = _spellFactory.CreateFaux(spellKey);
                Spells.Add(spell);
            }
        }

        private static IEnumerable<string> PickRandomSpells(string[] allSpells, int numberOfSpells) => allSpells.OrderBy(_ => Guid.NewGuid()).Take(numberOfSpells);
    }
}