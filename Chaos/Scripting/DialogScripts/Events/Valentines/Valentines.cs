using Chaos.Collections;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Storage.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Events.Valentines;

public class Valentines(
    Dialog subject,
    IEffectFactory effectFactory,
    IItemFactory itemFactory,
    IClientRegistry<IChaosWorldClient> clientRegistry,
    IShardGenerator shardGenerator,
    ISimpleCache simpleCache) : DialogScriptBase(subject)
{
    public static Dictionary<int, Riddle> Riddles = new()
    {
        {
            1,
            new Riddle(
                "Though I'm struck, I do not bleed. My wound's unseen, yet hearts concede. I make the mighty weak in stride, when longing takes them for a ride.",
                [
                    "An Arrow",
                    "Love",
                    "A Sword"
                ],
                "Love")
        },

        {
            2,
            new Riddle(
                "I have a twin but we never meet. Though close at heart, we'll never greet. We work as one, yet stand apart. We race through life, a mirrored art.",
                [
                    "A Reflection",
                    "A Wedding Ring",
                    "Shoes"
                ],
                "A Reflection")
        },

        {
            3,
            new Riddle(
                "I'm caught but never thrown, though some let me slip. I'm silent, yet cause cries. Some give me freely, others never try.",
                [
                    "A Whisper",
                    "A Kiss",
                    "A Tear"
                ],
                "A Kiss")
        },

        {
            4,
            new Riddle(
                "I can be made but not seen, whispered yet never heard. I carry weight but never touch, binding souls with just a word.",
                [
                    "A Secret",
                    "A Promise",
                    "A Wish"
                ],
                "A Promise")
        },

        {
            5, new Riddle(
                "I exist between two souls yet I am neither. I am felt but never held. If true, I last forever.",
                [
                    "A Dream",
                    "Love",
                    "A Memory"
                ],
                "Love")
        },

        {
            6,
            new Riddle(
                "I am not a mirror, but I reflect. I make two one, yet stay intact. I show the past, present, and what's to come. Who am I?",
                [
                    "A Photograph",
                    "A Shadow",
                    "A Memory"
                ],
                "A Photograph")
        },

        {
            7,
            new Riddle(
                "I can be wrapped but I'm not a gift. I can be tight but I'm not a fist. I bring comfort when given right. What am I?",
                [
                    "A Hug",
                    "A Scarf",
                    "A Ribbon"
                ],
                "A Hug")
        },

        {
            8, new Riddle(
                "I am heavy yet I float. I am silent yet I scream. I am invisible yet felt by all. What am I?",
                [
                    "A Dream",
                    "A Memory",
                    "A Heart"
                ],
                "A Heart")
        },

        {
            9, new Riddle(
                "The more you take, the more you leave behind. But in love, the ones you make stay in mind.",
                [
                    "A Kiss",
                    "A Memory",
                    "A Footstep"
                ],
                "A Memory")
        },

        {
            10,
            new Riddle(
                "I am full when I start, but I vanish by the end. I bring joy, yet sometimes pain, and memories I help suspend.",
                [
                    "A Letter",
                    "A Candle",
                    "A Love Song"
                ],
                "A Candle")
        },

        {
            11,
            new Riddle(
                "I can be spoken, yet I make no sound. I bind two hearts, yet I am not a rope. Once made, I should never be broken. What am I?",
                [
                    "A Promise",
                    "A Vow",
                    "A Secret"
                ],
                "A Vow")
        },

        {
            12,
            new Riddle(
                "I am soft yet strong, silent yet loud. I stand in a field but never move. Though I fade, I'm never forgotten. What am I?",
                [
                    "A Rose",
                    "A Letter",
                    "A Shadow"
                ],
                "A Rose")
        },

        {
            13,
            new Riddle(
                "I grow but have no roots. I can be shared but not held. The more I give, the more I have. What am I?",
                [
                    "A Smile",
                    "Love",
                    "A Secret"
                ],
                "Love")
        },

        {
            14,
            new Riddle(
                "I can be found on a face but I'm not a nose. I can be a secret, but everyone knows. I can be stolen or freely given. What am I?",
                [
                    "A Kiss",
                    "A Wink",
                    "A Whisper"
                ],
                "A Kiss")
        },

        {
            15,
            new Riddle(
                "I begin empty but fill with delight. I hold treasures both big and small. I'm often opened with eager hands. What am I?",
                [
                    "A Love Letter",
                    "A Box of Chocolates",
                    "A Heart"
                ],
                "A Box of Chocolates")
        },

        {
            16,
            new Riddle(
                "I have many layers but I am not an onion. I stand tall in celebration, yet I disappear bite by bite. What am I?",
                [
                    "A Wedding Cake",
                    "A Love Story",
                    "A Rose"
                ],
                "A Wedding Cake")
        },

        {
            17,
            new Riddle(
                "I am a journey, not a place. I require two but can be lost by one. I grow stronger with time but fade if neglected. What am I?",
                [
                    "Marriage",
                    "A Dance",
                    "Trust"
                ],
                "Marriage")
        },

        {
            18,
            new Riddle(
                "I am a circle with no beginning or end. I am a promise made with metal and heart. I can be given but never owned alone. What am I?",
                [
                    "A Necklace",
                    "A Wedding Ring",
                    "A Coin"
                ],
                "A Wedding Ring")
        },

        {
            19,
            new Riddle(
                "I am invisible but I can be felt. I can be lost but never stolen. I live in the heart and grow with time. What am I?",
                [
                    "A Memory",
                    "A Promise",
                    "Love"
                ],
                "A Memory")
        },

        {
            20,
            new Riddle(
                "I have hands but do not hold. I mark the moments that make love bold. I move forward but never back. What am I?",
                [
                    "A Calendar",
                    "A Clock",
                    "A Sundial"
                ],
                "A Clock")
        }
    };

    private void AcceptPromiseRing(Aisling source)
    {
        var text = Subject.Text;
        const string DELIMITER = " is offering you a Promise Ring";

        var name
            = text.Contains(DELIMITER)
                ? text.Substring(0, text.IndexOf(DELIMITER, StringComparison.Ordinal))
                : "Unknown"; // Fallback if the format is incorrect

        var player = clientRegistry.Select(c => c.Aisling)
                                   .FirstOrDefault(a => a.Name.EqualsI(name));

        if (player == null)
            return;

        var ring = itemFactory.Create("lovering");
        ring.CustomNameOverride = $"{name}'s Love Ring";
        source.GiveItemOrSendToBank(ring);
        source.SendOrangeBarMessage($"You've obtained {ring.CustomNameOverride}! A symbol of love.");

        player.Inventory.Remove($"{source.Name}'s Promise Ring");

        player.Legend.AddOrAccumulate(
            new LegendMark(
                $"Gave a Love Ring to {source.Name}",
                "loveRing",
                MarkIcon.Heart,
                MarkColor.Pink,
                1,
                GameTime.Now));

        ApplyMarriageEffects(source, player);
    }

    private void ApplyMarriageEffects(Aisling source, Aisling target)
    {
        var effect = effectFactory.Create("marriage");
        var effect2 = effectFactory.Create("marriage");
        source.Effects.Apply(source, effect);
        target.Effects.Apply(target, effect2);
    }

    private static void DeclinePromiseRing(Aisling source)
    {
        var targetPlayer = source.MapInstance
                                 .GetEntitiesWithinRange<Aisling>(source, 1)
                                 .FirstOrDefault(x => x.Trackers.Counters.ContainsKey($"ValentinesPromise[{source.Name.ToLower()}]"));

        if (targetPlayer == null)
            return;

        targetPlayer.Inventory.Remove($"{source.Name.ToLower()}'s Promise Ring");
        source.Say($"I'm sorry, I cannot accept this, {targetPlayer.Name}.");

        targetPlayer.SendServerMessage(
            ServerMessageType.ScrollWindow,
            "Your lover has declined your promise ring. You can return to Cueti to try another love.");
        targetPlayer.Trackers.Counters.Remove($"ValentinesPromise[{source.Name.ToLower()}]", out _);
    }

    private void HandleCuetiInitial(Aisling source)
    {
        if (source.UserStatSheet.Level < 11)
        {
            Subject.Reply(source, "You must be at least level 11 to participate in this event.");

            return;
        }

        var keysToRemove = source.Trackers
                                 .Counters
                                 .Where(key => key.Key.StartsWith("ValentinesPromise[", StringComparison.OrdinalIgnoreCase))
                                 .ToList();

        if (keysToRemove.Count != 0)
            Subject.Reply(source, "May your hearts stay warm forevermore! You can make another ring in the next event!");
    }

    private void HandleEventCandy(Aisling source, Merchant merchant)
    {
        if (!EventPeriod.IsEventActive(DateTime.UtcNow, merchant.MapInstance.InstanceId))
        {
            Subject.Reply(source, "Looks like I'm all out of candies..");

            return;
        }

        if (source.Trackers.TimedEvents.HasActiveEvent("VdayBonus", out _))
        {
            Subject.Reply(source, "I already gave you candy for this event.");

            return;
        }

        var effect = effectFactory.Create("ValentinesCandy");
        source.SendOrangeBarMessage("Nadia stuffs a chocolate in your face. Knowledge rates increased!");
        source.Effects.Apply(source, effect);
        source.Trackers.TimedEvents.AddEvent("VdayBonus", TimeSpan.FromDays(30), true);
    }

    private void HandleFlourentineInitial(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out FlourentineQuest queststate);

        if (queststate is FlourentineQuest.FinishedQuest)
        {
            Subject.Reply(source, "Thanks for helping get the True Love Essence back. I hope you find your true love!");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithFlourentineFirst)
        {
            Subject.Reply(source, "I see you've returned. Did you catch Snaggles the Sweetsnatcher?", "flourentine_aftersnaggles");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithFlourentineAfterZappy)
        {
            Subject.Reply(source, "I see you've returned. Huh, you spoke with Zappy?", "flourentine_afterzappy");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithZappy)
        {
            Subject.Reply(source, "I heard Zappy may have had something to do with this.");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithSnaggles)
        {
            Subject.Reply(source, "So it wasn't Tricksy? I see, I see. Have you spoken with Snaggles the Sweetsnatcher yet?");

            return;
        }

        if (queststate != FlourentineQuest.SpeakWithJester)
            return;

        Subject.Reply(source, "Have you spoke with Tricksy Lovefool yet?");
    }

    private void HandleItemExchange(
        Aisling source,
        string item1,
        int quantity1,
        string item2,
        int quantity2,
        int goldRequired,
        string rewardItem)
    {
        if (!source.Inventory.HasCountByTemplateKey(item1, quantity1)
            || !source.Inventory.HasCountByTemplateKey(item2, quantity2)
            || (source.Gold < goldRequired))
        {
            Subject.Reply(source, "You don't have the required items or gold.");

            return;
        }

        if (!source.TryTakeGold(goldRequired))
        {
            Subject.Reply(source, $"You don't have enough gold. I need {goldRequired:N0} gold.");

            return;
        }

        source.Inventory.RemoveQuantityByTemplateKey(item1, quantity1);
        source.Inventory.RemoveQuantityByTemplateKey(item2, quantity2);
        var item = itemFactory.Create(rewardItem);
        source.GiveItemOrSendToBank(item);
    }

    private void HandlePromiseRingCreation(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var partner = clientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name));

        if (partner == null)
        {
            Subject.Reply(source, "I cannot sense their presence in this land. Are you sure this 'lover' of yours exists?");

            return;
        }

        if (partner.Aisling.Name == source.Name)
        {
            Subject.Reply(source, "You cannot promise a ring to yourself.");

            return;
        }

        source.Trackers.Counters.AddOrIncrement($"ValentinesPromise[{partner.Aisling.Name.ToLower()}]");
        var ring = itemFactory.Create("promisering");
        ring.CustomNameOverride = $"{partner.Aisling.Name}'s Promise Ring";
        source.GiveItemOrSendToBank(ring);

        Subject.Reply(
            source,
            $"Ah, {partner.Aisling.Name}, such a wonderful choice! I do hope they feel the same way. Here is your Promise Ring. Go use it facing them.");
    }

    private void HandlePromiseRingResponse(Aisling source, byte? optionIndex)
    {
        if (optionIndex == null)
            return;

        switch (optionIndex)
        {
            case 1:
                AcceptPromiseRing(source);

                break;

            case 2:
                DeclinePromiseRing(source);

                break;
        }
    }

    private void HandleRewards(Aisling source, bool romantic)
    {
        if (romantic)
        {
            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Loures Love Heist Romantic",
                    "louresheistroman",
                    MarkIcon.Heart,
                    MarkColor.Pink,
                    1,
                    GameTime.Now));
            var item = itemFactory.Create("blackcape");
            source.GiveItemOrSendToBank(item);
            source.SendOrangeBarMessage("You've received a unique item and legend mark!");
            source.Trackers.Enums.Set(FlourentineQuest.FinishedQuest);
        } else
        {
            source.Legend.AddOrAccumulate(
                new LegendMark(
                    "Loures Love Heist Pragmatic",
                    "louresheistprag",
                    MarkIcon.Victory,
                    MarkColor.LightGreen,
                    1,
                    GameTime.Now));
            var item = itemFactory.Create("royalcloak");
            source.GiveItemOrSendToBank(item);
            source.SendOrangeBarMessage("You've received a unique item and legend mark!");
            source.Trackers.Enums.Set(FlourentineQuest.FinishedQuest);
        }
    }

    private void HandleRiddleAnswers(Aisling source, byte? optionIndex)
    {
        if (optionIndex is null or < 1 or > 3)
            return;

        var questionText = Subject.Text.Trim();

        var selectedRiddle = Riddles.Values.FirstOrDefault(r => r.Question == questionText);

        if (selectedRiddle == null)
        {
            Subject.Reply(source, "Hmm... I can't seem to recall that riddle. Tricksy Lovefool must be messing with us!");

            return;
        }

        var correctAnswer = selectedRiddle.Answer;
        var adjustedIndex = (int)optionIndex - 1;
        var selectedAnswer = Subject.Options[adjustedIndex].OptionText;

        if (selectedAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
        {
            source.Trackers.Counters.AddOrIncrement("ValentinesRiddles");
            source.Trackers.Counters.TryGetValue("ValentinesRiddles", out var value);

            Subject.Reply(
                source,
                $"Correct! Tricksy Lovefool scowls, defeated. You've answered {value.ToWords()} of three correct.",
                "tricksylovefool_riddle1");
        } else
        {
            source.Trackers.Counters.TryGetValue("ValentinesRiddles", out var value);

            Subject.Reply(
                source,
                $"Wrong! Tricksy Lovefool bursts into laughter at your blunder. You've answered {value.ToWords()} of three correct.",
                "tricksylovefool_riddle1");
        }
    }

    private void HandleRiddles(Aisling source)
    {
        if (source.Trackers.Counters.TryGetValue("ValentinesRiddles", out var value))
            if (value >= 3)
            {
                Subject.Reply(
                    source,
                    "The mark of true intelligence. Yes, yes, I did take a tiny piece for a taste test. Purely for quality control, you see! But it was the greedy goblin Snaggles who snatched the whole batch!",
                    "tricksylovefool_snaggle");

                return;
            }

        var random = IntegerRandomizer.RollSingle(20);
        var selectedRiddle = Riddles[random];

        Subject.Options.Clear();
        Subject.InjectTextParameters(selectedRiddle.Question);

        var shuffledOptions = selectedRiddle.Options.ToList();

        for (var i = shuffledOptions.Count - 1; i > 0; i--)
        {
            var swapIndex = Random.Shared.Next(i + 1);
            (shuffledOptions[i], shuffledOptions[swapIndex]) = (shuffledOptions[swapIndex], shuffledOptions[i]);
        }

        foreach (var option in shuffledOptions)
        {
            var opt = new DialogOption
            {
                OptionText = option,
                DialogKey = "tricksylovefool_riddleanswer"
            };
            Subject.Options.Add(opt);
        }
    }

    private void HandleSendToFlourentineAfterZappy(Aisling source)
    {
        source.Trackers.Enums.Set(FlourentineQuest.SpeakWithFlourentineAfterZappy);
        source.SendOrangeBarMessage("You should head back to Flourentine with the True Love Essence!");
    }

    private void HandleSendToFlourentineFirst(Aisling source)
    {
        var item = itemFactory.Create("jaroftrueloveessence");
        source.GiveItemOrSendToBank(item);
        source.Trackers.Enums.Set(FlourentineQuest.SpeakWithFlourentineFirst);
        source.SendOrangeBarMessage("You should head back to Flourentine!");
        var mapInstance = simpleCache.Get<MapInstance>("loures_1_floor_restaurant");
        source.TraverseMap(mapInstance, new Point(5, 5));
    }

    private void HandleSendToJester(Aisling source)
    {
        source.Trackers.Enums.Set(FlourentineQuest.SpeakWithJester);
        source.SendOrangeBarMessage("You've been tasked to speak with Tricksy Lovefool.");
    }

    private void HandleSendToSnaggles(Aisling source)
    {
        source.Trackers.Enums.Set(FlourentineQuest.SpeakWithSnaggles);
        source.SendOrangeBarMessage("You've been tasked to go find Snaggles the Sweetsnatcher!");
    }

    private void HandleSendToZappy(Aisling source)
    {
        var item = itemFactory.Create("jaroftrueloveessence");
        source.GiveItemOrSendToBank(item);
        source.Trackers.Enums.Set(FlourentineQuest.SpeakWithZappy);
        source.SendOrangeBarMessage("You've been tasked to take the True Love Essence to Zappy first!");
        var mapInstance = simpleCache.Get<MapInstance>("loures_1_floor_restaurant");
        source.TraverseMap(mapInstance, new Point(5, 5));
    }

    private void HandleSnagglePortal(Aisling source, byte? optionIndex)
    {
        switch (optionIndex)
        {
            case 1:
            {
                var shard = shardGenerator.CreateShardOfInstance("SnagglesChallenge");
                shard.Shards.TryAdd(shard.InstanceId, shard);

                if (source.Group is null)
                {
                    source.Trackers.Enums.TryGetValue(out FlourentineQuest queststate);

                    if (queststate is not FlourentineQuest.SpeakWithSnaggles)
                    {
                        source.SendOrangeBarMessage("You're not on this part of the quest yet.");

                        return;
                    }

                    if (source.Effects.Contains("Mount"))
                        source.Effects.Dispel("Mount");

                    source.SendOrangeBarMessage("Catch Snaggles the Sweetsnatcher!");
                    source.TraverseMap(shard, new Point(21, 12));
                    Subject.Close(source);
                } else
                {
                    var allOnSnaggles = source.Group.All(
                        x => x.Trackers.Enums.TryGetValue(out FlourentineQuest queststate)
                             && (queststate == FlourentineQuest.SpeakWithSnaggles));

                    if (!allOnSnaggles) // If NOT all members are on SpeakWithSnaggles
                    {
                        source.SendOrangeBarMessage("Someone in the group is not on this part of the quest yet.");
                        Subject.Close(source);

                        return;
                    }

                    if (source.Group.Count > 3)
                    {
                        source.SendOrangeBarMessage("Only three members can be in your group for this part of the quest.");
                        Subject.Close(source);

                        return;
                    }

                    foreach (var member in source.Group)
                    {
                        if (member.Effects.Contains("Mount"))
                            member.Effects.Dispel("Mount");

                        member.SendOrangeBarMessage("Catch Snaggles the Sweetsnatcher!");
                        member.TraverseMap(shard, new Point(21, 12));
                        Subject.Close(source);
                    }
                }

                break;
            }
            case 2:
            {
                // Send a message to the Aisling
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "Return when you are ready to fight the Sweetsnatcher!");

                // Warp the source back
                var point = source.DirectionalOffset(source.Direction.Reverse());
                source.WarpTo(point);

                break;
            }
        }
    }

    private void HandleTricksyInitial(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out FlourentineQuest queststate);

        if (queststate is FlourentineQuest.None)
        {
            Subject.Reply(source, "I hear Flourentine has been looking for you. She's in the Loures Restaurant.");

            return;
        }

        if (queststate is FlourentineQuest.FinishedQuest)
        {
            Subject.Reply(source, "Thanks for helping get the True Love Essence back. I hope you find your true love!");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithZappy)
        {
            Subject.Reply(source, "I heard Zappy may have had something to do with this.");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithFlourentineFirst)
        {
            Subject.Reply(source, "I heard Flourentine was looking for you.");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithFlourentineAfterZappy)
        {
            Subject.Reply(source, "I heard Flourentine was looking for you.");

            return;
        }

        if (queststate != FlourentineQuest.SpeakWithSnaggles)
            return;

        Subject.Reply(source, "Have you snatched up the sweetsnatcher yet?");
    }

    private void HandleZappyInitial(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out FlourentineQuest queststate);

        if (queststate is FlourentineQuest.None)
        {
            Subject.Reply(source, "I hear Flourentine has been looking for you. She's in the Loures Restaurant.");

            return;
        }

        if (queststate is FlourentineQuest.FinishedQuest)
        {
            Subject.Reply(source, "He's so dreamy... I mean, what can I do for you?");

            return;
        }

        if (queststate is FlourentineQuest.SpeakWithSnaggles)
            Subject.Reply(source, "Someone said you're looking for Snaggles the Sweetsnatcher. He's in the Loures Restaurant, Floor 1.");

        if (queststate is FlourentineQuest.SpeakWithFlourentineAfterZappy)
            Subject.Reply(source, "He's so dreamy... I mean, what can I do for you?");
    }

    public override void OnDisplaying(Aisling source)
    {
        if (Subject.DialogSource is not Merchant merchant)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "flourentine_afterzappy_explain2":
                HandleRewards(source, true);

                break;

            case "flourentine_aftersnaggles_explain2":
                HandleRewards(source, false);

                break;

            case "zappy_explain4":
                HandleSendToFlourentineAfterZappy(source);

                break;

            case "zappy_initial":
                HandleZappyInitial(source);

                break;

            case "snagglesthesweetsnatcher_explain7":
                HandleSendToFlourentineFirst(source);

                break;

            case "snagglesthesweetsnatcher_explain6":
                HandleSendToZappy(source);

                break;

            case "tricksylovefool_initial":
                HandleTricksyInitial(source);

                break;

            case "tricksylovefool_finish":
                HandleSendToSnaggles(source);

                break;

            case "tricksylovefool_riddle1":
                HandleRiddles(source);

                break;

            case "flourentine_initial":
                HandleFlourentineInitial(source);

                break;

            case "flourentine_sendtojester":
                HandleSendToJester(source);

                break;

            case "cueti_initial":
                HandleCuetiInitial(source);

                break;

            case "aidan_polyppurplemake":
                HandleItemExchange(
                    source,
                    "polypsac",
                    3,
                    "grape",
                    30,
                    5000000,
                    "purpleheartpuppet");

                break;

            case "aidan_polypredmake":
                HandleItemExchange(
                    source,
                    "polypsac",
                    3,
                    "cherry",
                    30,
                    5000000,
                    "redheartpuppet");

                break;

            case "nadia_confirm":
                HandleEventCandy(source, merchant);

                break;
        }
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "snagglesthesweetsnatcher_portal":
                HandleSnagglePortal(source, optionIndex);

                break;

            case "tricksylovefool_riddle1":
                HandleRiddleAnswers(source, optionIndex);

                break;

            case "cueti_givering":
                HandlePromiseRingResponse(source, optionIndex);

                break;

            case "cueti_makeapromise":
                HandlePromiseRingCreation(source);

                break;
        }
    }

    public class Riddle(string question, List<string> options, string answer)
    {
        public string Answer { get; } = answer;
        public List<string> Options { get; } = options;
        public string Question { get; } = question;
    }
}