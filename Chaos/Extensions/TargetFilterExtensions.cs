using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;

namespace Chaos.Extensions;

public static class TargetFilterExtensions
{
    private static bool InnerIsValidTarget(this TargetFilter filter, Creature source, Creature target)
        => filter switch
        {
            TargetFilter.None => true,
            TargetFilter.FriendlyOnly => source.IsFriendlyTo(target),
            TargetFilter.HostileOnly => source.IsHostileTo(target),
            TargetFilter.NeutralOnly => !source.IsFriendlyTo(target) && !source.IsHostileTo(target),
            TargetFilter.NonFriendlyOnly => !source.IsFriendlyTo(target),
            TargetFilter.NonHostileOnly => !source.IsHostileTo(target),
            TargetFilter.NonNeutralOnly => source.IsFriendlyTo(target) || source.IsHostileTo(target),
            TargetFilter.AliveOnly => target.IsAlive,
            TargetFilter.DeadOnly => target.IsDead,
            TargetFilter.AislingsOnly => target is Aisling,
            TargetFilter.MonstersOnly => target is Monster,
            TargetFilter.MerchantsOnly => target is Merchant,
            TargetFilter.NonAislingsOnly => target is not Aisling,
            TargetFilter.NonMonstersOnly => target is not Monster,
            TargetFilter.NonMerchantsOnly => target is not Merchant,
            TargetFilter.SelfOnly => source.Equals(target),
            TargetFilter.OthersOnly => !source.Equals(target),
            TargetFilter.GroupOnly => GroupOnlyFilter(source, target),
            TargetFilter.PetOnly => target is Monster monster && target.Script.Is<PetScript>() && Equals(monster.PetOwner, source),
            TargetFilter.PetOwnerOnly => source is Monster monster && monster.Script.Is<PetScript>() && Equals(monster.PetOwner, target),
            TargetFilter.PetOwnerGroupOnly => source is Monster monster
                                              && monster.Script.Is<PetScript>()
                                              && target is Aisling aisling
                                              && monster.PetOwner?.Group is not null
                                              && (aisling.Group == monster.PetOwner.Group),
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
        };

    private static bool GroupOnlyFilter(Creature source, Creature target)
    {
        if (source.Equals(target))
            return true;
        
        if (source is not Aisling aisling)
            return false;

        if (aisling.Group is null)
        {
            if (IsPet(target, out var pet))
                return pet.PetOwner!.Equals(aisling);
        }
        else
        {
            if (IsPet(target, out var pet))
                return pet.PetOwner!.Group == aisling.Group;
            
            return aisling.Group.Contains(target, WorldEntity.IdComparer);
        }

        return false;
    }
    
    private static bool IsPet(this Creature c, [MaybeNullWhen(false)] out Monster pet)
    {
        pet = null;
        
        if (c is Monster monster && monster.Script.Is<PetScript>() && monster.PetOwner is not null)
        {
            pet = monster;

            return true;
        }

        return false;
    }

    //iterate over all flags present in the enum value
    //if they all result in true, this is a valid target
    public static bool IsValidTarget(this TargetFilter filter, Creature source, Creature target)
        => (filter == TargetFilter.None)
           || filter.GetFlags()
                    .All(flag => InnerIsValidTarget(flag, source, target));
}