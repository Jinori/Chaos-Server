using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Behaviors;

public class RestrictionBehavior
{
    public virtual bool CanMove(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.Status.HasFlag(Status.Suain) || aisling.Status.HasFlag(Status.Pramh) || aisling.Status.HasFlag(Status.BeagSuain):
            {
                aisling.SendOrangeBarMessage("You cannot move.");
                return false;   
            }
            case Monster monster when monster.Status.HasFlag(Status.Suain) || monster.Status.HasFlag(Status.Blind) || monster.Status.HasFlag(Status.Pramh) || monster.Status.HasFlag(Status.BeagSuain):
            {
                return false;
            }
        }
        
        return creature.MapInstance.Name.EqualsI("The Afterlife") || creature.IsAlive;
    }

    public virtual bool CanTalk(Creature creature) => true;

    public virtual bool CanTurn(Creature creature)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.Status.HasFlag(Status.Suain) || aisling.Status.HasFlag(Status.Pramh):
            {
                aisling.SendOrangeBarMessage("You cannot turn.");
                return false;   
            }
            case Monster monster when monster.Status.HasFlag(Status.Suain) || monster.Status.HasFlag(Status.Pramh)|| monster.Status.HasFlag(Status.BeagSuain):
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
            if (aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out var timedEvent))
            {
                aisling.SendOrangeBarMessage("You can't do that");
                return false;
            }
            if (!aisling.Status.HasFlag(Status.Suain) || !aisling.Status.HasFlag(Status.Pramh))
                return aisling.IsAlive;
        }

        if (aisling.IsDead && item.Template.TemplateKey.EqualsI("revivePotion"))
        {
            return true;
        }

        aisling.SendOrangeBarMessage("You can't do that");
        return false;
    }


    public virtual bool CanUseSkill(Creature creature, Skill skill)
    {
        
        switch (creature)
        {
            case Aisling aisling when aisling.Status.HasFlag(Status.Suain) || aisling.Status.HasFlag(Status.Pramh) || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out var timedEvent):
            {
                aisling.SendOrangeBarMessage("You cannot use skills.");
                return false;
            }
            case Monster monster when monster.Status.HasFlag(Status.Suain) || monster.Status.HasFlag(Status.Pramh) || monster.Status.HasFlag(Status.BeagSuain):
            {
                return false;
            }
            case Aisling aisling when aisling.Effects.Contains("mount"):
            {
                aisling.Effects.Dispel("mount");

                return true;
            }
        }

        return creature.IsAlive;
    }

    public virtual bool CanUseSpell(Creature creature, Spell spell)
    {
        switch (creature)
        {
            case Aisling aisling when aisling.Status.HasFlag(Status.Suain) || aisling.Status.HasFlag(Status.Pramh) || aisling.Trackers.TimedEvents.HasActiveEvent("Jail", out var timedEvent):
            {
                aisling.SendOrangeBarMessage("You cannot use spells.");
                return false;
            }
            case Monster monster when monster.Status.HasFlag(Status.Suain) || monster.Status.HasFlag(Status.Pramh) || monster.Status.HasFlag(Status.BeagSuain):
            {
                return false;
            }
            case Aisling aisling when aisling.Effects.Contains("mount"):
            {
                aisling.Effects.Dispel("mount");

                return true;
            }
        }
        
        if (creature.IsDead && (spell.Template.Name == "Self Revive"))
            return true;
        
        return creature.IsAlive;
    }
}
