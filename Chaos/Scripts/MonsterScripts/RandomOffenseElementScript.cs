using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.MonsterScripts
{
    public class RandomOffenseElementScript : ConfigurableMonsterScriptBase
    {
        protected Element[] Elements { get; init; } = Array.Empty<Element>();

        /// <inheritdoc />
        public RandomOffenseElementScript(Monster subject)
            : base(subject)
        {
            var element = Elements.PickRandom();

            Subject.StatSheet.SetOffenseElement(element);
        }
    }
}
