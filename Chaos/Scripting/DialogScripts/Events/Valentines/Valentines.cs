using Chaos.Collections;
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

namespace Chaos.Scripting.DialogScripts.Events.Valentines
{
    public class Valentines(
        Dialog subject,
        IEffectFactory effectFactory,
        IItemFactory itemFactory,
        IClientRegistry<IChaosWorldClient> clientRegistry, IShardGenerator shardGenerator, ISimpleCache simpleCache)
        : DialogScriptBase(subject)
    {
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
                    HandleItemExchange(source, "polypsac", 3, "grape", 30, 5000000, "purpleheartpuppet");
                    break;

                case "aidan_polypredmake":
                    HandleItemExchange(source, "polypsac", 3, "cherry", 30, 5000000, "redheartpuppet");
                    break;

                case "nadia_initial":
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

        
        private void HandleRewards(Aisling source, bool romantic)
        {
            if (romantic)
            {
                source.Legend.AddOrAccumulate(new LegendMark(
                    "Loures Love Heist Romantic",
                    "louresheistromance", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now));
                var item = itemFactory.Create("blackcape");
                source.GiveItemOrSendToBank(item);
                source.SendOrangeBarMessage("You've been a unique item and legend mark!");
                source.Trackers.Enums.Set(FlourentineQuest.FinishedQuest);
            }
            else
            {
                source.Legend.AddOrAccumulate(new LegendMark(
                    "Loures Love Heist Pragmatic",
                    "louresheistromance", MarkIcon.Victory, MarkColor.Pink, 1, GameTime.Now));
                var item = itemFactory.Create("royalcloak");
                source.GiveItemOrSendToBank(item);
                source.SendOrangeBarMessage("You've been a unique item and legend mark!");
                source.Trackers.Enums.Set(FlourentineQuest.FinishedQuest);
            }
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
            {
                Subject.Reply(source, "Someone said you're looking for Snaggles the Sweetsnatcher. He's in the Loures Restaurant, Floor 1.");
            }
            
            if (queststate is FlourentineQuest.SpeakWithFlourentineAfterZappy)
            {
                Subject.Reply(source, "He's so dreamy... I mean, what can I do for you?");
            }
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
                            source.SendOrangeBarMessage($"You're not on this part of the quest yet.");
                            return;
                        }
                        
                        source.SendOrangeBarMessage($"Catch Snaggles the Sweetsnatcher!");
                        source.TraverseMap(shard, new Point(21, 12));
                        Subject.Close(source);
                    }
                    else
                    {
                        
                        var allOnSnaggles = source.Group.Any(x => x.Trackers.Enums.TryGetValue(out FlourentineQuest queststate) && queststate != FlourentineQuest.SpeakWithSnaggles);
                        
                        if (allOnSnaggles)
                        {
                            source.SendOrangeBarMessage($"Someone in the group is not on this part of the quest yet.");
                            Subject.Close(source);
                            return;
                        }
                        
                        foreach (var member in source.Group)
                        {
                            member.SendOrangeBarMessage($"Catch Snaggles the Sweetsnatcher!");
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
                Subject.Reply(source, "Correct! Tricksy Lovefool scowls, defeated.", "tricksylovefool_riddle1");
                source.Trackers.Counters.AddOrIncrement("ValentinesRiddles");
            }
            else
            {
                Subject.Reply(source, "Wrong! Tricksy Lovefool bursts into laughter at your blunder.", "tricksylovefool_riddle1");
            }
        }


        
        private void HandleRiddles(Aisling source)
        {
            if (source.Trackers.Counters.TryGetValue("ValentinesRiddles", out var value))
            {
                if (value >= 3)
                {
                    Subject.Reply(source, "The mark of true intelligence. Yes, yes, I did take a tiny piece for a taste test. Purely for quality control, you see! But it was the greedy goblin Snaggles who snatched the whole batch!", "tricksylovefool_snaggle");
                    return;
                }
            }
            var rnd = new Random();
            var randomKey = rnd.Next(1, Riddles.Count + 1);
            var selectedRiddle = Riddles[randomKey];
            Subject.Options.Clear();
            
            Subject.InjectTextParameters(selectedRiddle.Question);
            foreach (var option in selectedRiddle.Options)
            {
                var opt = new DialogOption()
                {
                    OptionText = option,
                    DialogKey = "tricksylovefool_riddleanswer"
                };
                Subject.Options.Add(opt);
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

        private void HandleCuetiInitial(Aisling source)
        {
            if (source.UserStatSheet.Level < 11)
            {
                Subject.Reply(source, "You must be at least level 11 to participate in this event.");
                return;
            }

            var keysToRemove = source.Trackers.Counters
                .Where(key => key.Key.StartsWith("ValentinesPromise[", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (keysToRemove.Count != 0)
            {
                Subject.Reply(source,
                    "May your hearts stay warm forevermore! You can make another ring in the next event!");
            }
        }

        private void HandlePromiseRingResponse(Aisling source, byte? optionIndex)
        {
            if (optionIndex == null) return;

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

        private void AcceptPromiseRing(Aisling source)
        {
            var text = Subject.Text;
            const string DELIMITER = " is offering you a Promise Ring";
            var name = text.Contains(DELIMITER)
                ? text.Substring(0, text.IndexOf(DELIMITER, StringComparison.Ordinal))
                : "Unknown"; // Fallback if the format is incorrect

            var player = clientRegistry
                .Select(c => c.Aisling)
                .FirstOrDefault(a => a.Name.EqualsI(name));

            if (player == null)
                return;

            var ring = itemFactory.Create("lovering");
            ring.CustomNameOverride = $"{name}'s Love Ring";
            source.GiveItemOrSendToBank(ring);
            source.SendOrangeBarMessage($"You've obtained {ring.CustomNameOverride}! A symbol of love.");

            player.Inventory.Remove($"{source.Name}'s Promise Ring");
            player.Legend.AddOrAccumulate(new LegendMark(
                $"Gave a Love Ring to {source.Name}",
                "loveRing", MarkIcon.Heart, MarkColor.Pink, 1, GameTime.Now));

            ApplyMarriageEffects(source, player);
        }

        private static void DeclinePromiseRing(Aisling source)
        {
            var targetPlayer = source.MapInstance.GetEntitiesWithinRange<Aisling>(source, 1)
                .FirstOrDefault(x => x.Trackers.Counters.ContainsKey($"ValentinesPromise[{source.Name.ToLower()}]"));

            if (targetPlayer == null) return;

            targetPlayer.Inventory.Remove($"{source.Name.ToLower()}'s Promise Ring");
            source.Say($"I'm sorry, I cannot accept this, {targetPlayer.Name}.");
            targetPlayer.SendServerMessage(ServerMessageType.ScrollWindow,
                "Your lover has declined your promise ring. You can return to Cueti to try another love.");
            targetPlayer.Trackers.Counters.Remove($"ValentinesPromise[{source.Name.ToLower()}]", out _);
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
                Subject.Reply(source,
                    "I cannot sense their presence in this land. Are you sure this 'lover' of yours exists?");
                return;
            }

            source.Trackers.Counters.AddOrIncrement($"ValentinesPromise[{partner.Aisling.Name.ToLower()}]");
            var ring = itemFactory.Create("promisering");
            ring.CustomNameOverride = $"{partner.Aisling.Name}'s Promise Ring";
            source.GiveItemOrSendToBank(ring);
            Subject.Reply(source,
                $"Ah, {partner.Aisling.Name}, such a wonderful choice! I do hope they feel the same way. Here is your Promise Ring. Go use it facing them.");
        }

        private void HandleItemExchange(Aisling source, string item1, int quantity1,
            string item2, int quantity2, int goldRequired, string rewardItem)
        {
            if (!source.Inventory.HasCountByTemplateKey(item1, quantity1) ||
                !source.Inventory.HasCountByTemplateKey(item2, quantity2) ||
                source.Gold < goldRequired)
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

        private void ApplyMarriageEffects(Aisling source, Aisling target)
        {
            var effect = effectFactory.Create("marriage");
            var effect2 = effectFactory.Create("marriage");
            source.Effects.Apply(source, effect);
            target.Effects.Apply(target, effect2);
        }

        public static Dictionary<int, Riddle> Riddles = new()
        {
            {
                1, new Riddle(
                    "I begin as bitter, yet end up sweet. Melt me too much, and I admit defeat. I can be dark, or light as snow. On Valentine's Day, I steal the show!",
                    ["Vanilla", "Caramel", "Chocolate"], "Chocolate")
            },

            {
                2, new Riddle(
                    "I have no eyes, yet I can cry. I have no breath, yet I will sigh. I'm not a drum, but still I beat. When given away, I feel complete.",
                    ["A Drum", "A Whisper", "A Heart"], "A Heart")
            },

            {
                3, new Riddle(
                    "I wear a coat but have no feet. I can be hard or soft to eat. I come in pairs, in shapes, in swirls. I'm often gifted to boys and girls.",
                    ["An Apple", "A Lollipop", "Candy"], "Candy")
            },

            {
                4, new Riddle(
                    "I'm red but I'm not a rose. I'm sweet but I'm not sugar. I have seeds but I'm not an apple. What am I?",
                    ["A Cherry", "A Tomato", "Strawberry"], "Strawberry")
            },

            {
                5, new Riddle(
                    "I hold messages but I am not a phone. I am sealed with love and sent from home. What am I?",
                    ["A Scroll", "A Postcard", "A Love Letter"], "A Love Letter")
            },

            {
                6, new Riddle(
                    "I can be twisted, I can be tied, in loves embrace, I stand with pride. What am I?",
                    ["A Knot", "A Bowtie", "A Ribbon"], "A Ribbon")
            },

            {
                7, new Riddle(
                    "I grow shorter as I shine bright, bringing warmth and soft delight. Lovers light me on special days. What am I?",
                    ["A Lantern", "A Sparkler", "A Candle"], "A Candle")
            },

            {
                8, new Riddle(
                    "I have wings but I don't fly. I have a bow, but I don't tie. I strike hearts and make them swoon. Who am I?",
                    ["An Archer", "A Cherub", "Cupid"], "Cupid")
            },

            {
                9, new Riddle(
                    "I can be given, I can be stolen. I can be broken, yet I stay whole. What am I?",
                    ["A Promise", "A Hug", "A Kiss"], "A Kiss")
            },

            {
                10, new Riddle(
                    "I am round, yet not a ball. I shine bright, yet I'm not the sun. I am often placed upon the hand of one. What am I?",
                    ["A Coin", "A Bracelet", "A Ring"], "A Ring")
            },

            {
                11, new Riddle(
                    "I am written and spoken, yet never heard. I can bind two people without a word. What am I?",
                    ["A Contract", "A Poem", "A Vow"], "A Vow")
            },

            {
                12, new Riddle(
                    "I am soft, but I'm not a pillow. I have petals, but I'm not a tree. I have thorns, but I don't bite. What am I?",
                    ["A Tulip", "A Daisy", "A Rose"], "A Rose")
            },

            {
                13, new Riddle(
                    "I go up but never down. I'm counted once a year but never found. What am I?",
                    ["The Moon", "A Clock Hand", "Age"], "Age")
            },

            {
                14, new Riddle(
                    "I can be cracked, made, told, and played. But when I'm shared, love is displayed. What am I?",
                    ["A Secret", "A Story", "A Joke"], "A Joke")
            },

            {
                15, new Riddle(
                    "I come in a box but I am not a toy. I can be filled with sweetness and bring joy. What am I?",
                    ["A Treasure Chest", "A Jewelry Box", "A Box of Chocolates"],
                    "A Box of Chocolates")
            },

            {
                16, new Riddle(
                    "Though I start small, I can grow tall. When love is true, I stand above all. What am I?",
                    ["A Wedding Cake", "A Love Letter", "A Rosebush"], "A Wedding Cake")
            },

            {
                17, new Riddle(
                    "You'll find me where hearts are true, I tie the knot for more than two. What am I?",
                    ["A Love Triangle", "A Wedding Ring", "A Marriage"], "A Marriage")
            },

            {
                18, new Riddle(
                    "I can be gold, silver, or lace. I often bring a smile to a face. What am I?",
                    ["A Crown", "A Necklace", "Jewelry"], "Jewelry")
            },

            {
                19, new Riddle(
                    "I can be shared but I'm not a meal. I can be stolen but I'm not a jewel. What am I?",
                    ["A Story", "A Memory", "Love"], "Love")
            },

            {
                20, new Riddle(
                    "I have hands but no arms. I count moments that make hearts warm. What am I?",
                    ["A Sundial", "A Calendar", "A Clock"], "A Clock")
            }
        };

        
        public class Riddle(string question, List<string> options, string answer)
        {
            public string Question { get; } = question;
            public List<string> Options { get; } = options;
            public string Answer { get; } = answer;
        }
    }
}
