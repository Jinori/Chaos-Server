#region
using Chaos.Collections;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class UserOptionsMapperProfile : IMapperProfile<UserOptions, UserOptionsSchema>
{
    public UserOptions Map(UserOptionsSchema obj)
        => new()
        {
            ShowBodyAnimations = obj.ShowBodyAnimations,
            ListenToHitSounds = obj.ListenToHitSounds,
            PriorityAnimations = obj.PriorityAnimations,
            LockHands = obj.LockHands,
            WhisperSound = obj.Option5,
            AllowExchange = obj.AllowExchange,

            //option 7 not used
            Option8 = obj.Option8,

            //other options
            AllowGroup = obj.AllowGroup
        };

    public UserOptionsSchema Map(UserOptions obj)
        => new()
        {
            ShowBodyAnimations = obj.ShowBodyAnimations,
            ListenToHitSounds = obj.ListenToHitSounds,
            PriorityAnimations = obj.PriorityAnimations,
            LockHands = obj.LockHands,
            Option5 = obj.WhisperSound,
            AllowExchange = obj.AllowExchange,

            //option 7 not used
            Option8 = obj.Option8,

            //other options
            AllowGroup = obj.AllowGroup
        };
}