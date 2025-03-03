using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components.AbilityComponents
{
    public struct PeekAbilityComponent : IComponent
    {

        /// <inheritdoc />
        public void Execute(ActivationContext context, ComponentVars vars)
        {
            var options = vars.GetOptions<IShowPeekOptions>();

            if (string.IsNullOrEmpty(options.DialogKey))
                return;

            var targets = vars.GetTargets<Aisling>();

            foreach (var target in targets)
            {
                if (target.Name == context.SourceAisling.Name)
                    continue;

                context.SourceAisling.DialogHistory.Clear();

                var merchantType = context.SourceAisling.Gender == Gender.Male ? "fearlessmale" : "fearless";
                var merch = options.MerchantFactory.Create(merchantType, target.MapInstance, target);

                merch.Name = $"{target.Name}'s Inventory";
                
                var rogueDialogues = new List<string>
                {
                    "Let's see what {PlayerName} is keeping under lock and key...",
                    "A little peek never hurt anyone... unless they catch me.",
                    "What's yours is mine... at least for a glance, {PlayerName}.",
                    "Secrets, treasures, and trinkets... oh my! What do we have here?",
                    "Shh... just taking inventory. No need to alert the guards!",
                    "A rogue's curiosity knows no bounds. What's in {PlayerName}'s pockets?",
                    "One quick look won't hurt... as long as {PlayerName} doesn't notice.",
                    "I've got nimble fingers, but today, just my eyes will do.",
                    "Every great thief starts by knowing their mark. Let's see what {PlayerName} carries.",
                    "A little reconnaissance never hurt. Well, almost never...",
                    "Ooo, let's play 'What's in {PlayerName}'s pockets?'!",
                    "If I were {PlayerName}, what shiny things would I hoard?",
                    "No harm in a little window shopping, right?",
                    "A peek, a glance, a rogue's delight! What's {PlayerName} hiding?",
                    "I promise, I'll put everything back. Probably.",
                    "They say curiosity killed the cat, but I'm no cat, am I?",
                    "A rogue's gotta know the competition! Let's see what {PlayerName} has.",
                    "Consider this a security check... for research purposes, of course.",
                    "If {PlayerName} didn't want me to look, they should've hidden it better!",
                    "I'm not stealing, I'm admiring. Big difference.",
                    "Time to evaluate {PlayerName}'s taste in loot...or lack thereof.",
                    "I'd rate this stash a solid... hmm, let's find out.",
                    "The richer the mark, the better the loot. What category is {PlayerName} in?",
                    "I could teach {PlayerName} a thing or two about proper inventory management.",
                    "Let's see if {PlayerName} carries anything *worth* stealing."
                };


                var random = new Random();
                var selectedDialogue = rogueDialogues[random.Next(rogueDialogues.Count)]
                    .Replace("{PlayerName}", target.Name);


                var dialog = options.DialogFactory.Create(options.DialogKey, merch);
                
                foreach (var item in target.Inventory.OrderBy(x => x.Template.Category))
                    dialog.Items.Add(ItemDetails.WithdrawItem(item));

                dialog.InjectTextParameters(selectedDialogue);
                dialog.Display(context.SourceAisling);
            }
        }
        
        public interface IShowPeekOptions
        {
            IMerchantFactory MerchantFactory { get; init; }
            IDialogFactory DialogFactory { get; init; }
            string DialogKey { get; init; }
        }
    }
}