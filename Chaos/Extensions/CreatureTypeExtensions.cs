using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Extensions;

public static class CreatureTypeExtensions
{
    public static bool WillCollideWith(this CreatureType type, CreatureType otherType)
        => type switch
        {
            CreatureType.Normal      => true,
            CreatureType.WalkThrough => otherType is not CreatureType.Aisling,
            CreatureType.Merchant    => true,
            CreatureType.WhiteSquare => false,
            CreatureType.Aisling     => otherType is not CreatureType.WalkThrough,
            _                        => throw new ArgumentOutOfRangeException()
        };

    public static bool WillCollideWith(this CreatureType type, Creature creature)
    {
        if (creature is Aisling { IsAdmin: true, Visibility: VisibilityType.GmHidden })
            return false;

        if (creature.Type == CreatureType.WhiteSquare && creature.Name == "Crazed Reindeer" || creature.Name == "Smiley Blob Bomb")
            return false;
        
        return type.WillCollideWith(creature.Type);
    }
}