using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Piet;

public class PietVillagerScript : MerchantScriptBase
{
    public enum PietVillagerState
    {
        Idle,
        Wandering,
        SayRandomMessage,
        WalkingToArmory,
        WalkingToTailor,
        WalkingToRestaurant,
        TalkingToAnotherMerchant,
        TalkingToPlayer,
        FollowingPlayer,
        CalloutPasserby,
        HandleWerewolfConversation,
        WalkToSpawnPoint,
        Eating
    }

    private IPathOptions Options => PathOptions.Default with
    {
        LimitRadius = null,
        IgnoreBlockingReactors = true
    };



    private readonly IIntervalTimer ActionTimer;
    private readonly IIntervalTimer DialogueTimer;
    private readonly IIntervalTimer EatingTimer;
    private readonly IDialogFactory DialogFactory;
    private DateTime LastChant;
    private readonly TimeSpan MaxFollowDuration = TimeSpan.FromSeconds(50);

    public readonly Location PietArmoryDoorInsidePoint = new("piet_armor_shop", 11, 8);
    public readonly Location PietArmoryDoorPoint = new("piet", 55, 22);
    public readonly Location PietArmoryPoint = new("piet_armor_shop", 6, 8);

    public readonly Location PietRestaurantDoorInsidePoint = new("piet_restaurant", 8, 14);
    public readonly Location PietRestaurantDoorPoint = new("piet", 11, 49);
    public readonly Location PietRestaurantPoint = new("piet_restaurant", 8, 7);

    public readonly Location PietStorageDoorInsidePoint = new("piet_storage", 6, 11);
    public readonly Location PietStorageDoorPoint = new("piet", 51, 13);
    public readonly Location PietStoragePoint = new("piet_storage", 5, 8);

    private readonly Location Spawnpoint;
    private readonly IIntervalTimer WalkTimer;
    private DateTime FollowUntil;
    public bool HasAskedForItem;
    public bool HasHadConversation;
    public bool HasMerchantResponded;
    public bool HasMerchantResponded2;
    public bool HasMerchantResponded3;
    public bool HasSaidGoodbye;
    public bool HasPickedAnAisling;
    public bool HasReceivedItem;
    public bool HasSaidGreeting;
    public bool DoWerewolfConversation;
    private string? ItemRequested;
    private Aisling? RandomAisling;

    /// <inheritdoc />
    public PietVillagerScript(Merchant subject, IDialogFactory dialogFactory)
        : base(subject)
    {
        DialogFactory = dialogFactory;
        WalkTimer = new IntervalTimer(TimeSpan.FromMilliseconds(600), false);
        ActionTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
        Subject.PietVillagerState = PietVillagerState.Idle;

        DialogueTimer = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(5),
            50,
            RandomizationType.Positive,
            false);

        EatingTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);

        Spawnpoint = new Location(Subject.MapInstance.InstanceId, Subject.X, Subject.Y);
    }

    private void AttemptToOpenDoor(IPoint doorPoint)
    {
        var door = Subject.MapInstance.GetEntitiesAtPoint<Door>(doorPoint).FirstOrDefault();

        if (door is { Closed: true })
            door.OnClicked(Subject);
    }
    private void HandleEatingState()
    {
        if ((DateTime.Now - LastChant).TotalSeconds >= 2)
        {
            DisplayEatingMessage();
            LastChant = DateTime.Now;
        }

        if (EatingTimer.IntervalElapsed)
        {
            var npc = Subject.MapInstance.GetEntitiesWithinRange<Merchant>(Subject)
                .FirstOrDefault(x => x.Id != Subject.Id);

            var thanks = string.Format(PickRandom(CustomerThanksCook), npc?.Name);
            Subject.Say(thanks);

            HasReceivedItem = true;
            HasHadConversation = true;
            DialogueTimer.Reset();

            EatingTimer.Reset();
            Subject.PietVillagerState = PietVillagerState.WalkingToRestaurant;
        }
    }

    private void DisplayEatingMessage()
    {
        var chant = EatingActions.PickRandom();
        Subject.Chant($"*{chant}*");
    }

    private void ConductWerewolfConversation(Merchant npc)
{
    if (!HasSaidGreeting)
    {
        var greeting = string.Format(PickRandom(StartWereWolfConversation), npc.Name);
        Subject.Say(greeting);
        HasSaidGreeting = true;
        DialogueTimer.Reset();
    }
    else if (!HasMerchantResponded && DialogueTimer.IntervalElapsed)
    {
        var response = PickRandom(WerewolfReply1);
        npc.Say(response);
        HasMerchantResponded = true;
        DialogueTimer.Reset();
    }
    else if (!HasMerchantResponded2 && DialogueTimer.IntervalElapsed)
    {
        var greeting = string.Format(PickRandom(StartWereWolfConversation2), npc.Name);
        Subject.Say(greeting);
        HasMerchantResponded2 = true;
        DialogueTimer.Reset();
    }
    else if (!HasMerchantResponded3 && DialogueTimer.IntervalElapsed)
    {
        var response = PickRandom(WerewolfReply2);
        npc.Say(response);
        HasMerchantResponded3 = true;
        DialogueTimer.Reset();
    }
    else if (!HasSaidGoodbye && DialogueTimer.IntervalElapsed)
    {
        var greeting = string.Format(PickRandom(StartWereWolfConversation3), npc.Name);
        Subject.Say(greeting);
        HasSaidGoodbye = true;
        DialogueTimer.Reset();
    }
    else if (!HasHadConversation && DialogueTimer.IntervalElapsed)
    {
        var response = PickRandom(WerewolfReply3);
        npc.Say(response);

        var hasStage = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
                              .Any(x => x.Trackers.Enums.TryGetValue(out WerewolfOfPiet _));

        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject)
                              .Where(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.None) || !hasStage)
                              .ToList();

        foreach (var aisling in aislings)
        {
            var dialog = DialogFactory.Create("werewolf_initial", npc);
            dialog.Display(aisling);
        }

        HasHadConversation = true;
    }
}

    
    private void ConductDialogueWithMerchant(string merchant)
    {
        if (IntegerRandomizer.RollChance(20) && Subject.MapInstance.InstanceId.ContainsI("Piet") && !DoWerewolfConversation)
            DoWerewolfConversation = true;

        switch (merchant)
        {
            case "restaurant":
            {
                var npc = Subject.MapInstance.GetEntitiesWithinRange<Merchant>(Subject)
                    .FirstOrDefault(x => x.Id != Subject.Id);

                if (DoWerewolfConversation)
                {
                    if (npc != null)
                        ConductWerewolfConversation(npc);
                }
                else
                {
                    if (!HasSaidGreeting)
                    {
                        var greeting = string.Format(PickRandom(Greetings), npc?.Name);
                        Subject.Say(greeting);
                        HasSaidGreeting = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasAskedForItem)
                    {
                        var foodRequest = PickRandom(FoodRequests);
                        Subject.Say(foodRequest);
                        HasAskedForItem = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasMerchantResponded)
                    {
                        var response = PickRandom(ServingFoodResponses);
                        npc?.Say(response);
                        HasMerchantResponded = true;
                        Subject.PietVillagerState = PietVillagerState.Eating;
                        EatingTimer.Reset();
                        LastChant = DateTime.Now;
                    }
                }
                
                break;
            }


            case "tailor":
            {
                var npc = Subject.MapInstance.GetEntitiesWithinRange<Merchant>(Subject)
                    .FirstOrDefault(x => x.Id != Subject.Id);
                
                if (DoWerewolfConversation)
                {
                    if (npc != null)
                        ConductWerewolfConversation(npc);
                }
                else
                {
                    if (!HasSaidGreeting)
                    {
                        var greeting = string.Format(PickRandom(Greetings), npc?.Name);
                        Subject.Say(greeting);
                        HasSaidGreeting = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasAskedForItem)
                    {
                        var color = GetRandomDisplayColor();
                        var purchaseRequest = string.Format(PickRandom(AskAboutDye), color);
                        ItemRequested = color.ToString();
                        Subject.Say(purchaseRequest);
                        HasAskedForItem = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasMerchantResponded)
                    {
                        var purchaseRequest = string.Format(PickRandom(TailorResponses), ItemRequested);
                        npc?.Say(purchaseRequest);
                        HasMerchantResponded = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasReceivedItem)
                    {
                        var response = PickRandom(ItemReceivedResponses);
                        Subject.Say(response);
                        HasReceivedItem = true;
                        HasHadConversation = true;
                    }
                }

                break;
            }
            case "armorer":
            {
                var npc = Subject.MapInstance.GetEntitiesWithinRange<Merchant>(Subject)
                    .FirstOrDefault(x => x.Id != Subject.Id);
                
                if (DoWerewolfConversation)
                {
                    if (npc != null)
                        ConductWerewolfConversation(npc);
                }
                else
                {
                    if (!HasSaidGreeting)
                    {
                        var greeting = string.Format(PickRandom(Greetings), npc?.Name);
                        Subject.Say(greeting);
                        HasSaidGreeting = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasAskedForItem)
                    {
                        var randomItem = npc?.ItemsForSale.PickRandom();
                        var purchaseRequest = string.Format(PickRandom(PurchaseRequests), randomItem?.DisplayName);
                        ItemRequested = randomItem?.DisplayName;
                        Subject.Say(purchaseRequest);
                        HasAskedForItem = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasMerchantResponded)
                    {
                        var purchaseRequest = string.Format(PickRandom(MerchantResponses), ItemRequested);
                        npc?.Say(purchaseRequest);
                        HasMerchantResponded = true;
                        DialogueTimer.Reset();
                    }
                    else if (!HasReceivedItem)
                    {
                        var response = PickRandom(ItemReceivedResponses);
                        Subject.Say(response);
                        HasReceivedItem = true;
                        HasHadConversation = true;
                    }   
                }

                break;
            }
        }
    }

    public static DisplayColor GetRandomDisplayColor()
    {
        var value = Enum.GetValues<DisplayColor>().ToList();
        var random = value.PickRandom();

        return random;
    }



    private string GetRandomMessage()
    {
        var random = VillagerMessages.PickRandom();
        return random;
    }

    private void HandleApproachToArmory(TimeSpan delta)
    {
            if (ShouldWalkTo(PietArmoryDoorPoint))
                WalkTowards(PietArmoryDoorPoint, delta);
            else if (ShouldWalkTo(PietArmoryPoint))
                WalkTowards(PietArmoryPoint, delta);

    }

    private void HandleApproachToTailor(TimeSpan delta)
    {
            if (ShouldWalkTo(PietStorageDoorPoint))
                WalkTowards(PietStorageDoorPoint, delta);
            else if (ShouldWalkTo(PietStoragePoint))
                WalkTowards(PietStoragePoint, delta);
    }

    private void HandleApproachToRestaurant(TimeSpan delta)
    {
            if (ShouldWalkTo(PietRestaurantDoorPoint))
                WalkTowards(PietRestaurantDoorPoint, delta);
            else if (ShouldWalkTo(PietRestaurantPoint))
                WalkTowards(PietRestaurantPoint, delta);
    }

    private void HandleConversationWithMerchant(TimeSpan delta)
    {
            if (IsCloseTo(PietArmoryPoint))
                UpdateDialogue(delta, "armorer");

            if (IsCloseTo(PietStoragePoint))
                UpdateDialogue(delta, "tailor");

            if (IsCloseTo(PietRestaurantPoint))
                UpdateDialogue(delta, "restaurant");
    }

    private void HandleFollowingPlayer(TimeSpan delta)
    {
        if (!HasPickedAnAisling)
        {
            var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject).ToList();

            if (aislings.Count > 0)
            {
                RandomAisling = aislings.Count == 1 ? aislings[0] : aislings.PickRandom();
                FollowUntil =
                    DateTime.Now.AddSeconds(IntegerRandomizer.RollSingle((int)MaxFollowDuration.TotalSeconds));
                HasPickedAnAisling = true;
            }
        }

        if (HasPickedAnAisling && (DateTime.Now < FollowUntil))
        {
            if (RandomAisling != null)
            {
                var point = new Point(RandomAisling.X, RandomAisling.Y);
                var location = new Location(Subject.MapInstance.InstanceId, point);

                if (ShouldWalkTo(location))
                    WalkTowardsPlayer(location, delta);
                else
                {
                    if (ShouldWalkToSpawnPoint())
                        WalkTowards(Spawnpoint, delta);
                    else
                        ResetConversationState();
                }
            }
        }
        else
        {
            if (ShouldWalkToSpawnPoint())
                WalkTowards(Spawnpoint, delta);
            else
                ResetConversationState();
        }
    }

    private static readonly List<KeyValuePair<PietVillagerState, decimal>> StateData =
    [
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.Wandering, 40),
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.SayRandomMessage, 20),
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.WalkingToRestaurant, 10),
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.WalkingToTailor, 10),
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.WalkingToArmory, 10),
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.FollowingPlayer, 5),
        new KeyValuePair<PietVillagerState, decimal>(PietVillagerState.CalloutPasserby, 5),
    ];

    private void HandleIdleState()
    {
        var state = StateData.PickRandomWeightedSingleOrDefault();
        Subject.PietVillagerState = state;

        ActionTimer.Reset();
    }


    private void HandlePostConversationActions(TimeSpan delta, string merchant)
    {
        switch (merchant)
        {
            case "armorer":
            {
                    if (ShouldWalkTo(PietArmoryDoorInsidePoint))
                        WalkTowards(PietArmoryDoorInsidePoint, delta);
                    else if (ShouldWalkToSpawnPoint())
                        WalkTowards(Spawnpoint, delta);
                    else
                        ResetConversationState();
                    break;
            }
            case "tailor":
            {
                    if (ShouldWalkTo(PietStorageDoorInsidePoint))
                        WalkTowards(PietStorageDoorInsidePoint, delta);
                    else if (ShouldWalkToSpawnPoint())
                        WalkTowards(Spawnpoint, delta);
                    else
                        ResetConversationState();
                    break;
            }
            case "restaurant":
            {
                    if (ShouldWalkTo(PietRestaurantDoorInsidePoint))
                        WalkTowards(PietRestaurantDoorInsidePoint, delta);
                    else if (ShouldWalkToSpawnPoint())
                        WalkTowards(Spawnpoint, delta);
                    else
                        ResetConversationState();

                    break;
            }
        }
    }

    private void HandleWalkingToArmory(TimeSpan delta)
    {
        if (!HasHadConversation)
        {
            HandleApproachToArmory(delta);
            HandleConversationWithMerchant(delta);
        }
        else
            HandlePostConversationActions(delta, "armorer");
    }

    private void HandleWalkingToTailor(TimeSpan delta)
    {
        if (!HasHadConversation)
        {
            HandleApproachToTailor(delta);
            HandleConversationWithMerchant(delta);
        }
        else
            HandlePostConversationActions(delta, "tailor");
    }

    private void HandleWalkingToRestaurant(TimeSpan delta)
    {
        if (!HasHadConversation)
        {
            HandleApproachToRestaurant(delta);
            HandleConversationWithMerchant(delta);
        }
        else
            HandlePostConversationActions(delta, "restaurant");
    }

    private void HandleWanderingState()
    {
        Subject.Wander();

        if (Subject.WanderTimer.IntervalElapsed && IntegerRandomizer.RollChance(10))
        {
            Subject.PietVillagerState = PietVillagerState.Idle;
            ActionTimer.Reset();
        }

        Subject.WanderTimer.Reset();
    }

    private void HandleCalloutPasserby(TimeSpan delta)
    {
        if (!HasPickedAnAisling)
        {
            var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject).ToList();

            if (aislings.Count > 0)
            {
                RandomAisling = aislings.PickRandom();
                HasPickedAnAisling = true;
            }
        }

        if (HasPickedAnAisling && !HasHadConversation)
        {
            var legendmark = RandomAisling?.Legend.GetRandomMark();

            if (legendmark != null)
            {
                if (!LegendMarkResponses.TryGetValue(legendmark, out var response))
                {
                    HasHadConversation = true;
                }

                if (response != null)
                    Subject.Say(response);

                HasHadConversation = true;
            }
        }
        else
        {
            if (ShouldWalkToSpawnPoint())
                WalkTowards(Spawnpoint, delta);
            else
                ResetConversationState();
        }
    }

    private bool IsCloseTo(Location point) => Subject.WithinRange(point, 2);

    private static string PickRandom(ICollection<string> phrases) => phrases.PickRandom();

    private void ResetConversationState()
    {
        HasPickedAnAisling = false;
        HasHadConversation = false;
        HasReceivedItem = false;
        HasSaidGreeting = false;
        HasAskedForItem = false;
        HasMerchantResponded = false;
        HasMerchantResponded2 = false;
        HasMerchantResponded3 = false;
        HasSaidGoodbye = false;
        ItemRequested = null;
        RandomAisling = null;
        FollowUntil = DateTime.MinValue;
        Subject.PietVillagerState = PietVillagerState.Wandering;
    }

    private void HandleSayRandomMessageState()
    {
        var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 8);

        if (aislings.Any())
            Subject.Say(GetRandomMessage());

        Subject.PietVillagerState = PietVillagerState.Idle;
    }

    private bool ShouldWalkTo(Location destination) =>
        (Subject.DistanceFrom(destination) > 0) && Subject.OnSameMapAs(destination);
    private bool ShouldWalkToSpawnPoint() => Subject.DistanceFrom(Spawnpoint) > 0;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        ActionTimer.Update(delta);
        EatingTimer.Update(delta);

        switch (Subject.PietVillagerState)
        {
            case PietVillagerState.Idle:
                if (ActionTimer.IntervalElapsed)
                    HandleIdleState();

                break;

            case PietVillagerState.SayRandomMessage:
                HandleSayRandomMessageState();
                break;

            case PietVillagerState.Wandering:
                if (Subject.WanderTimer.IntervalElapsed)
                    HandleWanderingState();

                break;

            case PietVillagerState.WalkingToArmory:
                HandleWalkingToArmory(delta);

                break;
            case PietVillagerState.WalkingToTailor:
                HandleWalkingToTailor(delta);

                break;


            case PietVillagerState.WalkingToRestaurant:
                HandleWalkingToRestaurant(delta);

                break;

            case PietVillagerState.CalloutPasserby:
                HandleCalloutPasserby(delta);

                break;

            case PietVillagerState.TalkingToAnotherMerchant:
                break;
            case PietVillagerState.TalkingToPlayer:
                break;

            case PietVillagerState.FollowingPlayer:
                HandleFollowingPlayer(delta);

                break;

            case PietVillagerState.Eating:
                HandleEatingState();
                break;
        }
    }

    private void UpdateDialogue(TimeSpan delta, string merchant)
    {
        DialogueTimer.Update(delta);

        if (DialogueTimer.IntervalElapsed)
            ConductDialogueWithMerchant(merchant);
    }

    private void UpdateWalkTimer(TimeSpan delta) => WalkTimer.Update(delta);

    private void WalkTowards(Location destination, TimeSpan delta)
    {
        UpdateWalkTimer(delta);

        if (WalkTimer.IntervalElapsed)
        {
                if (destination == PietArmoryDoorPoint)
                    AttemptToOpenDoor(PietArmoryDoorPoint);

                if (destination == PietStorageDoorPoint)
                    AttemptToOpenDoor(PietStorageDoorPoint);

                if (destination == PietRestaurantDoorPoint)
                    AttemptToOpenDoor(PietRestaurantDoorPoint);

                Subject.Pathfind(destination, 0, Options);
        }
    }

    private void WalkTowardsPlayer(Location destination, TimeSpan delta)
    {
        UpdateWalkTimer(delta);

        if (WalkTimer.IntervalElapsed)
            Subject.Pathfind(destination, 0);
    }

    #region TrackedLegendMarks

    private readonly Dictionary<string, string> LegendMarkResponses = new()
    {
        { "Arena Winner", "I hear the arena is a scary place" },
        { "Arena Participant", "I hear the arena is a scary place" },
        { "Loved by Mileth Mundanes", "Wow, you're popular around Mileth" },
        { "Loved by Abel Mundanes", "Pretty helpful around Abel I see." },
        { "Inspired Fisk to Reach for the Stars", "Thanks for helping Fisk!" },
        { "Helped the Eingren Manor's Plumber", "You've dispatched a lot of ghosts!" },
        { "Cured the Sick Child of Loures", "Thanks for helping that girl in Loures." },
        { "Successfully conquered their Nightmares", "I had a bad nightmare once too." }
    };

    #endregion TrackedLegendMarks

    #region Messages

    private static readonly List<string> EatingActions =
    [
        "big bite",
        "gobbles",
        "quiet eat",
        "chews",
        "savors",
        "feasts",
        "devours",
        "nibbles",
        "munches",
        "tastes",
        "relishes",
        "partakes",
        "hearty",
        "fills up",
        "indulges",
        "snacks",
        "quick bite",
        "eats",
        "fast eat",
        "snack",
        "small bite",
        "king feast",
        "relish",
        "tucks in",
        "continues",
        "bite",
        "focuses",
        "dines"
    ];

    private static readonly List<string> TailorResponses =
    [
        "Ah, {0} dye? We have that in stock.",
        "{0} dye is a popular choice, here you go.",
        "We do have {0} dye. How much do you need?",
        "Yes, I can get you some {0} dye.",
        "Of course, {0} dye is right this way.",
        "{0} dye? A fine choice, I have it here.",
        "Absolutely, we've got plenty of {0} dye.",
        "{0} dye, a classic. I have some available.",
        "I can certainly provide {0} dye for you.",
        "Looking for {0} dye? I have just the thing.",
        "Yes, {0} dye is available. How many?",
        "I have excellent {0} dye, very high quality.",
        "{0} dye? You're in luck, I have it.",
        "Certainly, {0} dye is one of our specialties.",
        "Got a good stock of {0} dye right now.",
        "You want {0} dye? I can help with that.",
        "{0} dye? Yes, I can provide that.",
        "Indeed, we have {0} dye. Great for garments.",
        "{0} dye? Just got a new shipment in.",
        "Sure, {0} dye is on hand. How much?",
        "I have the perfect shade of {0} dye.",
        "{0} dye, coming right up!",
        "We're well-stocked in {0} dye today.",
        "{0} dye? Of course, let me show you.",
        "Good timing, I have plenty of {0} dye.",
        "{0} dye is here. It's very sought after.",
        "Certainly, {0} dye is one of our best.",
        "You've chosen well with {0} dye, here it is.",
        "I can absolutely help you with {0} dye.",
        "{0} dye is ready for you. How much?"
    ];

    private static readonly List<string> AskAboutDye =
    [
        "Do you have {0} dye in stock?",
        "Is {0} dye available here?",
        "Can I find {0} dye at your shop?",
        "I'm looking for {0} dye. Got any?",
        "Any chance you've got {0} dye?",
        "I need some {0} dye, do you have it?",
        "I'm in need of {0} dye, can you help?",
        "I'd like to buy {0} dye, do you sell it?",
        "Looking for {0} dye, do you carry it?",
        "Is there any {0} dye on your shelves?",
        "Hoping to find {0} dye, have you got some?",
        "Could I get my hands on some {0} dye here?",
        "Do you stock {0} dye by any chance?",
        "How about {0} dye, is that available?",
        "In search of {0} dye, do you have that?",
        "I'm after {0} dye, is it in stock?",
        "I was wondering if you sell {0} dye?",
        "Do you offer {0} dye in your collection?",
        "Got any {0} dye for sale?",
        "Would you happen to have {0} dye?",
        "I'm on the lookout for {0} dye, got it?",
        "Do you carry {0} dye in this shop?",
        "Any {0} dye to purchase here?",
        "Can I purchase {0} dye from you?",
        "Looking to buy some {0} dye, available?",
        "Is it possible to get {0} dye here?",
        "Could you help me with {0} dye?",
        "I'd like some {0} dye, please.",
        "In need of {0} dye, do you supply it?",
        "Any luck finding {0} dye in your store?"
    ];

    private static readonly List<string> ItemReceivedResponses =
    [
        "Thanks a lot!",
        "Just what I needed, thank you!",
        "Perfect, appreciate it!",
        "Exactly what I wanted!",
        "You read my mind!",
        "Couldn't be happier!",
        "This is fantastic, thanks!",
        "Spot on, thank you!",
        "You're the best!",
        "Marvelous choice!",
        "Superb, just superb!",
        "Top-notch, as always!",
        "You never disappoint!",
        "Great, just great!",
        "This will do nicely!",
        "Well done, thank you!",
        "You've outdone yourself!",
        "Exceptional, truly!",
        "Remarkable service!",
        "A splendid choice!",
        "Excellent, very pleased!",
        "Oh, this is great!",
        "That's really nice!",
        "Amazing, thank you!",
        "Right on the mark!",
        "So pleased with this!",
        "Quality as expected!",
        "You've nailed it!",
        "Such a fine selection!",
        "A masterful choice!"
    ];

    private static readonly List<string> PurchaseRequests =
    [
        "I'd like to buy a {0} please.",
        "Could I get one {0}?",
        "I'm interested in a {0}.",
        "Can I purchase a {0}?",
        "One {0}, if you don't mind.",
        "How about a {0} for me?",
        "May I have a {0}?",
        "I'll take a {0}, please.",
        "Looking to buy a {0}.",
        "Can you spare a {0}?",
        "I need a {0}, please.",
        "A {0} would be great.",
        "I'd love a {0}.",
        "Please, a {0}?",
        "Would you sell a {0}?",
        "I'm in need of a {0}.",
        "A {0}, if possible.",
        "Let's go with a {0}.",
        "Is the {0} available?",
        "Hand me a {0}, please.",
        "I choose the {0}.",
        "I'd prefer a {0}.",
        "I think a {0} suits me.",
        "Any {0} left for sale?",
        "How's the {0} priced?",
        "May I secure a {0}?",
        "I vote for a {0}.",
        "Let's try the {0}.",
        "How much for a {0}?",
        "Got a {0} in stock?"
    ];

    private static readonly List<string> Greetings =
    [
        "Hello {0}, how are you today?",
        "Good day, {0}! How's everything?",
        "Hi {0}, lovely to see you!",
        "Hey {0}, great to meet you!",
        "Greetings, {0}!",
        "Howdy {0}, what's new?",
        "Hello there, {0}!",
        "Ah, {0}! Good to see you.",
        "Hiya {0}! How are you?",
        "Welcome, {0}! How goes it?",
        "Well met, {0}!",
        "Heya {0}, how's life?",
        "Hello {0}, what's up?",
        "Good to see you, {0}!",
        "Pleased to meet you, {0}.",
        "How's it going, {0}?",
        "Nice to see you, {0}.",
        "What's happening, {0}?",
        "Ahoy {0}!",
        "Salutations, {0}!",
        "Hey there, {0}!",
        "Hello {0}, nice to meet you.",
        "Greetings and welcome, {0}!",
        "Hi {0}, what brings you here?",
        "Ello, {0}!",
        "Ola, {0}! How's your day?",
        "How's everything, {0}?",
        "Hi {0}, all well?",
        "Greetings, {0}, how do you do?",
        "Hey {0}, any news?"
    ];

    private static readonly List<string> MerchantResponses =
    [
        "Certainly! One {0} coming right up.",
        "You've got it. I'll get a {0} for you.",
        "Absolutely! A {0} is an excellent choice.",
        "A favorite of mine. I'll fetch one for you.",
        "Brilliant! The {0} is one of our best!",
        "A {0}? Fantastic! Let me grab that.",
        "Oh, a {0}? An interesting selection!",
        "The {0}? That's an unusual choice, but great!",
        "The {0}, huh? It has a story behind it.",
        "Thank you for choosing the {0}.",
        "A {0}, right away!",
        "Good eye! The {0} is perfect.",
        "A {0}? Excellent taste!",
        "Right then, a {0} for you!",
        "You'll love the {0}.",
        "One moment, getting your {0}.",
        "Sure thing, a {0} coming up.",
        "A wise choice, the {0} it is!",
        "Ah, the {0}! Coming up.",
        "Happy to provide a {0}.",
        "One {0}, with pleasure!",
        "The {0}? A superb choice!",
        "I recommend the {0}.",
        "Your {0} is right here.",
        "Preparing the {0} now.",
        "A {0}? You won't regret it!",
        "The {0}, always a good pick.",
        "One {0}, as requested.",
        "Let's get you that {0}.",
        "You'll be pleased with the {0}."
    ];

    private readonly List<string> VillagerMessages =
    [
        "Lovely weather we're having!",
        "Have you visited the blacksmith?",
        "Beware of wolves in the forest.",
        "The harvest is bountiful this year.",
        "Stay clear of the Dark Woods.",
        "Have you seen the new merchant?",
        "Legends speak of hidden treasures.",
        "The old well is said to be cursed.",
        "Travelers bring interesting tales.",
        "Wish I could see the capital city.",
        "Keep your wits about you at night.",
        "Ever heard the howl of the banshee?",
        "Stay a while and listen.",
        "The river seems high this season.",
        "Look out for falling branches!",
        "A mysterious fog covered us last night.",
        "I miss the songs of the minstrels.",
        "Do you believe in forest spirits?",
        "I'd avoid the ancient ruins.",
        "I've heard tales of a secret cave.",
        "My garden is my greatest treasure.",
        "Best ale in town is at the tavern.",
        "The old tower? Avoid it at all costs!",
        "Strange lights were seen near the hill.",
        "A traveling bard visited yesterday.",
        "The wind speaks, if you listen...",
        "Ever tasted Mileth's famous stew?",
        "The mill's been quiet these days.",
        "Those mountains hold dark secrets.",
        "The full moon brings strange events.",
        "My family's lived here for generations.",
        "I once saw a ghost, I swear it!",
        "Sometimes, I hear whispers in the fog.",
        "I wouldn't stray far from the village.",
        "The forest isn't safe for travelers.",
        "Did you come to see the festival?",
        "Our village is small, but proud.",
        "Beware of the witch in the woods.",
        "Some say the lake is enchanted.",
        "I've never seen a dragon, thankfully.",
        "The ancient tree holds ancient power.",
        "Keep an eye on your belongings.",
        "Nothing beats our local honey.",
        "There's a chill in the air today.",
        "The old bookshop holds rare tomes.",
        "Watch the skies, stranger.",
        "Don't trust the goblins, ever.",
        "The stars have been bright lately."
    ];

    private static readonly List<string> FoodRequests =
    [
        "I'll have today's special, please.",
        "What's good on the menu today?",
        "I'd like a bowl of the stew, please.",
        "Can I have a pint and a burger?",
        "What do you recommend for a quick bite?",
        "I'll try the fish and chips, thanks.",
        "Any sandwich you'd suggest?",
        "I'm in the mood for a hearty pie.",
        "Got any house specials today?",
        "What's the most popular dish here?",
        "I'd love a serving of your shepherd's pie.",
        "How about your famous meatloaf?",
        "I'm starving – how's the roast beef?",
        "Anything warm and filling would be great.",
        "What's the soup of the day?",
        "Feeling like a steak sandwich today.",
        "I'll have your best hot dog and fries.",
        "What's fresh and quick to eat?",
        "I'd like something with chips, please.",
        "Got any specials on the grill?",
        "What's the chef's choice for today?",
        "Any hearty meals for a hungry traveller?",
        "Something warm and cheesy, please!"
    ];

    private static readonly List<string> ServingFoodResponses =
    [
        "Here is your order, enjoy the meal!",
        "Fresh out of the kitchen, one hot plate!",
        "Got your food right here. Bon appétit!",
        "Hope you are hungry! Here's your dish.",
        "Your order is ready. Looks delicious!",
        "Enjoy your meal.",
        "Here is the special of the day, just for you.",
        "Serving up your request. Eat up and enjoy!",
        "One tasty meal, coming right up!",
        "Your food's ready. Careful, its hot!",
        "Here you go, cooked to perfection!",
        "Just in time, your meal is ready.",
        "Here is what you asked for. Its a popular choice!",
        "Piping hot and delicious – just for you.",
        "And here's your order. That smells great!",
        "Fresh from the kitchen! Hope it hits the spot.",
        "That's one of our best dishes. Enjoy your meal!",
        "Here's your food, made with care.",
        "Served up and ready to go! Enjoy.",
        "Hope this meal makes your day better!",
        "Here is the hearty meal you asked for.",
        "Fresh off the grill, just for you.",
        "Your delicious order is all set!",
        "Ready to dig in? Here's your food.",
        "A perfect meal for a perfect guest!",
        "Enjoy the flavors of our house special.",
        "There you go! Enjoy every bite."
    ];

    private static readonly List<string> CustomerThanksCook =
    [
        "Thanks, {0}! The meal was fantastic.",
        "Much appreciated, {0}! Everything was delicious.",
        "{0}, that was an excellent meal, thank you!",
        "Kudos to you, {0}! This was really tasty.",
        "Thank you, {0}, for such a wonderful dish!",
        "Hats off to you, {0}! It was superb.",
        "{0}, you outdid yourself – thank you!",
        "My compliments to you, {0}. That was delightful!",
        "A big thank you to {0}! Great job.",
        "{0}, that was perfect! Thank you so much.",
        "Exceptional meal, {0}! I'm thoroughly impressed.",
        "Thank you, {0}! It was everything I hoped for.",
        "{0}, your cooking made my day, thanks!",
        "Delicious! Please thank {0} for me.",
        "{0}, you've got magic hands – thank you!",
        "That meal was amazing, {0}! Hats off!",
        "My thanks to {0} for an excellent meal!",
        "Bravo, {0}! The food was top-notch.",
        "{0}, your meal was the highlight of my day!",
        "Superb cooking, {0}! I really enjoyed it.",
        "{0}, you deserve all the praise – thank you!",
        "A heartfelt thanks to {0} for a great meal!",
        "Thank you, {0}! That was truly a treat.",
        "{0}, you've outdone yourself! It was great.",
        "Amazing meal, {0}! You're incredibly talented.",
        "{0}, your culinary skills are fantastic. Thanks!",
        "I'm impressed, {0}! Wonderful flavors.",
        "Thanks to {0} for an unforgettable meal!",
        "{0}, you nailed it! The food was excellent.",
        "Huge thanks, {0}! Every bite was a delight."
    ];

    private static readonly List<string> StartWereWolfConversation =
    [
        "{0}! Did you hear those howls last night?",
        "I couldn't sleep last night, the howls were so loud.",
        "The howls last night were unbearable!",
        "{0} are you hearing the howls at night?"
    ];

    private static readonly List<string> WerewolfReply1 =
    [
        "At first I thought it was just the wind but nope!",
        "Yes the howls! I wonder where they're coming from.",
        "I heard the howls, they sounded close.",
        "The howls are pretty frightening.",
    ];

    private static readonly List<string> StartWereWolfConversation2 =
    [
        "There must be a werewolf in our town.",
        "I think it's one of our town members.",
        "Someone in our town must be a werewolf.",
        "It seems to only be in our town, someone is causing them.",
    ];

    private static readonly List<string> WerewolfReply2 =
    [
        "Oh no. I hope it's not hungry.",
        "I really should lock my doors at night!",
        "If it's someone here, we should leave.",
        "Something sure is out there at night.",
    ];

    private static readonly List<string> StartWereWolfConversation3 =
    [
        "I better get back now, good talking with you {0}!",
        "I need to get going, lots to do before night fall.",
        "I am really scared, I should get going now.",
        "It could be one of us! I need to go now, bye {0}!",
    ];

    private static readonly List<string> WerewolfReply3 =
    [
        "See you again soon! Be safe out there.",
        "That's a good idea, please be safe!",
        "Don't wander alone! See you again soon.",
        "Be safe out there, bye!",
    ];

    #endregion Messages
}