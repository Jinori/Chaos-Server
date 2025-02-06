using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Events.Valentines
{
    public class Valentines(Dialog subject, IEffectFactory effectFactory, IItemFactory itemFactory, IClientRegistry<IChaosWorldClient> clientRegistry) 
        : DialogScriptBase(subject)
    {
        public override void OnDisplaying(Aisling source)
        {
            if (Subject.DialogSource is not Merchant merchant) 
                return;

            switch (Subject.Template.TemplateKey.ToLower())
            {
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
                case "cueti_givering":
                    HandlePromiseRingResponse(source, optionIndex);
                    break;

                case "cueti_makeapromise":
                    HandlePromiseRingCreation(source);
                    break;
            }
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
                Subject.Reply(source, "May your hearts stay warm forevermore! You can make another ring in the next event!");
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
                Subject.Reply(source, "I cannot sense their presence in this land. Are you sure this 'lover' of yours exists?");
                return;
            }

            source.Trackers.Counters.AddOrIncrement($"ValentinesPromise[{partner.Aisling.Name.ToLower()}]");
            var ring = itemFactory.Create("promisering");
            ring.CustomNameOverride = $"{partner.Aisling.Name}'s Promise Ring";
            source.GiveItemOrSendToBank(ring);
            Subject.Reply(source, $"Ah, {partner.Aisling.Name}, such a wonderful choice! I do hope they feel the same way. Here is your Promise Ring. Go use it facing them.");
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
    }
}
