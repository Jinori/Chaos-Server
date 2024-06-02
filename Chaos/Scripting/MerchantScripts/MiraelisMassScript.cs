using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MerchantScripts;

public class MiraelisMassScript(
    Merchant subject,
    IClientRegistry<IChaosWorldClient> clientRegistry,
    IItemFactory itemFactory
)
    : MerchantScriptBase(subject)
{
    private const int FAITH_REWARD = 25;
    private const int ESSENCE_CHANCE = 10;
    private const int LATE_FAITH_REWARD = 15;
    private const int SERMON_DELAY_SECONDS = 5;
    private const int MASS_SERMON_COUNT = 10;

    #region MassMessages
    private readonly List<string> MiraelisMassMessages =
    [
        "Compassion unites hearts, fosters understanding.",
        "Embrace nature, wisdom; inspire love and harmony.",
        "Let the light of compassion guide our actions.",
        "May empathy bind us in a tapestry of unity.",
        "In solace and grace, embrace resides.",
        "Nature inspires; wisdom illuminates our path.",
        "Kindness begets kindness; let compassion flow.",
        "The light of compassion guides compassionate souls.",
        "In nature's whispers, hear wisdom's voice.",
        "Let love and understanding be our foundation.",
        "Grace heals and unifies our world.",
        "Embrace all life's beings, interconnectedness.",
        "Nature's tapestry reveals existence's secrets.",
        "Wisdom unveils the boundless universe within.",
        "Kindness ripples, spreading love endlessly.",
        "Compassion soothes our troubled world.",
        "Nature's embrace offers peace and renewal.",
        "Wisdom's journey unveils life's mysteries.",
        "Kindness ignites compassion's flame.",
        "The touch heals and comforts.",
        "Nature whispers, universe's secrets unfold.",
        "Wisdom's path leads to profound understanding.",
        "Compassion flows, bridging divides between souls.",
        "Blessings nurture and heal our world.",
        "Nature's sanctuary grants solace and renewal.",
        "Enlightenment blooms, hearts open to wisdom.",
        "Kindness, a gentle rain nourishes the soul.",
        "The light guides through darkness' depths.",
        "Nature's embrace soothes and rejuvenates.",
        "Wisdom's journey unveils life's mysteries.",
        "Compassion connects souls, harmonious empathy.",
        "Grace heals our fractured world.",
        "Nature's sanctuary brings serenity, renewal.",
        "Enlightenment dawns, wisdom illuminates minds.",
        "Kindness, gentle touch reverberates eternally.",
        "Wisdom navigates uncertainty's sea.",
        "Nature's melody sings the cosmic symphony.",
        "Wisdom's flame illuminates the path.",
        "Compassion's bridge unites souls harmoniously.",
        "Love mends our fractured humanity.",
        "Nature's sanctuary renews and rejuvenates.",
        "Enlightenment blooms, mind receptive to light.",
        "Kindness, language of the heart, transcends.",
        "The touch stirs the spirit's essence.",
        "Nature's whispers unveil cosmic mysteries.",
        "Wisdom's path winds through understanding's depths.",
        "Compassion's flame guides through shadows' veil.",
        "Love heals our fractured humanity.",
        "Nature's sanctuary renews and rejuvenates.",
        "Enlightenment dawns, wisdom illuminates minds.",
        "Kindness, gentle touch reverberates eternally.",
        "Wisdom navigates uncertainty's sea.",
        "Nature's melody sings the cosmic symphony.",
        "Wisdom's flame illuminates the path.",
        "Compassion's bridge unites souls harmoniously.",
        "Grace heals our divided world.",
        "Nature's eyes unveil the tapestry of existence.",
        "Wisdom's current carries to shores of enlightenment.",
        "Kindness, love's manifestation, radiant brilliance.",
        "The embrace offers solace, renewal.",
        "Enlightenment blooms, mind receptive to light.",
        "Kindness, gentle rain nourishes soul's garden.",
        "The touch whispers truth, guiding wisdom.",
        "Nature's whispers unveil cosmic secrets.",
        "Wisdom's path winds through understanding's depths.",
        "Compassion's flame guides in shadows' realm.",
        "Love mends our fractured humanity.",
        "Nature's sanctuary renews and rejuvenates.",
        "Enlightenment dawns, wisdom illuminates minds.",
        "Kindness, gentle touch reverberates eternally.",
        "Wisdom navigates uncertainty's sea.",
        "Nature's melody sings the cosmic symphony.",
        "Wisdom's flame illuminates the path.",
        "Compassion's bridge unites souls harmoniously.",
        "Grace heals our divided world.",
        "Nature's eyes unveil the tapestry of existence.",
        "Wisdom's current carries to shores of enlightenment.",
        "Kindness, love's manifestation, radiant brilliance.",
        "The embrace offers solace, renewal."
    ];
    #endregion MassMessages

    private IEnumerable<Aisling>? AislingsAtStart { get; set; }

    private bool AnnouncedMassBegin { get; set; }

    private bool AnnouncedMassFiveMinutes { get; set; }
    private bool AnnouncedMassOneMinute { get; set; }
    private DateTime? LastSermonTime { get; set; }
    private DateTime? MassAnnouncementTime { get; set; }
    private bool MassCompleted { get; set; }
    private int SermonCount { get; set; }
    protected IClientRegistry<IChaosWorldClient> ClientRegistry { get; } = clientRegistry;

    protected IItemFactory ItemFactory { get; } = itemFactory;

    protected Animation PrayerSuccess { get; } = new()
    {
        AnimationSpeed = 60,
        TargetAnimation = 5
    };
    private HashSet<string> SpokenMessages { get; } = new();

    private void AnnounceMassBeginning()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Mass held by Miraelis at the temple is starting now.");

        AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();

        Subject.Say($"{AislingsAtStart.Count().ToWords().Humanize(LetterCasing.Title)} aislings bless me with their presence.");
    }

    public void AnnounceMassFiveMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Miraelis will be holding mass at her temple in five minutes.");
    }

    public void AnnounceMassOneMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Miraelis will be holding mass at her temple in one minute.");
    }

    public static string? CheckDeity(Aisling source)
    {
        if (source.Legend.ContainsKey("Miraelis"))
            return "Miraelis";

        return null;
    }

    private void ConductMass()
    {
        if (SermonCount >= MASS_SERMON_COUNT)
        {
            MassCompleted = true; // Set flag to true when mass is finished

            return;
        }

        // Check if it's time to say the next sermon
        if (LastSermonTime.HasValue && (DateTime.UtcNow.Subtract(LastSermonTime.Value).TotalSeconds < SERMON_DELAY_SECONDS))
            return;

        // Check if we have said all sermons
        if (SermonCount >= MASS_SERMON_COUNT)
            return;

        var random = new Random();
        int index;

        do
            index = random.Next(MiraelisMassMessages.Count);
        while (SpokenMessages.Contains(MiraelisMassMessages[index]));

        var message = MiraelisMassMessages[index];
        Subject.Say(message);
        SpokenMessages.Add(message);
        LastSermonTime = DateTime.UtcNow;
        SermonCount++;
    }

    private void PostMassActions()
    {
        Subject.Say("Mass is complete!");
        var aislingsAtEnd = Subject.MapInstance.GetEntities<Aisling>().ToList();
        var aislingsStillHere = AislingsAtStart!.Intersect(aislingsAtEnd).ToList();

        foreach (var player in aislingsStillHere)
        {
            player.Animate(PrayerSuccess);

            if (IntegerRandomizer.RollChance(ESSENCE_CHANCE))
            {
                var item = ItemFactory.Create("essenceofMiraelis");
                player.Inventory.TryAddToNextSlot(item);
                player.SendActiveMessage("You received an Essence of Miraelis");
            }

            TryAddFaith(player, FAITH_REWARD);
        }

        foreach (var latePlayers in aislingsAtEnd.Except(aislingsStillHere))
        {
            latePlayers.SendActiveMessage("You must be present from start to finish to receive full benefits.");
            TryAddFaith(latePlayers, LATE_FAITH_REWARD);
        }

        Subject.CurrentlyHostingMass = false;
    }

    public static bool TryAddFaith(Aisling source, int amount)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
        {
            faith.Count += amount;
            source.SendActiveMessage($"You received {amount} faith!");

            return true;
        }

        return false;
    }

    public override void Update(TimeSpan delta)
    {
        if (Subject.CurrentlyHostingMass)
        {
            if (!AnnouncedMassFiveMinutes)
            {
                AnnounceMassFiveMinuteStart();
                MassAnnouncementTime = DateTime.UtcNow;
                AnnouncedMassFiveMinutes = true;
            }
            else if (MassAnnouncementTime.HasValue
                     && (DateTime.UtcNow.Subtract(MassAnnouncementTime.Value).TotalMinutes >= 4)
                     && !AnnouncedMassOneMinute)
            {
                AnnounceMassOneMinuteStart();
                AnnouncedMassOneMinute = true;
            }
            else if (MassAnnouncementTime.HasValue
                     && (DateTime.UtcNow.Subtract(MassAnnouncementTime.Value).TotalMinutes >= 5)
                     && !AnnouncedMassBegin)
            {
                AnnounceMassBeginning();
                AnnouncedMassBegin = true;
                MassAnnouncementTime = null; // Resetting the timer
            }
            else if (AnnouncedMassBegin && !MassCompleted)
                ConductMass();
            else if (MassCompleted)
                PostMassActions();
        }
    }
}