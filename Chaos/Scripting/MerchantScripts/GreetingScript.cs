using Chaos.Common.Utilities;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class GreetingScript : MerchantScriptBase
{
    public GreetingScript(Merchant subject) : base(subject)
    {
    }

    public override void OnApproached(Creature source)
    {
        if (source is not Aisling aisling)
            return;
        
        if (IntegerRandomizer.RollChance(60))
            return;
        
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "thibault":
            {
                Subject.Say($"{source.Name} enters the throne room!");
                return;
            }
            case "leia":
            {
                Subject.Say($"Hello {source.Name}!");
                return;
            }
            case "skarn":
            {
                if (aisling.Trackers.TimedEvents.HasActiveEvent("CryptSlayerCd", out var timedEvent))
                    return;

                Subject.Say($"{source.Name}, I have a job for you.");
                return;
            }
            case "riona":
            {
                Subject.Say("Welcome to my Inn!");
                return;
            }
            case "cain":
            {
                Subject.Say("My carrots! Help! Get out of my garden!");
                return;
            }
            case "fabrizio":
            {
                Subject.Say("These leaders just won't listen to reason.");
                return;
            }
            case "sgrios":
            {
                Subject.Say("Another disgusting view. What is it?");
                return;
            }
        }
    }
}