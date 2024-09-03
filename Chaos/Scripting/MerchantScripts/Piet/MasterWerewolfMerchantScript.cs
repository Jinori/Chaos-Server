using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Piet;

public class MasterWerewolfMerchantScript : MerchantScriptBase
{
    public enum MasterWerewolfState
    {
        Idle,
        Wandering,
        SeenByAislingWithEnum,
    }
    
    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;

    /// <inheritdoc />
    public MasterWerewolfMerchantScript(Merchant subject)
        : base(subject)
    {
        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(3),
            20,
            RandomizationType.Positive,
            false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.MasterWerewolfState = MasterWerewolfState.Idle;
    }
    
    public bool HasSaidGreeting;
    public bool HasSaidDialog1;
    public bool HasSaidDialog2;
    public bool HasSaidDialog3;
    public bool HasSaidDialog4;

    private void HandleIdleState()
    {
        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)))
            Subject.MasterWerewolfState = MasterWerewolfState.SeenByAislingWithEnum;

        Subject.MasterWerewolfState = MasterWerewolfState.Wandering;

        ActionTimer.Reset();
    }

    private void HandleWanderingState()
    {
        Subject.Wander();

        if (Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
            .Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)))
            Subject.MasterWerewolfState = MasterWerewolfState.SeenByAislingWithEnum;

        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            Subject.MasterWerewolfState = MasterWerewolfState.Idle;
            ActionTimer.Reset();
        }

        Subject.WanderTimer.Reset();
    }

    private static string PickRandom(ICollection<string> phrases) => phrases.PickRandom();

    private void HandleWerewolfConversation(TimeSpan delta)
    {
        DialogueTimer.Update(delta);

        if (!DialogueTimer.IntervalElapsed) return;
        
        if (!HasSaidGreeting)
        {
            Subject.Turn(Direction.Up);
            var greeting = string.Format(PickRandom(Greeting));
            Subject.Say(greeting);
            HasSaidGreeting = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog1 && DialogueTimer.IntervalElapsed)
        {
            var dialog1 = PickRandom(Dialog1);
            Subject.Say(dialog1);
            HasSaidDialog1 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog2 && DialogueTimer.IntervalElapsed)
        {
            var greeting = string.Format(PickRandom(Dialog2));
            Subject.Say(greeting);
            HasSaidDialog2 = true;
            DialogueTimer.Reset();
        }
        else if (!HasSaidDialog3 && DialogueTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>()
                .Where(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard) || x.Trackers.Enums.HasValue(WerewolfOfPiet.RetryWerewolf)).ToList();
            foreach (var aisling in aislings)
            {
                aisling.Trackers.Enums.Set(WerewolfOfPiet.SpawnedWerewolf2);
                aisling.SendOrangeBarMessage("The Master Werewolf becomes hostile...");
            }

            HasSaidDialog2 = true;
            DialogueTimer.Reset();
        }
    }


    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        
        switch (Subject.MasterWerewolfState)
        {
            case MasterWerewolfState.Idle:
            {
                HandleIdleState();
                break;
            }

            case MasterWerewolfState.Wandering:
            {
                HandleWanderingState();
                break;
            }

            case MasterWerewolfState.SeenByAislingWithEnum:
            {
                HandleWerewolfConversation(delta);
                break;
            }
        }
    }
    private static readonly List<string> Greeting =
    [
        "You are not a Werewolf, why are you in my woods...",
        "I can smell you coming, you don't belong here.",
        "There is no reason for you to be here...",
        "Are you looking for me... I doubt it."
    ];

    private static readonly List<string> Dialog1 =
    [
        "The Rose of Sharon belongs here, I will not let you take it.",
        "Why do you think you'd just take the flowers that grow here?",
        "There is no way you'd escape with our flowers.",
        "Do you really think you'd make it past me?"
    ];

    private static readonly List<string> Dialog2 =
    [
        "Enough chitchat, you'll expire here now.",
        "Your head will look great on my tree.",
        "I am glad you brought me fresh blood.",
        "No way you'll make it out of here alive."
    ];
}