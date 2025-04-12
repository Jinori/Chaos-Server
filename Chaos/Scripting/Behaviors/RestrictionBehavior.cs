#region
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Functional;
#endregion

namespace Chaos.Scripting.Behaviors;

public class RestrictionBehavior
{
    private readonly List<string> MapsGhostsCanMoveOn =
    [
        "Labyrinth Battle Ring",
        "The Afterlife"
    ];

    public virtual bool CanDropItem(Aisling aisling, Item item) => aisling.IsAlive;

    public virtual bool CanDropItemOn(Aisling aisling, Item item, Creature target) => aisling.IsAlive;

    public virtual bool CanDropMoney(Aisling aisling, int amount) => aisling.IsAlive;

    public virtual bool CanDropMoneyOn(Aisling aisling, int amount, Creature target) => aisling.IsAlive;

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
                                      || monster.IsDall
                                      || monster.IsPramhed()
                                      || monster.IsBeagSuained()
                                      || monster.IsRooted():
            {
                return false;
            }
        }

        return MapsGhostsCanMoveOn.Contains(creature.MapInstance.Name) || creature.IsAlive;
    }

    public virtual bool CanPickupItem(Aisling aisling, GroundItem groundItem) => aisling.IsAlive;

    public virtual bool CanPickupMoney(Aisling aisling, Money money) => aisling.IsAlive;

    public virtual bool CanTalk(Creature creature)
    {
        if (creature.Effects.Contains("KnightSilence"))
            return false;

        return true;
    }

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
            if (IsItemUsageRestricted(aisling, item))
            {
                aisling.SendOrangeBarMessage("You can't do that now.");
                return false;
            }

            return true;
        }

        // Special case: allow using revive item while dead
        if (aisling.IsDead && item.Template.TemplateKey.EqualsI("revivePotion"))
            return true;

        aisling.SendOrangeBarMessage("You can't do that now.");
        return false;
    }

    private bool IsItemUsageRestricted(Aisling aisling, Item item)
    {
        // General Restrictions
        if (aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
            || aisling.MapInstance.Script.Is<NoItemUsageScript>())
            return true;

        // Status Restrictions
        if (aisling.IsPramhed() || aisling.IsSuained())
            return true;

        // Stoned: can only use specific item
        if (aisling.IsStoned() && !item.Template.TemplateKey.EqualsI("lightPotion"))
            return true;

        // Map-specific restrictions
        var mapId = aisling.MapInstance.LoadedFromInstanceId;
        if (mapId.StartsWithI("phoenix_sky"))
        {
            aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");
            return true;
        }

        return false;
    }


    public virtual bool CanUseSkill(Creature creature, Skill skill)
    {
        switch (creature)
        {
            case Aisling aisling:
            {
                if (!aisling.IsGodModeEnabled() && IsSkillUsageRestricted(aisling))
                {
                    aisling.SendOrangeBarMessage("You cannot use skills.");
                    return false;
                }

                if (!aisling.IsAdmin && aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"))
                {
                    aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");
                    return false;
                }
                
                if (aisling.Effects.Contains("Mount"))
                {
                    aisling.Effects.Dispel("Mount");
                    return true;
                }

                if (aisling.Effects.Contains("Rumination"))
                {
                    aisling.Effects.Dispel("Rumination");
                    aisling.SendOrangeBarMessage("You ended your rumination.");
                    return true;
                }

                break;
            }
            
            case Monster monster when IsSkillUsageRestricted(monster):
                return false;
        }

        return creature.IsAlive;
    }
    
    private bool IsSkillUsageRestricted(Creature creature) =>
        creature.IsSuained()
        || creature.IsPramhed()
        || creature.IsStoned()
        || creature.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
        || creature.MapInstance.Script.Is<NoSkillSpellUsageScript>()
        || creature.MapInstance.Script.Is<NoSkillUsageScript>();

    public virtual bool CanUseSpell(Creature creature, Spell spell)
    {
        if (IsSpellAllowedWhileRestricted(creature, spell))
            return true;

        if (creature is Aisling aisling)
        {
            if (!aisling.IsGodModeEnabled() && IsSpellUsageRestricted(aisling))
            {
                aisling.SendOrangeBarMessage("You cannot use spells.");
                return false;
            }

            if (!aisling.IsAdmin && aisling.MapInstance.LoadedFromInstanceId.StartsWithI("phoenix_sky"))
            {
                aisling.SendOrangeBarMessage("You are in Lady Phoenix's clutches");
                return false;
            }

            if (aisling.Effects.Contains("Mount"))
            {
                aisling.Effects.Dispel("Mount");
                aisling.Refresh();
                return true;
            }

            if (aisling.Effects.Contains("Rumination"))
            {
                aisling.Effects.Dispel("Rumination");
                aisling.SendOrangeBarMessage("You ended your Rumination.");
                return true;
            }
        }

        if (creature is Monster monster && IsSpellUsageRestricted(monster))
            return false;

        if (creature.IsDead && (spell.Template.Name == "Self Revive"))
            return true;

        return creature.IsAlive;
    }

    private bool IsSpellAllowedWhileRestricted(Creature creature, Spell spell)
    {
        var name = spell.Template.Name.ToLowerInvariant();

        return (creature.IsSuained() && name is "ao suain" or "cure ailments")
               || (creature.IsPramhed() && (name == "dinarcoli"));
    }

    private bool IsSpellUsageRestricted(Aisling aisling) =>
        aisling.IsSuained()
        || aisling.IsPramhed()
        || aisling.IsStoned()
        || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out _)
        || aisling.MapInstance.Script.Is<NoSkillSpellUsageScript>()
        || aisling.MapInstance.Script.Is<NoSpellUsageScript>();

    private bool IsSpellUsageRestricted(Monster monster) =>
        monster.IsSuained()
        || monster.IsPramhed();
}