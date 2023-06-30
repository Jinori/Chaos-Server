using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Common.Utilities;
using Chaos.Models.Data;
using Chaos.Services.Factories.Abstractions;
using Humanizer;

namespace Chaos.Scripting.MerchantScripts
{
    public class SerendaelMassScript : MerchantScriptBase
    {
        private const int FAITH_REWARD = 25;
        private const int LATE_FAITH_REWARD = 15;
        private const int ESSENCE_CHANCE = 10;
        private const int SERMON_DELAY_SECONDS = 5;
        private const int MASS_SERMON_COUNT = 10;
        
        #region MassMessages
        private readonly List<string> SerendaelMassMessages = new List<string>
        {
            "Gather as Miraelis' embrace fills this space.",
            "Compassion unites hearts, fosters understanding.",
            "Embrace nature, wisdom; inspire love and harmony.",
            "Let Miraelis' light guide a compassionate world.",
            "Blessings empower kindness, wisdom illuminates.",
            "Carry Miraelis' spirit, nurture compassion in all.",
            "May empathy bind us in a tapestry of unity.",
            "In Miraelis' embrace, solace and grace reside.",
            "Nature inspires; wisdom's flame enlightens.",
            "Seek wisdom's touch; let enlightenment guide.",
            "Open hearts blossom with compassion's touch.",
            "Nature's harmony reveals life's mysteries.",
            "Kindness begets kindness; let compassion flow.",
            "Miraelis' light guides compassionate souls.",
            "In nature's whispers, hear wisdom's voice.",
            "Let love and understanding be our foundation.",
            "Miraelis' grace heals and unifies our world.",
            "Embrace all life's beings, interconnectedness.",
            "Nature's tapestry reveals existence's secrets.",
            "Wisdom unveils the boundless universe within.",
            "Kindness ripples, spreading love endlessly.",
            "Miraelis' compassion soothes our troubled world.",
            "Nature's embrace offers peace and renewal.",
            "Wisdom enlightens on the quest for truth.",
            "Kindness ignites compassion's flame.",
            "Miraelis' touch heals and comforts.",
            "Nature whispers, universe's secrets unfold.",
            "Wisdom's path leads to profound understanding.",
            "Compassion flows, bridging divides between souls.",
            "Miraelis' blessings nurture and heal our world.",
            "Nature's sanctuary grants solace and renewal.",
            "Enlightenment blooms, hearts open to wisdom.",
            "Kindness, a gentle rain nourishes the soul.",
            "Miraelis' light guides through darkness' depths.",
            "Nature's embrace soothes and rejuvenates.",
            "Wisdom's journey unveils life's mysteries.",
            "Kindness, ripples of love, reverberates.",
            "Miraelis' wisdom guides in uncertain times.",
            "Nature's melody whispers cosmic secrets.",
            "Wisdom's flame illuminates paths of truth.",
            "Compassion connects souls, harmonious empathy.",
            "Miraelis' grace heals our fractured world.",
            "Nature's embrace restores inner peace.",
            "Enlightenment blooms when wisdom awakens.",
            "Kindness, language of the heart, transcends.",
            "Miraelis' touch stirs the spirit's essence.",
            "Nature's whispers unveil cosmic mysteries.",
            "Wisdom's path winds through understanding's depths.",
            "Compassion's flame guides through shadows' veil.",
            "Miraelis' love mends our fractured humanity.",
            "Nature's sanctuary renews and rejuvenates.",
            "Enlightenment dawns, wisdom illuminates minds.",
            "Kindness, gentle touch reverberates eternally.",
            "Miraelis' wisdom navigates uncertainty's sea.",
            "Nature's melody sings the cosmic symphony.",
            "Wisdom's flame illuminates the path.",
            "Compassion's bridge unites souls harmoniously.",
            "Miraelis' grace heals our divided world.",
            "Nature's eyes unveil the tapestry of existence.",
            "Wisdom's current carries to shores of enlightenment.",
            "Kindness, love's manifestation, radiant brilliance.",
            "Miraelis' embrace offers solace, renewal.",
            "Enlightenment blooms, mind receptive to light.",
            "Kindness, gentle rain nourishes soul's garden.",
            "Miraelis' touch whispers truth, guiding wisdom.",
            "Nature's whispers unveil cosmic secrets.",
            "Wisdom's path meanders, profound understanding.",
            "Compassion's flame guides in shadows' realm.",
            "Miraelis' love heals our fractured humanity.",
            "Nature's sanctuary brings serenity, renewal.",
            "Enlightenment blooms, mind receptive to light.",
            "Kindness, language of the heart, transcends.",
            "Miraelis' touch stirs the spirit's essence.",
            "Nature's whispers unveil cosmic mysteries.",
            "Wisdom's path winds through understanding's depths.",
            "Compassion's flame guides through shadows' veil.",
            "Miraelis' love mends our fractured humanity.",
            "Nature's sanctuary renews and rejuvenates.",
            "Enlightenment dawns, wisdom illuminates minds.",
            "Kindness, gentle touch reverberates eternally.",
            "Miraelis' wisdom navigates uncertainty's sea.",
            "Nature's melody sings the cosmic symphony.",
            "Wisdom's flame illuminates the path.",
            "Compassion's bridge unites souls harmoniously.",
            "Miraelis' grace heals our divided world.",
            "Nature's eyes unveil the tapestry of existence.",
            "Wisdom's current carries to shores of enlightenment.",
            "Kindness, love's manifestation, radiant brilliance.",
            "Miraelis' embrace offers solace, renewal."
        };
        #endregion MassMessages
        
        protected Animation PrayerSuccess { get; } = new()
        {
            AnimationSpeed = 60,
            TargetAnimation = 5
        };
        
        protected IItemFactory ItemFactory { get; }
        protected IClientRegistry<IWorldClient> ClientRegistry { get; }

        private bool AnnouncedMassFiveMinutes { get; set; }
        private bool AnnouncedMassOneMinute { get; set; }
        
        private bool AnnouncedMassBegin { get; set; }
        private DateTime? MassAnnouncementTime { get; set; }
        private DateTime? LastSermonTime { get; set; }
        private HashSet<string> SpokenMessages { get; set; } = new();
        private int SermonCount { get; set; }
        private IEnumerable<Aisling>? AislingsAtStart { get; set; }
        private bool MassCompleted { get; set; }

        public SerendaelMassScript(
            Merchant subject,
            IClientRegistry<IWorldClient> clientRegistry,
            IItemFactory itemFactory
        )
            : base(subject)
        {
            ItemFactory = itemFactory;
            ClientRegistry = clientRegistry;   
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
                else if (MassAnnouncementTime.HasValue && DateTime.UtcNow.Subtract(MassAnnouncementTime.Value).TotalMinutes >= 4 && !AnnouncedMassOneMinute)
                {
                    AnnounceMassOneMinuteStart();
                    AnnouncedMassOneMinute = true;
                }
                else if (MassAnnouncementTime.HasValue && DateTime.UtcNow.Subtract(MassAnnouncementTime.Value).TotalMinutes >= 5 && !AnnouncedMassBegin)
                {
                    AnnounceMassBeginning();
                    AnnouncedMassBegin = true;
                    MassAnnouncementTime = null; // Resetting the timer
                } 
                else if (AnnouncedMassBegin && !MassCompleted)
                {
                    ConductMass();
                }
                else if (MassCompleted)
                {
                    PostMassActions();
                }
            }
        }

        private void ConductMass()
        {
            if (SermonCount >= MASS_SERMON_COUNT)
            {
                MassCompleted = true; // Set flag to true when mass is finished
                return;
            }
            
            // Check if it's time to say the next sermon
            if (LastSermonTime.HasValue && DateTime.UtcNow.Subtract(LastSermonTime.Value).TotalSeconds < SERMON_DELAY_SECONDS)
            {
                return;
            }

            // Check if we have said all sermons
            if (SermonCount >= MASS_SERMON_COUNT)
            {
                return;
            }

            var random = new Random();
            int index;

            do
            {
                index = random.Next(SerendaelMassMessages.Count);
            }
            while (SpokenMessages.Contains(SerendaelMassMessages[index]));
    
            var message = SerendaelMassMessages[index];
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
                    var item = ItemFactory.Create($"essenceofSerendael");
                    player.Inventory.TryAddToNextSlot(item);
                    player.SendActiveMessage($"You received an Essence of Serendael");
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
        
        public static string? CheckDeity(Aisling source)
        {
            if (source.Legend.ContainsKey("Serendael"))
            {
                return "Serendael";
            }
            
            return null;
        }
        
        public void AnnounceMassFiveMinuteStart()
        {
            foreach (var client in ClientRegistry)
            {
                client.Aisling.SendActiveMessage("Serendael will be holding mass at her temple in five minutes.");
            }
        }

        public void AnnounceMassOneMinuteStart()
        {
            foreach (var client in ClientRegistry)
            {
                client.Aisling.SendActiveMessage("Serendael will be holding mass at her temple in one minute.");
            }
        }
        
        private void AnnounceMassBeginning()
        {
            foreach (var client in ClientRegistry)
                client.Aisling.SendActiveMessage($"Mass held by Serendael at the temple is starting now.");
    
            AislingsAtStart = Subject.MapInstance.GetEntities<Aisling>().ToList();

            Subject.Say(
                $"{AislingsAtStart.Count().ToWords().Humanize(LetterCasing.Title)} aislings bless me with their presence.");
        }
    }
}
