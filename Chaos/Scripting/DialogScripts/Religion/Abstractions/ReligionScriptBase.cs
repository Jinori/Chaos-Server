using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Religion.Abstractions;

public class ReligionScriptBase : DialogScriptBase
{
    protected IClientRegistry<IWorldClient> ClientRegistry { get; }
    protected IItemFactory ItemFactory { get; }
    private const string MIRAELIS_LEGEND_KEY = "Miraelis";
    private const string THESELENE_LEGEND_KEY = "Theselene";
    private const string SERENDAEL_LEGEND_KEY = "Serendael";
    private const string SKANDARA_LEGEND_KEY = "Skandara";
    private const int TEMPLE_SCROLL_FAITH_COST = 1;
    private const int AMOUNT_TO_TRANSFER_FAITH = 5;
    
    protected Animation PrayerSuccess { get; } = new()
    {
        AnimationSpeed = 60,
        TargetAnimation = 5
    };
    #region Prayers
    public readonly Dictionary<string, List<string>> DeityPrayers = new Dictionary<string, List<string>>()
    {
        {
            "Serendael", new List<string>
            {
                "Oh Serendael, Lady of Fortune, bless us with your favorable hand.",
                "Serendael, the weaver of destiny, guide us through the twists and turns of fate.",
                // Add more prayers for Serendael as desired
            }
        },
        {
            "Skandara", new List<string>
            {
                "Mighty Skandara, grant us strength and courage to triumph in battle.",
                "Skandara, goddess of war, may your fury drive us to victory.",
                // Add more prayers for Skandara as desired
            }
        },
        {
            "Theselene", new List<string>
            {
                "Theselene, Keeper of Secrets, reveal to us the hidden truths that lie in the shadows.",
                "Grant us the wisdom to unravel the mysteries that cloak our path.",
                // Add more prayers for Theselene as desired
            }
        },
        {
            "Miraelis", new List<string>
            {
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
                "May the love and compassion we receive from you, Miraelis, overflow into the lives of those around us.",
            }
        }
    };
    #endregion Prayers
    /// <inheritdoc />
    public ReligionScriptBase(Dialog subject, IClientRegistry<IWorldClient> clientRegistry, IItemFactory itemFactory)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        ItemFactory = itemFactory;
    }
    

    public enum Rank
    {
        Worshipper,
        Acolyte,
        Emissary,
        Seer,
        Favor,
        Champion
    }

    public void TransferFaith(Aisling source, string deity)
    {
        //ensure the name is present
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);
            return;
        }

        var target = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name))?.Aisling;

        //ensure the player is online
        if (target is null)
        {
            Subject.Reply(source, $"{name} is not online", $"{deity}_temple_initial");
            return;
        }
        
        if (CheckCurrentFaith(source) <= AMOUNT_TO_TRANSFER_FAITH)
        {
            Subject.Reply(source, "Your faith is too low to transfer yours to another.", $"{deity}_temple_initial");
            return;
        }

        if (!TryTransferFaith(source, target))
        {
            Subject.Reply(source, "Your faith may be too low or there was an issue.", $"{deity}_temple_initial");
            return;
        }
        
        source.SendActiveMessage($"You bestow your faith upon {target.Name}. May {deity} bless you both.");
        target.SendActiveMessage($"{source.Name} has bestowed faith of {deity} upon you!");
        target.Animate(PrayerSuccess);
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
        
        source.SendActiveMessage($"In gratitude for your loyalty, {deity} hands you a temple scroll.");

        //Change to the specific god's temple map
        var templeScroll = deity switch
        {
            "Miraelis"  => "milethScroll",
            "Skandara"  => "milethScroll",
            "Theselene" => "milethScroll",
            "Serendael" => "milethScroll",
            _           => ""
        };

        var scroll = ItemFactory.Create(templeScroll);
        source.Inventory.TryAddToNextSlot(scroll);
    }
    
    public static void SendOnJoinQuest(Aisling source, string deity)
    {
        switch (deity)
        {
            case "Miraelis":
                source.Trackers.Enums.Set(typeof(JoinReligionQuest), JoinReligionQuest.MiraelisQuest);
                break;
            case "Skandara":
                source.Trackers.Enums.Set(typeof(JoinReligionQuest), JoinReligionQuest.SkandaraQuest);
                break;
            case "Theselene":
                source.Trackers.Enums.Set(typeof(JoinReligionQuest), JoinReligionQuest.TheseleneQuest);
                break;
            case "Serendael":
                source.Trackers.Enums.Set(typeof(JoinReligionQuest), JoinReligionQuest.SerendaelQuest);
                break;
        }
        source.SendActiveMessage($"{deity} wants you to retreive (3) Essence of {deity}.");
    }

    public void HideDialogOptions(Aisling source, string deity, Dialog subject)
    {
        if (!source.Trackers.Enums.TryGetValue(typeof(JoinReligionQuest), out var enumValue))
        {
            enumValue = JoinReligionQuest.None;
        }

        if (enumValue is JoinReligionQuest.None or JoinReligionQuest.MiraelisQuest or JoinReligionQuest.SerendaelQuest or JoinReligionQuest.SkandaraQuest or JoinReligionQuest.TheseleneQuest)
        {
            RemoveOption(subject, "Pray");
            RemoveOption(subject, "Transfer Faith");
            RemoveOption(subject, "Scroll of the Temple");
        }

        if (enumValue is not JoinReligionQuest.JoinReligionComplete && 
            enumValue is not JoinReligionQuest.None)
        {
            RemoveOption(subject, "Join the Temple");

            if (!subject.GetOptionIndex($"Essence of {deity}").HasValue)
            {
                subject.AddOption($"Essence of {deity}", $"{deity}_temple_completejoinQuest");
            }
        }

        if (enumValue is JoinReligionQuest.JoinReligionComplete)
        {
            RemoveOption(subject, "Join the Temple");
        }
    }

    private void RemoveOption(Dialog subject, string optionName)
    {
        if (subject.GetOptionIndex(optionName).HasValue)
        {
            var s = subject.GetOptionIndex(optionName)!.Value;
            subject.Options.RemoveAt(s);
        }
    }
    
    public void CheckJoinQuestCompletion(Aisling source, string deity)
    {
        if (source.Inventory.CountOf($"Essence of {deity}") < 3)
        {
            source.SendActiveMessage($"You do not have the required Essence of {deity}.");

            Subject.Reply(
                source,
                "Though the essence of Miraelis eludes you, seeker, remember that the journey itself holds wisdom. Reflect on your quest, learn from its challenges, and embrace compassion, nature, and intellect in your future endeavors. The path to enlightenment awaits your renewed efforts.");
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
        
        source.Inventory.RemoveQuantity($"Essence of {deity}", 3);
        source.SendActiveMessage($"You have joined the temple of {deity} as a Worshipper!");
    }
    
    public void Pray(Aisling source, string deity)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("PrayerCooldown", out var timedEvent))
        {
            Subject.Reply(source, $"You cannot pray with {deity} at this time. \nTry again in {
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
            UpdateReligionRank(source);
            source.Animate(PrayerSuccess);
            if (IntegerRandomizer.RollChance(5))
            {
                var essence = ItemFactory.Create($"essenceof{deity}");
                source.Inventory.TryAddToNextSlot(essence);
                source.SendActiveMessage($"Through prayer, you receive a Essence of {deity}!");
            }
        } 
        else
        {
            Subject.Reply(source, $"You have reached your limit in prayer. Try again tomorrow!");
            source.Trackers.TimedEvents.AddEvent("PrayerCooldown", TimeSpan.FromDays(1));
        }
    }
    
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

        source.Legend.Remove(deity, out _);
        source.SendActiveMessage($"You turn your back on {deity} and leave the ranks of worship.");
    }
    
    private void UpdateReligionRank(Aisling source)
    {
        var key = CheckDeity(source);
        source.Legend.TryGetValue(key!, out var existingMark);

        if (existingMark is null)
        {
            source.Legend.AddOrAccumulate(
                new LegendMark($"Worshipper of {key}",
                    key!,
                    MarkIcon.Heart,
                    MarkColor.White,
                    1,
                    GameTime.Now));
        }
        
        if (existingMark is not null)
        {
            var faithCount = existingMark.Count;

            existingMark.Text = faithCount switch
            {
                > 1499 when !existingMark.Text.Contains("Champion") => $"Champion of {key}",
                > 999 when !existingMark.Text.Contains("Favor")     => $"Favor of {key}",
                > 749 when !existingMark.Text.Contains("Seer")      => $"Seer of {key}",
                > 499 when !existingMark.Text.Contains("Emissary")  => $"Emissary of {key}",
                > 299 when !existingMark.Text.Contains("Acolyte")  => $"Acolyte of {key}",
                _ when !existingMark.Text.Contains("Novice")        => $"Worshipper of {key}",
                _                                                   => existingMark.Text
            };

            existingMark.Count++;
        }
    }
    
    public Rank GetPlayerRank(Aisling source)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
        {
            var faithCount = faith.Count;

            switch (faithCount)
            {
                case < 150:
                    return Rank.Worshipper;
                case < 300:
                    return Rank.Acolyte;
                case < 500:
                    return Rank.Emissary;
                case < 750:
                    return Rank.Seer;
                case < 1000:
                    return Rank.Favor;
                case < 1500:
                    return Rank.Champion;
            }
        }

        return Rank.Worshipper;
    }
    
    public static string? CheckDeity(Aisling source)
    {
        var deityKeys = new[] {
            MIRAELIS_LEGEND_KEY,
            THESELENE_LEGEND_KEY,
            SERENDAEL_LEGEND_KEY,
            SKANDARA_LEGEND_KEY
        };

        return deityKeys.FirstOrDefault(key => source.Legend.ContainsKey(key));
    }
    
    public int CheckCurrentFaith(Aisling source)
    {
        var key = CheckDeity(source);

        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
            return faith.Count;

        return 0;
    }

    public bool TryTransferFaith(Aisling source, Aisling target)
    {
        var sourceKey = CheckDeity(source);
        var targetKey = CheckDeity(target);
        
        if (sourceKey is not null && targetKey is not null && (sourceKey == targetKey) && source.Legend.TryGetValue(sourceKey, out var faith) && target.Legend.TryGetValue(targetKey, out var targetFaith))
        {
            if (AMOUNT_TO_TRANSFER_FAITH < faith.Count)
            {
                faith.Count -= AMOUNT_TO_TRANSFER_FAITH;
                targetFaith.Count += AMOUNT_TO_TRANSFER_FAITH;
                return true;
            }
        }

        return false;
    }
    
    public bool TrySubtractFaith(Aisling source, int amount)
    {
        var key = CheckDeity(source);
        
        if ((key != null) && source.Legend.TryGetValue(key, out var faith))
        {
            if (amount < faith.Count)
            {
                source.Legend.RemoveCount(key, amount, out _);
                return true;
            }
        }

        return false;
    }
    
    public static bool IsDeityMember(Aisling source, string deity) => source.Legend.ContainsKey(deity);
}