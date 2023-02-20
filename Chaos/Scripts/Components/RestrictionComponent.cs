using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripts.Components;

public class RestrictionComponent
{
    public virtual bool CanMove(Creature creature)
    {
        var aisling = creature as Aisling;
        
        if (creature.Status.HasFlag(Status.Suain))
        {
            aisling?.SendOrangeBarMessage("You are frozen.");
            return false;
        }

        if (creature.Status.HasFlag(Status.Pramh))
        {
            aisling?.SendOrangeBarMessage("You are asleep.");
            return false;
        }

        if (creature.Status.HasFlag(Status.BeagSuain))
        {
            aisling?.SendOrangeBarMessage("You are stunned.");
            return false;
        }

        return creature.IsAlive;
    }

    public virtual bool CanTalk(Creature creature) => true;

    public virtual bool CanTurn(Creature creature)
    {
        var aisling = creature as Aisling;
        
        if (creature.Status.HasFlag(Status.Suain))
        {
            aisling?.SendOrangeBarMessage("You are frozen.");
            return false;
        }

        if (creature.Status.HasFlag(Status.Pramh))
        {
            aisling?.SendOrangeBarMessage("You are asleep.");
            return false;
        }

        return true;
    }

    public virtual bool CanUseItem(Aisling aisling, Item item)
    {
        if (!aisling.IsAlive)
        {
            aisling.SendOrangeBarMessage("You can't do that");
            return false;
        }

        if (aisling.Status.HasFlag(Status.Suain))
        {
            aisling.SendOrangeBarMessage("You are frozen.");
            return false;
        }

        if (aisling.Status.HasFlag(Status.Pramh))
        {
            aisling.SendOrangeBarMessage("You are asleep.");
            return false;
        }

        return aisling.IsAlive;
    }

    public virtual bool CanUseSkill(Creature creature, Skill skill)
    {
        var aisling = creature as Aisling;
        
        if (creature.Status.HasFlag(Status.Suain))
        {
            aisling?.SendOrangeBarMessage("You are frozen.");
            return false;
        }

        if (creature.Status.HasFlag(Status.Pramh))
        {
            aisling?.SendOrangeBarMessage("You are asleep.");
            return false;
        }

        return creature.IsAlive;
    }

    public virtual bool CanUseSpell(Creature creature, Spell spell)
    {
        var aisling = creature as Aisling;
        
        if (creature.Status.HasFlag(Status.Suain))
        {
            aisling?.SendOrangeBarMessage("You are frozen.");
            return false;
        }

        if (creature.Status.HasFlag(Status.Pramh) && spell.Template.Name != "dinarcoli")
        {
            aisling?.SendOrangeBarMessage("You are asleep.");
            return false;
        }

        return creature.IsAlive;
    }
}