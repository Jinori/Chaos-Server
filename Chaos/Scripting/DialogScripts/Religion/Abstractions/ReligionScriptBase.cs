using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Religion.Abstractions;

public class ReligionScriptBase : DialogScriptBase
{
    protected IClientRegistry<IWorldClient> ClientRegistry { get; }
    private const string MIRAELIS_LEGEND_KEY = "Miraelis";
    private const string THESELENE_LEGEND_KEY = "Theselene";
    private const string SERENDAEL_LEGEND_KEY = "Serendael";
    private const string SKANDARA_LEGEND_KEY = "Skandara";
    
    
    /// <inheritdoc />
    public ReligionScriptBase(Dialog subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject) =>
        ClientRegistry = clientRegistry;

    public enum Rank
    {
        Worshipper,
        Acolyte,
        Emissary,
        Seer,
        Favor,
        Champion
    }

    public void Pray(Aisling source, string deity)
    {
        if (source.Trackers.TimedEvents.HasActiveEvent("PrayerCooldown", out var timedEvent))
        {
            Subject.Reply(source, $"You cannot pray to {deity} at this time. Try again in {
                timedEvent.Remaining.ToReadableString()}");
            
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
            var legendMark = source.Legend.TryGetValue(deity, out var existingMark);

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
        var legendMark = source.Legend.TryGetValue(deity, out var existingMark);
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
        var legendMark = source.Legend.TryGetValue(key!, out var existingMark);

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

    public static bool IsDeityMember(Aisling source, string deity) => source.Legend.ContainsKey(deity);
}