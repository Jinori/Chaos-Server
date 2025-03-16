#region
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Scripting.Behaviors;

public class RestrictionBehavior
{
    public virtual bool CanDropItem(Aisling aisling, Item item) => aisling.IsAlive;

    public virtual bool CanDropItemOn(Aisling aisling, Item item, Creature target) => aisling.IsAlive;

    public virtual bool CanDropMoney(Aisling aisling, int amount) => aisling.IsAlive;

    public virtual bool CanDropMoneyOn(Aisling aisling, int amount, Creature target) => aisling.IsAlive;
    private readonly List<string> MapsGhostsCanMoveOn =
    [
        "Labyrinth Battle Ring",
        "The Afterlife"
    ];

    public virtual bool CanPickupItem(Aisling aisling, GroundItem groundItem) => aisling.IsAlive;

    public virtual bool CanPickupMoney(Aisling aisling, Money money) => aisling.IsAlive;

    public virtual bool CanMove(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.IsSuained()
                                      || aisling.IsBeagSuained()
                                      || aisling.IsPramhed()
                                      || aisling.IsRooted()
                                      || aisling.IsStoned():
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

            case Aisling { IsAdmin: false } aisling when aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"):
            {
                aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");

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
            case Aisling aisling when aisling.IsSuained() || aisling.IsPramhed() || aisling.IsStoned() || aisling.IsIntimidated():
            {
                aisling.SendOrangeBarMessage("You cannot turn.");

                return false;
            }
            case Aisling { OnTwentyOneTile: true }:
            {
                return false;
            }
            case Aisling { IsAdmin: false } aisling when aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"):
            {
                aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");

                return false;
            }
            case Monster monster when monster.IsSuained() || monster.IsPramhed() || monster.IsBeagSuained() || monster.IsIntimidated():
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

            if (aisling.IsStoned() && !item.Template.TemplateKey.EqualsI("lightPotion"))
                return false;
            
            if (aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"))
            {
                aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");

                return false;
            }
            
            if (aisling.MapInstance.LoadedFromInstanceId.StartsWithI("snaggleschallenge"))
            {
                aisling.SendOrangeBarMessage("You can't use items here.");
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
                                      || aisling.IsStoned()
                                      || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
                                      || aisling.MapInstance.Name.EqualsI("Frosty's Challenge") || (aisling.MapInstance.Name.EqualsI("Snaggles Secret Sweetroom") && !aisling.IsGodModeEnabled()):
            {
                aisling.SendOrangeBarMessage("You cannot use skills.");

                return false;
            }
            case Aisling { IsAdmin: false } aisling when aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"):
            {
                aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");

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
                                      || aisling.IsStoned()
                                      || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
                                      || aisling.MapInstance.Name.EqualsI("Frosty's Challenge") || (aisling.MapInstance.Name.EqualsI("Snaggles Secret Sweetroom") && !aisling.IsGodModeEnabled()):
            {
                aisling.SendOrangeBarMessage("You cannot use spells.");

                return false;
            }
            case Aisling { IsAdmin: false } aisling when aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"):
            {
                aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");

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

        if (creature is { IsDead: true } && (spell.Template.Name == "Self Revive"))
            return true;

        return creature is { IsAlive: true };
    }
}