using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Behaviors;

public class RestrictionBehavior
{
    private readonly List<string> MapsGhostsCanMoveOn =
    [
        "Arena Battle Ring",
        "The Afterlife"
    ];

    public virtual bool CanMove(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.IsSuained() || aisling.IsBeagSuained() || aisling.IsPramhed() || aisling.IsRooted():
            {
                aisling.SendOrangeBarMessage("You cannot move.");
                return false;
            }
            case Aisling { OnTwentyOneTile: true } aislingCasino:
            {
                aislingCasino.SendOrangeBarMessage("You cannot move when at a game table.");

                return false;
            }
            case Monster monster when monster.IsSuained()
                                      || monster.IsBlind
                                      || monster.IsPramhed()
                                      || monster.IsBeagSuained()
                                      || monster.IsRooted():
            {
                return false;
            }
        }

        return MapsGhostsCanMoveOn.Contains(creature.MapInstance.Name) || creature.IsAlive;
    }

    public virtual bool CanTalk(Creature creature) => true;

    public virtual bool CanTurn(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.IsSuained() || aisling.IsPramhed() || aisling.IsRooted():
            {
                aisling.SendOrangeBarMessage("You cannot turn.");

                return false;
            }
            case Aisling { OnTwentyOneTile: true }:
            {
                return false;
            }
            case Monster monster when monster.IsSuained() || monster.IsPramhed() || monster.IsBeagSuained() || monster.IsRooted():
            {
                return false;
            }
        }

        return true;
    }

    public virtual bool CanUseItem(Aisling aisling, Item item)
    {
        if (aisling.IsAlive)
        {
            if (aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _))
            {
                aisling.SendOrangeBarMessage("You can't do that now.");

                return false;
            }

            if (aisling.IsPramhed() || aisling.IsSuained())
            {
                aisling.SendOrangeBarMessage("You can't do that now.");

                return false;
            }

            return true;
        }

        if (aisling.IsDead && item.Template.TemplateKey.EqualsI("revivePotion"))
            return true;

        aisling.SendOrangeBarMessage("You can't do that now.");

        return false;
    }

    public virtual bool CanUseSkill(Creature creature, Skill skill)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.IsSuained()
                                      || aisling.IsPramhed()
                                      || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _):
            {
                aisling.SendOrangeBarMessage("You cannot use skills.");

                return false;
            }
            case Monster monster when monster.IsSuained()
                                      || monster.IsPramhed()
                                      || monster.IsBeagSuained():
            {
                return false;
            }
            case Aisling aisling when aisling.Effects.Contains("mount"):
            {
                aisling.Effects.Dispel("mount");

                return true;
            }
            case Aisling aisling when aisling.Effects.Contains("rumination"):
            {
                aisling.Effects.Dispel("rumination");
                aisling.SendOrangeBarMessage("You ended your rumination.");

                return true;
            }
        }

        return creature.IsAlive;
    }

    public virtual bool CanUseSpell(Creature creature, Spell spell)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.IsSuained() && (spell.Template.Name == "ao suain"):
            {
                return true;
            }
            case Aisling aisling when aisling.IsPramhed() && (spell.Template.Name == "dinarcoli"):
            {
                return true;
            }

            case Aisling aisling when aisling.IsSuained()
                                      || aisling.IsPramhed()
                                      || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _):
            {
                aisling.SendOrangeBarMessage("You cannot use spells.");

                return false;
            }
            case Monster monster when monster.IsSuained()
                                      || monster.IsPramhed()
                                      || monster.IsBeagSuained():
            {
                return false;
            }
            case Aisling aisling when aisling.Effects.Contains("mount"):
            {
                aisling.Effects.Dispel("mount");
                aisling.Refresh();

                return true;
            }
            case Aisling aisling when aisling.Effects.Contains("rumination"):
            {
                aisling.Effects.Dispel("rumination");
                aisling.SendOrangeBarMessage("You ended your rumination.");

                return true;
            }
        }

        if (creature.IsDead && (spell.Template.Name == "Self Revive"))
            return true;

        return creature.IsAlive;
    }
}