using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Religion.Abstractions;

public class ReligionScriptBase : DialogScriptBase
{
    public enum Rank
    {
        None,
        Worshipper,
        Acolyte,
        Emissary,
        Seer,
        Favor,
        Champion
    }

    private const string MIRAELIS_LEGEND_KEY = "Miraelis";
    private const string THESELENE_LEGEND_KEY = "Theselene";
    private const string SERENDAEL_LEGEND_KEY = "Serendael";
    private const string SKANDARA_LEGEND_KEY = "Skandara";
    private const int TEMPLE_SCROLL_FAITH_COST = 1;
    private const int FAITH_REWARD = 25;
    private const int LATE_FAITH_REWARD = 25;
    private const int ESSENCE_CHANCE = 10;

    private static readonly Dictionary<Rank, List<string>> RoleOptions = new()
    {
        {
            Rank.None, new List<string>
            {
                "Join the Temple",
                "The Gods"
            }
        },
        {
            Rank.Worshipper, new List<string>
            {
                "Pray",
                "A Path Home",
                "The Gods",
                "Leave Faith"
            }
        },
        {
            Rank.Acolyte, new List<string>
            {
                "Pray",
                "A Path Home",
                "The Gods",
                "Leave Faith"
            }
        },
        {
            Rank.Emissary, new List<string>
            {
                "Pray",
                "A Path Home",
                "The Gods",
                "Leave Faith"
            }
        },
        {
            Rank.Seer, new List<string>
            {
                "Pray",
                "A Path Home",
                "Hold Mass",
                "The Gods",
                "Leave Faith"
            }
        },
        {
            Rank.Favor, new List<string>
            {
                "Pray",
                "A Path Home",
                "Hold Mass",
                "The Gods",
                "Leave Faith"
            }
        },
        {
            Rank.Champion, new List<string>
            {
                "Pray",
                "A Path Home",
                "Hold Mass",
                "The Gods",
                "Leave Faith"
            }
        }
    };

    #region Prayers
    public readonly Dictionary<string, List<string>> DeityPrayers = new()
    {
        {
            "Serendael", [
                             "Oh Serendael, Lady of Fortune, bless us with your favorable hand.",
                             "Serendael, the weaver of destiny, guide us through the twists and turns of fate.",
                             "Grant us your wisdom, Serendael, to navigate the capricious paths of chance.",
                             "Serendael, in your multicolored gaze, may we find the courage to seize our destiny.",
                             "Guide us to favorable odds, Serendael, in the ever-shifting game of life.",
                             "Oh Serendael, bless our endeavours with your lucky charm.",
                             "May our paths cross the lines of your favor, Serendael.",
                             "Let your coin flip in our favor, mighty Serendael.",
                             "Serendael, guide us through the uncertainty of the morrow.",
                             "May we dance with fortune under your gaze, Serendael.",
                             "Oh Serendael, in your realm of chance, may we find our purpose.",
                             "Serendael, may your light guide us through the labyrinth of fate.",
                             "Serendael, let your multicolor radiance illuminate our journey.",
                             "Bless us with your serendipity, Serendael, to find joy in unexpected places.",
                             "May our journey align with your threads of destiny, Serendael.",
                             "Guide us in the dance of chance and choice, Serendael.",
                             "Bless us with the courage to seize the opportunities you weave, Serendael.",
                             "Serendael, in your endless dance of possibilities, may we find our rhythm.",
                             "May your winds of fortune steer us to calm waters, Serendael.",
                             "Serendael, may we be swift and sure in seizing the chances you offer.",
                             "Guide us through the vast tapestry of destiny, Serendael.",
                             "Bless us with your playful spirit, Serendael, to find joy in the dance of life.",
                             "Serendael, may we dance with destiny and pirouette with fortune.",
                             "Bless us with your kaleidoscopic gaze, Serendael, to see the beauty in change.",
                             "May we walk the tightrope of chance with your grace, Serendael.",
                             "Guide us to our destiny across your multicolored canvas, Serendael.",
                             "Serendael, may your playful winds guide our sails.",
                             "Bless us with the serenity to accept fate's turns, Serendael.",
                             "Serendael, guide us to find fortune in each day's sunrise.",
                             "May we find serenity in the uncertainty of fate, Serendael.",
                             "Bless our journey with your lucky stars, Serendael.",
                             "May we find joy in the unexpected gifts of life, Serendael.",
                             "Serendael, guide our steps in the dance of fortune and fate.",
                             "Bless us with the wisdom to see opportunity in uncertainty, Serendael.",
                             "May we dance with destiny in your cosmic ballroom, Serendael.",
                             "Bless our path with your guiding stars, Serendael.",
                             "Serendael, let us find joy in the dance of chance.",
                             "Bless our journey with the warmth of your radiant colors, Serendael.",
                             "May we navigate the maze of destiny under your watchful gaze, Serendael.",
                             "Serendael, bless us with the courage to take a chance."
                         ]
        },
        {
            "Skandara", [
                            "Mighty Skandara, grant us strength and courage to triumph in battle.",
                            "Skandara, goddess of war, may your fury drive us to victory.",
                            "Skandara, in the heart of the conflict, may we find the resolve to endure.",
                            "May our chains be broken under your watch, Skandara, freeing us to face our battles.",
                            "Skandara, bless us with the sharpness of your blade and the steadiness of your resolve.",
                            "Grant us your warrior spirit, Skandara, to face our trials.",
                            "May we stand strong in the face of adversity, guided by your light, Skandara.",
                            "Skandara, lend us your might to overcome the obstacles in our path.",
                            "May your battle cry echo in our hearts, Skandara, as we face our struggles.",
                            "Skandara, guide our sword hand and steady our shield arm in the face of battle.",
                            "Bless us with the courage of a lion and the strength of a bear, Skandara.",
                            "May we face our battles with unwavering resolve, guided by your light, Skandara.",
                            "Grant us your fury, Skandara, to face our trials with untamed spirit.",
                            "May we find the strength to endure in the heart of your storm, Skandara.",
                            "Skandara, bless our journey with the courage to face the unknown.",
                            "Guide us in the dance of war, Skandara, so we may stand victorious.",
                            "May our spirits burn with your ferocity, Skandara, as we face our trials.",
                            "Skandara, lend us your warrior heart to face our battles with bravery.",
                            "May we wield your courage as our sword and your wisdom as our shield, Skandara.",
                            "Skandara, guide us to face our struggles with the fierceness of a storm.",
                            "Bless us with your relentless spirit, Skandara, to face our battles.",
                            "Guide us through the tempest of struggle, Skandara, to find our strength.",
                            "May we find the strength to endure in the heart of the storm, Skandara.",
                            "Skandara, bless us with the courage to face our fears.",
                            "Guide us in the dance of battle, Skandara, so we may stand strong.",
                            "May our spirits burn with your fierce flame, Skandara, as we face our trials.",
                            "Skandara, lend us your strength to shatter the chains of our struggles.",
                            "May we wield your courage as our weapon and your wisdom as our armor, Skandara.",
                            "Skandara, guide us through the battlefield of life, to find our victory.",
                            "Bless us with the strength of your spirit, Skandara, to overcome our challenges.",
                            "Guide us in the symphony of conflict, Skandara, so we may find harmony.",
                            "May we face our battles with the courage of a lion and the strength of a bear, Skandara.",
                            "Skandara, lend us your fierce heart to stand strong in the face of adversity.",
                            "May our spirits roar with your courage, Skandara, as we face our trials.",
                            "Skandara, bless us with the steadfastness of your shield, to protect us in conflict.",
                            "Guide us through the storm of battle, Skandara, to find our calm.",
                            "May we stand strong in the face of the storm, guided by your strength, Skandara.",
                            "Skandara, lend us your warrior spirit to face our struggles with bravery."
                        ]
        },
        {
            "Theselene", [
                             "Theselene, Keeper of Secrets, reveal to us the hidden truths that lie in the shadows.",
                             "In your silent realm we seek clarity, guide us, Theselene, through the mysteries of life.",
                             "We walk in darkness, Theselene. Illuminate our path with the gleam of unveiled secrets.",
                             "Theselene, your whispers echo in the shadows. Grant us the discernment to understand their meaning.",
                             "In the quiet night, we seek your counsel, Theselene. Guide us to the secrets that sleep in its depths.",
                             "Theselene, cast your cloak of understanding over us. Let us perceive what lies beneath the surface.",
                             "Through a veil of shadows, may we glimpse the truth with your guidance, Theselene.",
                             "In obscurity, we find enlightenment. Theselene, lend us your night-born wisdom.",
                             "Silent Theselene, who treads the corridors of the unknown, reveal to us the enigmas of the universe.",
                             "In the moon's pale light, Theselene, we seek the knowledge hidden by day.",
                             "Theselene, your whispers stir the silence. Let us understand the secrets they carry.",
                             "Under your star-strewn mantle, Theselene, we seek the wisdom to decrypt the universe's riddles.",
                             "Theselene, Goddess of Shadows, guide us through the murk of confusion towards clarity.",
                             "Your wisdom, Theselene, is the lantern in the shadowy maze of existence.",
                             "Theselene, in the hush of twilight, we listen for your whispers of wisdom.",
                             "Under the starlit vault, we seek the mysteries only you can reveal, Theselene.",
                             "Theselene, in the quietude of shadows, your secrets reverberate. Help us comprehend their meaning.",
                             "In the penumbra of your domain, Theselene, we seek the wisdom to perceive what is veiled.",
                             "May the secrets of the night unravel under your guidance, Theselene.",
                             "Theselene, you walk the twilight path between known and unknown. Guide us along this way.",
                             "Oh silent Theselene, in the tranquility of your domain, let us hear the whispers of the cosmos.",
                             "Theselene, beneath the cloak of night, let us discover the universe's hidden narratives.",
                             "In your realm of quietude and mystery, Theselene, we seek the wisdom of the stars.",
                             "Theselene, when night descends, your secrets shimmer in the darkness. Help us decipher them.",
                             "May the wisdom of your twilight realm guide us, Theselene, to the concealed truths of existence.",
                             "Theselene, your silent whispers stir the darkness. Let their echoes be our guide.",
                             "Beneath the moon's quiet gaze, Theselene, we seek the knowledge hidden in its light.",
                             "In your realm, Theselene, secrets bloom like night flowers. Let us understand their language.",
                             "Theselene, in the obscurity of shadows, your wisdom gleams. Guide us to its source.",
                             "We seek the concealed wisdom of your shadowy realm, Theselene. Guide us on this quest."
                         ]
        },
        {
            "Miraelis", [
                            "Benevolent Miraelis, Goddess of Compassion, guide us in nurturing and understanding.",
                            "May your light shine upon us, filling our hearts with empathy and love.",
                            "Miraelis, grant us the wisdom to heal the wounds of the world with compassion.",
                            "In your embrace, we find solace and strength to uplift those in need.",
                            "Goddess of Nature and Intellect, teach us to honor and protect all living things.",
                            "Divine Miraelis, show us the path to unity and harmony among all beings.",
                            "In your presence, may empathy blossom, and kindness radiate in our every action.",
                            "Miraelis, bless us with the wisdom to embrace diversity and celebrate the tapestry of life.",
                            "With your guidance, may our intellect be tempered by compassion and used for the greater good.",
                            "Goddess of Compassion, may your gentle touch mend broken hearts and soothe troubled minds.",
                            "Divine Miraelis, may your nurturing essence bring peace and healing to all in need.",
                            "Grant us the strength to stand against cruelty and injustice, guided by your compassionate spirit.",
                            "Miraelis, in your name, may acts of kindness and empathy echo throughout the world.",
                            "Goddess of Nature, inspire us to protect and preserve the beauty that surrounds us.",
                            "May the wisdom of Miraelis guide us to be stewards of the earth and its precious resources.",
                            "Divine Miraelis, may our intellect be used to unravel the mysteries of the natural world and protect its fragile balance.",
                            "In your divine embrace, may we find comfort and solace in times of sorrow and despair.",
                            "Miraelis, instill in us the strength to forgive and let compassion flow even in the face of adversity.",
                            "Goddess of Compassion, may our hearts be open to the suffering of others, offering support and understanding.",
                            "Miraelis, in your presence, may we find the courage to show kindness and love to all we encounter.",
                            "Grant us the wisdom to see the interconnectedness of all life, and the responsibility to care for it.",
                            "Blessed Miraelis, may our actions reflect the gentle harmony of your compassionate nature.",
                            "May your light guide us through the darkest of times, leading us to paths of compassion and understanding.",
                            "Goddess of Nature, inspire us to walk gently upon the earth, preserving its beauty for generations to come.",
                            "In your name, Miraelis, may we nurture love and acceptance, creating a world where all are valued and respected.",
                            "Miraelis, fill our hearts with empathy, that we may bring comfort and healing to those who suffer.",
                            "Divine Miraelis, teach us the power of forgiveness, freeing ourselves and others from the chains of resentment.",
                            "May the kindness we show to others be a reflection of your divine compassion, Miraelis.",
                            "Goddess of Nature, may our actions honor the intricate web of life, fostering balance and harmony in all we do.",
                            "In your name, Miraelis, may we be a beacon of hope, spreading compassion and understanding throughout the world.",
                            "Blessed Miraelis, may our words and actions heal the wounds of the world, fostering peace and unity among all.",
                            "Miraelis, grant us the strength to extend our love and care to the natural world, preserving its wonders for future generations.",
                            "Goddess of Compassion, may we be a source of comfort and support to those who need it most.",
                            "Divine Miraelis, may our intellect be guided by wisdom and empathy, promoting understanding and harmony.",
                            "In your divine presence, may we find the courage to stand up against injustice, driven by love and compassion.",
                            "Miraelis, inspire us to walk a path of kindness, leaving a trail of compassion and understanding in our wake.",
                            "Goddess of Nature, may we be mindful of the delicate balance of ecosystems, nurturing and protecting their diversity.",
                            "In your name, Miraelis, may we sow seeds of love and compassion, nurturing a world where all beings can thrive.",
                            "Blessed Miraelis, may our actions uplift and inspire, spreading compassion and hope throughout the world.",
                            "Miraelis, may our hearts be filled with empathy, forging connections that bridge the divides among us.",
                            "Divine Miraelis, guide us to cherish the beauty of nature, awakening a deep sense of wonder and gratitude within us.",
                            "In your radiant light, Miraelis, may we find the strength to forgive ourselves and others, fostering healing and growth.",
                            "Goddess of Compassion, may our hands be vessels of comfort and aid, extending your loving touch to those in need.",
                            "Miraelis, may the seeds of compassion we sow blossom into a world where empathy is the guiding force.",
                            "In your name, Miraelis, may our intellect be a beacon of enlightenment, nurturing a future of wisdom and understanding.",
                            "Blessed Miraelis, may our actions echo the harmony of the natural world, fostering balance and sustainability.",
                            "Miraelis, may the compassion we show to others be a reflection of the compassion you bestow upon us.",
                            "Divine Miraelis, may we be guardians of the earth, tending to its precious ecosystems with reverence and care.",
                            "In your divine presence, may we learn to see the divine spark in every being, treating all with love and respect.",
                            "Goddess of Compassion, may our voices be filled with kindness and our words bring solace to those who listen.",
                            "Miraelis, in your name, may we be catalysts for positive change, igniting a world driven by empathy and compassion.",
                            "Grant us the strength to protect and restore the natural world, guided by your divine wisdom, Miraelis.",
                            "May the love and compassion we receive from you, Miraelis, overflow into the lives of those around us."
                        ]
        }
    };
    #endregion Prayers

    protected IClientRegistry<IChaosWorldClient> ClientRegistry { get; }

    private IExperienceDistributionScript ExperienceDistributionScript { get; }
    protected IItemFactory ItemFactory { get; }

    protected Animation PrayerSuccess { get; } = new()
    {
        AnimationSpeed = 60,
        TargetAnimation = 5
    };

    /// <inheritdoc />
    public ReligionScriptBase(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IItemFactory itemFactory)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        ItemFactory = itemFactory;
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
    }

    private void AddJoinQuestOptionIfApplicable(Aisling source, Dialog subject, string? deity)
    {
        if (source.Trackers.Enums.TryGetValue(out JoinReligionQuest stage))
            if (stage is JoinReligionQuest.MiraelisQuest
                         or JoinReligionQuest.SerendaelQuest
                         or JoinReligionQuest.SkandaraQuest
                         or JoinReligionQuest.TheseleneQuest)
                AddOption(subject, $"Essence of {deity}", $"{deity}_temple_completejoinQuest");
    }

    private void AddOption(Dialog subject, string optionName, string templateKey)
    {
        if (!subject.GetOptionIndex(optionName)
                    .HasValue)
            subject.AddOption(optionName, templateKey);
    }

    private void AnnounceMassEnd(string deity)
    {
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage($"{deity} mass has been concluded.");
    }

    public void AnnounceMassStart(Aisling source, string deity, bool self)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
        {
            Subject.Reply(
                source,
                $"You cannot host a mass at this time or use your voice to shout. \nTry again in {
                    timedEvent.Remaining.ToReadableString()}.");

            return;
        }

        foreach (var client in ClientRegistry)
            switch (self)
            {
                case true:
                    client.Aisling.SendActiveMessage($"{source.Name} will be hosting {deity} mass in five minutes.");

                    break;
                case false:
                    client.Aisling.SendActiveMessage($"{deity} will be holding mass at her temple in five minutes.");

                    break;
            }
    }

    public void AnnounceOneMinuteWarning(Aisling source, string deity, bool self)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
        {
            Subject.Reply(
                source,
                $"You cannot host a mass at this time or use your voice to shout. \nTry again in {
                    timedEvent.Remaining.ToReadableString()}.");

            return;
        }

        foreach (var client in ClientRegistry)
            switch (self)
            {
                case true:
                    client.Aisling.SendActiveMessage($"{deity} mass held by {source.Name} starts in a minute.");

                    break;
                case false:
                    client.Aisling.SendActiveMessage($"Mass held by {deity} will begin in one minute.");

                    break;
            }
    }

    public void AwardAttendees(
        Aisling source,
        string deity,
        IEnumerable<Aisling> aislingsAtStart,
        Merchant? goddess,
        bool self)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
        {
            Subject.Reply(
                source,
                $"You cannot hold Mass at this time. You've already hosted it recently. \nTry again in {
                    timedEvent.Remaining.ToReadableString()}.");

            return;
        }

        source.Legend.AddOrAccumulate(
            new LegendMark(
                $"Last Held Mass for {deity}",
                $"{deity}Mass",
                MarkIcon.Heart,
                MarkColor.White,
                1,
                GameTime.Now));

        switch (self)
        {
            case true:
            {
                var aislings = goddess?.MapInstance
                                      .GetEntities<Aisling>()
                                      .ToList();

                if (aislings != null)
                    foreach (var player in aislings)
                    {
                        if (!IsDeityMember(player, deity))
                        {
                            player.SendOrangeBarMessage("You aren't a member of this deity, you gain nothing.");

                            continue;
                        }

                        if (player.Trackers.TimedEvents.HasActiveEvent($"attendedmass{deity}", out _))
                        {
                            player.SendOrangeBarMessage("You've attended mass too recently.");

                            continue;
                        }

                        player.Animate(PrayerSuccess);

                        if (IntegerRandomizer.RollChance(ESSENCE_CHANCE))
                        {
                            var item = ItemFactory.Create($"essenceof{deity}");
                            player.Inventory.TryAddToNextSlot(item);
                            player.SendActiveMessage($"You received an Essence of {deity} and faith!");
                        } else
                            player.SendActiveMessage("You receive faith!");

                        TryAddFaith(player, FAITH_REWARD);
                        UpdateReligionRank(player);
                        player.Trackers.TimedEvents.AddEvent($"attendedmass{deity}", TimeSpan.FromHours(18), true);
                        var tnl = LevelUpFormulae.Default.CalculateTnl(player);
                        var twentyFivePercent = Convert.ToInt32(.25 * tnl);

                        if (player.UserStatSheet.Level < 99)
                            ExperienceDistributionScript.GiveExp(player, twentyFivePercent);
                        else
                            ExperienceDistributionScript.GiveExp(player, 10000000);
                    }

                goddess?.Say($"Thank you, {source.Name}. You honor me.");
                AnnounceMassEnd(deity);
                source.Trackers.TimedEvents.AddEvent("Mass", TimeSpan.FromDays(4), true);

                break;
            }
            case false:
            {
                var aislingsAtEnd = goddess?.MapInstance
                                           .GetEntities<Aisling>()
                                           .ToList();

                var aislingsStillHere = aislingsAtStart.Intersect(aislingsAtEnd!)
                                                       .ToList();

                foreach (var player in aislingsStillHere)
                {
                    player.Animate(PrayerSuccess);

                    if (IntegerRandomizer.RollChance(ESSENCE_CHANCE))
                    {
                        var item = ItemFactory.Create($"essenceof{deity}");
                        player.Inventory.TryAddToNextSlot(item);
                        player.SendActiveMessage($"You received an Essence of {deity} and faith!");
                    } else
                        player.SendActiveMessage("You receive faith!");

                    TryAddFaith(player, FAITH_REWARD);
                    UpdateReligionRank(player);
                    player.Trackers.TimedEvents.AddEvent($"attendedmass{deity}", TimeSpan.FromHours(18), true);
                    var tnl = LevelUpFormulae.Default.CalculateTnl(player);
                    var twentyFivePercent = Convert.ToInt32(.25 * tnl);

                    if (player.UserStatSheet.Level < 99)
                        ExperienceDistributionScript.GiveExp(player, twentyFivePercent);
                    else
                        ExperienceDistributionScript.GiveExp(player, 10000000);
                }

                foreach (var latePlayers in aislingsAtEnd!.Except(aislingsStillHere))
                {
                    latePlayers.SendActiveMessage("You must be present from start to finish to receive full benefits.");
                    TryAddFaith(latePlayers, LATE_FAITH_REWARD);
                }

                source.Trackers.TimedEvents.AddEvent("Mass", TimeSpan.FromDays(4), true);

                break;
            }
        }
    }

    private static int CheckCurrentFaith(Aisling source)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
            return faith.Count;

        return 0;
    }

    public static string? CheckDeity(Aisling source)
    {
        var deityKeys = new[]
        {
            MIRAELIS_LEGEND_KEY,
            THESELENE_LEGEND_KEY,
            SERENDAEL_LEGEND_KEY,
            SKANDARA_LEGEND_KEY
        };

        foreach (var key in deityKeys)
            if (source.Legend.ContainsKey(key))
                return key;

        return null;
    }

    public void CheckJoinQuestCompletion(Aisling source, string deity)
    {
        if (source.Inventory.CountOf($"Essence of {deity}") < 3)
        {
            source.SendActiveMessage($"You do not have the required Essence of {deity}.");

            Subject.Reply(
                source,
                $"Though the essence of {deity} eludes you, seeker, remember that the journey itself holds wisdom. Reflect on your quest, learn from its challenges, and embrace compassion, nature, and intellect in your future endeavors. The path to enlightenment awaits your renewed efforts.");

            return;
        }

        source.Trackers.Enums.Set(typeof(JoinReligionQuest), JoinReligionQuest.JoinReligionComplete);

        source.Legend.AddOrAccumulate(
            new LegendMark(
                $"Worshipper of {deity}",
                deity,
                MarkIcon.Heart,
                MarkColor.White,
                1,
                GameTime.Now));

        var trinket = ItemFactory.Create($"{deity}Stone");
        source.Inventory.TryAddToNextSlot(trinket);
        source.Inventory.RemoveQuantity($"Essence of {deity}", 3);
        source.SendActiveMessage($"You have joined the temple of {deity} as a Worshipper!");
    }

    public void CreateTempleScroll(Aisling source, string deity)
    {
        if (CheckCurrentFaith(source) <= 1)
        {
            Subject.Reply(source, "Your faith is too low to create a scroll to the temple.", $"{deity}_temple_initial");

            return;
        }

        if (!TrySubtractFaith(source, TEMPLE_SCROLL_FAITH_COST))
        {
            Subject.Reply(source, "Your faith may be too low or there was an issue.", $"{deity}_temple_initial");

            return;
        }

        source.SendActiveMessage($"In gratitude for your loyalty, {deity} hands you a passport.");

        var scroll = ItemFactory.Create("passport");
        source.Inventory.TryAddToNextSlot(scroll);
    }

    private Rank DetermineRank(int faithCount)
        => faithCount switch
        {
            <= 0                          => Rank.None,
            >= FaithThresholds.CHAMPION   => Rank.Champion,
            >= FaithThresholds.FAVOR      => Rank.Favor,
            >= FaithThresholds.SEER       => Rank.Seer,
            >= FaithThresholds.EMISSARY   => Rank.Emissary,
            >= FaithThresholds.ACOLYTE    => Rank.Acolyte,
            >= FaithThresholds.WORSHIPPER => Rank.Worshipper,
            _                             => Rank.Worshipper
        };

    public Rank GetPlayerRank(Aisling source)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
            return DetermineRank(faith.Count);

        return Rank.None;
    }

    public void GoddessHoldMass(Aisling source, string deity, Merchant? goddess)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("Mass", out var timedEvent))
        {
            Subject.Reply(
                source,
                $"You cannot hold Mass at this time. \nTry again in {
                    timedEvent.Remaining.ToReadableString()}.");

            return;
        }

        switch (goddess)
        {
            case { CurrentlyHostingMass: true }:
                Subject.Reply(source, "I am currently busy already holding Mass at this time.");

                return;

            case { CurrentlyHostingMass: false }:
            {
                goddess.CurrentlyHostingMass = true;
                source.Trackers.TimedEvents.AddEvent("Mass", TimeSpan.FromDays(4), true);

                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        $"Last Held Mass for {deity}",
                        $"{deity}Mass",
                        MarkIcon.Heart,
                        MarkColor.White,
                        1,
                        GameTime.Now));

                break;
            }
        }
    }

    public void HideDialogOptions(Aisling source, string? deity, Dialog subject)
    {
        var playerGod = CheckDeity(source);

        if ((playerGod != null) && (playerGod != deity))
        {
            subject.Reply(source, $"Your faith lies within {playerGod} already. Please seek them out.");

            return;
        }

        var rank = GetPlayerRank(source);

        var allOptions = new List<string>
        {
            "Pray",
            "A Path Home",
            "Hold Mass",
            "Join the Temple",
            "Leave Faith",
            "The Gods"
        };

        if (RoleOptions.TryGetValue(rank, out var allowedOptions))
        {
            var optionsToRemove = allOptions.Except(allowedOptions)
                                            .ToList();

            foreach (var option in optionsToRemove)
                RemoveOption(subject, option);
        }

        if (Subject.DialogSource.EntityType == EntityType.Item)
            RemoveOption(subject, "Hold Mass");

        if (rank == Rank.None)
        {
            AddJoinQuestOptionIfApplicable(source, subject, deity);
            RemoveOption(subject, "Leave Faith");
        }
    }

    public static bool IsDeityMember(Aisling source, string deity) => source.Legend.ContainsKey(deity);

    public void JoinDeity(Aisling source, string deity)
    {
        if (!IsDeityMember(source, deity))
        {
            source.Legend.TryGetValue(deity, out var existingMark);

            if (existingMark is not null)
            {
                source.SendActiveMessage("You already belong to a deity.");

                return;
            }

            source.Legend.AddOrAccumulate(
                new LegendMark(
                    $"Worshipper of {deity}",
                    deity,
                    MarkIcon.Heart,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            var worldClients = ClientRegistry.Where(x => x.Aisling.Legend.ContainsKey(deity));

            foreach (var client in worldClients)
                client.Aisling.SendActiveMessage($"{source.Name} has joined the ranks of {deity}.");
        }
    }

    public void LeaveDeity(Aisling source, string deity)
    {
        source.Legend.TryGetValue(deity, out var existingMark);

        if (existingMark is null)
        {
            source.SendActiveMessage("You do not belong to a deity.");

            return;
        }

        if (source.Inventory.ContainsByTemplateKey($"{deity}Stone"))
            source.Inventory.RemoveQuantityByTemplateKey($"{deity}Stone", 1);

        if (source.Bank.Contains($"{deity} Stone"))
            source.Bank.TryWithdraw($"{deity} Stone", 1, out _);

        source.Trackers.Enums.Set(JoinReligionQuest.None);
        source.Legend.Remove(deity, out _);
        source.SendActiveMessage($"You turn your back on {deity} and leave the ranks of worship.");
    }

    public void Pray(Aisling source, string deity)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("PrayerCooldown", out var timedEvent))
        {
            Subject.Reply(
                source,
                $"You cannot pray with {deity} at this time. \nTry again in {
                    timedEvent.Remaining.ToReadableString()}.");

            return;
        }

        var currentPrayerCount = source.Trackers.Enums.TryGetValue(out ReligionPrayer count);

        if (!currentPrayerCount)
            count = ReligionPrayer.None;

        if (count < ReligionPrayer.End)
        {
            count++;
            source.Trackers.Enums.Set(typeof(ReligionPrayer), count);
            TryAddFaith(source, 1);
            UpdateReligionRank(source);
            var tnl = LevelUpFormulae.Default.CalculateTnl(source);
            var threePercent = Convert.ToInt32(.03 * tnl);

            ExperienceDistributionScript.GiveExp(source, threePercent);
            source.Animate(PrayerSuccess);

            if (IntegerRandomizer.RollChance(10))
            {
                var essence = ItemFactory.Create($"essenceof{deity}");
                source.Inventory.TryAddToNextSlot(essence);
                source.SendActiveMessage($"Through prayer, you receive a Essence of {deity}!");
            }
        } else
        {
            Subject.Reply(source, "You have reached your limit in prayer. Try again tomorrow!");
            source.Trackers.TimedEvents.AddEvent("PrayerCooldown", TimeSpan.FromHours(22), true);
            source.Trackers.Enums.Set(ReligionPrayer.None);
        }
    }

    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName)
                   .HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }

    public static void SendOnJoinQuest(Aisling source, string deity)
    {
        switch (deity)
        {
            case "Miraelis":
                source.Trackers.Enums.Set(JoinReligionQuest.MiraelisQuest);

                break;
            case "Skandara":
                source.Trackers.Enums.Set(JoinReligionQuest.SkandaraQuest);

                break;
            case "Theselene":
                source.Trackers.Enums.Set(JoinReligionQuest.TheseleneQuest);

                break;
            case "Serendael":
                source.Trackers.Enums.Set(JoinReligionQuest.SerendaelQuest);

                break;
        }

        source.SendActiveMessage($"{deity} is requesting (3) Essence of {deity}.");
    }

    public static bool TryAddFaith(Aisling source, int amount)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
        {
            faith.Count += amount;

            return true;
        }

        return false;
    }

    public bool TrySubtractFaith(Aisling source, int amount)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
            if (amount < faith.Count)
            {
                source.Legend.RemoveCount(key, amount, out _);

                return true;
            }

        return false;
    }

    private void UpdateReligionRank(Aisling source)
    {
        var key = CheckDeity(source);

        if (key != null)
        {
            source.Legend.TryGetValue(key, out var existingMark);

            if (existingMark is null)
                source.Legend.AddOrAccumulate(
                    new LegendMark(
                        $"Worshipper of {key}",
                        key,
                        MarkIcon.Heart,
                        MarkColor.White,
                        1,
                        GameTime.Now));

            if (existingMark is not null)
            {
                var faithCount = existingMark.Count;
                var previousRank = existingMark.Text;

                var newRank = faithCount switch
                {
                    > 1499 when !previousRank.Contains("Champion")   => $"Champion of {key}",
                    > 999 when !previousRank.Contains("Favor")       => $"Favor of {key}",
                    > 749 when !previousRank.Contains("Seer")        => $"Seer of {key}",
                    > 499 when !previousRank.Contains("Emissary")    => $"Emissary of {key}",
                    > 299 when !previousRank.Contains("Acolyte")     => $"Acolyte of {key}",
                    <= 299 when !previousRank.Contains("Worshipper") => $"Worshipper of {key}",
                    _                                                => previousRank
                };

                var rankOrder = new List<string>
                {
                    "Worshipper",
                    "Acolyte",
                    "Emissary",
                    "Seer",
                    "Favor",
                    "Champion"
                };
                var currentRankIndex = rankOrder.FindIndex(rank => previousRank.Contains(rank));
                var newRankIndex = rankOrder.FindIndex(rank => newRank.Contains(rank));

                if (newRankIndex > currentRankIndex)
                {
                    existingMark.Text = newRank;
                    source.SendActiveMessage($"Your rank has increased to {newRank}.");
                }
            }
        }
    }

    public class FaithThresholds
    {
        public const int WORSHIPPER = 150;
        public const int ACOLYTE = 300;
        public const int EMISSARY = 500;
        public const int SEER = 750;
        public const int FAVOR = 1000;
        public const int CHAMPION = 1500;
    }
}