using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MerchantScripts;

public class TheseleneMassScript(
    Merchant subject,
    IClientRegistry<IWorldClient> clientRegistry,
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
    private readonly List<string> TheseleneMassMessages =
    [
        "In shadows' embrace, mysteries find solace.",
        "Whispers of secrets reveal hidden truths.",
        "Nature of secrets, behold the dance of shadows.",
        "Shadows cloak secrets, unveiling their essence.",
        "In whispers of shadows, wisdom lies hidden.",
        "Illuminate hidden paths of knowledge.",
        "Through shadows' realm, secrets take flight.",
        "Silent whispers echo within the grasp of shadow.",
        "Secrets hold untold knowledge and revelations.",
        "Shadows conceal, offering veiled revelations.",
        "Within shadows' depths, truth's essence resides.",
        "Pierce the veil of secrets with your gaze.",
        "Hidden truths awaken in shadows' embrace.",
        "The realm reveals the unseen truths, hidden and profound.",
        "Secrets unfold, guided by unseen hands.",
        "In the realm of shadows, secrets are unveiled.",
        "Uncover hidden whispers of truth.",
        "Secrets whispered in shadows speak the truth.",
        "Mysteries dance in the shadows, waiting to be discovered.",
        "The unseen realm holds secrets and enigmatic wisdom.",
        "In shadows' realm, secrets lie in wait.",
        "Whispers carry hidden knowledge and truth.",
        "Hidden truths unfurl within shadows' embrace.",
        "Essence reveals the hidden world, both mystical and profound.",
        "Within shadows' depths, the realm blooms with hidden knowledge.",
        "Hold the keys to the secrets, unraveling mysteries.",
        "Mysteries unravel within the unseen domain, guided by presence.",
        "In the realm of secrets, truth whispers.",
        "Shadows hold the keys to eternal domains.",
        "Secrets unfold in shadow's embrace, revealing the unknown.",
        "Whispers unveil hidden truths and profound wisdom.",
        "In the realm of shadows, wisdom awaits seekers of truth.",
        "Touch uncovers the mysteries within the shadows.",
        "Secrets reside in eternal embrace, waiting to be discovered.",
        "Whispers carry the weight of truth, guiding seekers in the dark.",
        "Shadows conceal enigmatic wisdom, waiting to be revealed.",
        "Within the veil of shadows, knowledge thrives and illuminates.",
        "Realm breathes life into hidden secrets, unlocking their power.",
        "In the realm of shadows, essence resonates, beckoning the curious.",
        "Secrets bloom in the depths of darkness, offering profound insights.",
        "Within the shadows, whispers hold the key to hidden truths.",
        "Touch reveals the hidden tapestry, connecting the threads of knowledge.",
        "Shadows offer solace as secrets unfold in their comforting embrace.",
        "Whispers guide seekers of hidden truths, leading them on profound journeys.",
        "In the realm of shadows, the realm takes form, inviting explorations of the unknown.",
        "Secrets unravel within shadows' depths, revealing the mysteries they hold.",
        "Whispers guide seekers of the unknown, unveiling profound revelations.",
        "In the realm of shadows, wisdom beckons, calling forth those who seek enlightenment."
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
    protected IClientRegistry<IWorldClient> ClientRegistry { get; } = clientRegistry;

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
            client.Aisling.SendActiveMessage("Mass held by Theselene at the temple is starting now.");

        AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();

        Subject.Say($"{AislingsAtStart.Count().ToWords().Humanize(LetterCasing.Title)} aislings bless me with their presence.");
    }

    public void AnnounceMassFiveMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Theselene will be holding mass at her temple in five minutes.");
    }

    public void AnnounceMassOneMinuteStart()
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage("Theselene will be holding mass at her temple in one minute.");
    }

    public static string? CheckDeity(Aisling source)
    {
        if (source.Legend.ContainsKey("Theselene"))
            return "Theselene";

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
            index = random.Next(TheseleneMassMessages.Count);
        while (SpokenMessages.Contains(TheseleneMassMessages[index]));

        var message = TheseleneMassMessages[index];
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
                var item = ItemFactory.Create("essenceofTheselene");
                player.Inventory.TryAddToNextSlot(item);
                player.SendActiveMessage("You received an Essence of Theselene");
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