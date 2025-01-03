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

            case Aisling { IsAdmin: true } aisling when aisling.Effects.Contains("Follow"):
            {
                // Terminate the follow effect if God Mode is enabled
                aisling.Effects.Terminate("Follow");
                aisling.SendOrangeBarMessage("Follow effect has been terminated because you moved.");
            }

                break;

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
            case Aisling aisling when aisling.IsSuained() || aisling.IsPramhed():
            {
                aisling.SendOrangeBarMessage("You cannot turn.");

                return false;
            }
            case Aisling { OnTwentyOneTile: true }:
            {
                return false;
            }
            case Monster monster when monster.IsSuained() || monster.IsPramhed() || monster.IsBeagSuained():
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
                                      || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
                                      || (aisling.MapInstance.Name.EqualsI("Frosty's Challenge") && !aisling.IsGodModeEnabled()):
            {
                aisling.SendOrangeBarMessage("You cannot use skills.");

                return false;
            }
            case Monster monster when monster.IsSuained() || monster.IsPramhed() || monster.IsBeagSuained():
            {
                return false;
            }
            case Aisling aisling when aisling.Effects.Contains("Mount"):
            {
                aisling.Effects.Dispel("Mount");

                return true;
            }
            case Aisling aisling when aisling.Effects.Contains("Rumination"):
            {
                aisling.Effects.Dispel("Rumination");
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
            case not null when creature.IsSuained() && (spell.Template.Name == "ao suain"):
            {
                return true;
            }
            case not null when creature.IsSuained() && (spell.Template.Name == "Cure Ailments"):
            {
                return true;
            }
            case not null when creature.IsPramhed() && (spell.Template.Name == "dinarcoli"):
            {
                return true;
            }

            case Aisling aisling when aisling.IsSuained()
                                      || aisling.IsPramhed()
                                      || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
                                      || (aisling.MapInstance.Name.EqualsI("Frosty's Challenge") && !aisling.IsGodModeEnabled()):
            {
                aisling.SendOrangeBarMessage("You cannot use spells.");

                return false;
            }
            case Monster monster when monster.IsSuained() || monster.IsPramhed():
            {
                return false;
            }
            case Aisling aisling when aisling.Effects.Contains("Mount"):
            {
                aisling.Effects.Dispel("Mount");
                aisling.Refresh();

                return true;
            }
            case Aisling aisling when aisling.Effects.Contains("Rumination"):
            {
                aisling.Effects.Dispel("Rumination");
                aisling.SendOrangeBarMessage("You ended your Rumination.");

                return true;
            }
        }

        if (creature.IsDead && (spell.Template.Name == "Self Revive"))
            return true;

        return creature.IsAlive;
    }
}