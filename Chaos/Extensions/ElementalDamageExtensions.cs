using Chaos.Common.Definitions;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Chaos.Extensions
{
    public static class ElementalDamageExtensions
    {
        public static int ConvertElementalAttackDamage(Element offense, Element defense, int damage)
        {
            if (offense is Element.Fire)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 0.32);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 1.74);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 1.03);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 0.66);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 1.88);
            }
            if (offense is Element.Wind)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 0.66);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 0.58);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 1.74);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 1.03);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 0.50);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 0.83);
            }
            if (offense is Element.Earth)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 1.03);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 0.66);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 0.58);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 1.74);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 0.50);
            }
            if (offense is Element.Water)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 1.74);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 1.03);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 0.66);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 0.58);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 1.88);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 0.83);
            }
            if (offense is Element.Holy)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 0.76);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 0.76);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 0.76);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 0.76);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.58);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 1.48);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 0.76);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 0.76);
            }
            if (offense is Element.Darkness)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 1.25);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 1.25);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 1.25);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 1.25);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 1.48);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.58);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 1.25);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 1.25);
            }
            if (offense is Element.Metal)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 1.88);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 0.50);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 0.58);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 1.25);
            }
            if (offense is Element.Undead)
            {
                if (defense is Element.None)
                    return damage += Convert.ToInt32(damage * 2.32);
                if (defense is Element.Fire)
                    return damage += Convert.ToInt32(damage * 0.50);
                if (defense is Element.Wind)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Earth)
                    return damage += Convert.ToInt32(damage * 1.88);
                if (defense is Element.Water)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Holy)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Darkness)
                    return damage += Convert.ToInt32(damage * 0.93);
                if (defense is Element.Wood)
                    return damage += Convert.ToInt32(damage * 0.83);
                if (defense is Element.Metal)
                    return damage += Convert.ToInt32(damage * 1.25);
                if (defense is Element.Undead)
                    return damage += Convert.ToInt32(damage * 0.58);
            }
            return damage;
        }
    }
}
