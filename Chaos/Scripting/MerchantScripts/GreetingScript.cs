using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;

namespace Chaos.Scripting.MerchantScripts;

public class GreetingScript(Merchant subject) : MerchantScriptBase(subject)
{
    public override void OnApproached(Creature source)
    {
        if (source is not Aisling aisling)
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
                if (aisling.StatSheet.Level > 71)
                    return;

                if (aisling.Trackers.TimedEvents.HasActiveEvent("CryptSlayerCd", out _))
                    return;

                if (aisling.Trackers.Enums.TryGetValue(out RionaTutorialQuestStage stage) &&
                    (stage == RionaTutorialQuestStage.StartedSkarn))
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
            case "nightmare_priestsupport":
            {
                Subject.Say("Do you need help warrior? Speak to me and I will heal you!");

                return;
            }
            case "miraelisgod":
            {
                if (source.Trackers.Enums.HasValue(MainStoryEnums.SpokeToZephyr))
                {
                    Subject.Say($"Welcome {source.Name}, what news have you to share?");
                    return;
                }
                Subject.Say($"Welcome back {source.Name}.");
                return;
            }
        }
    }
}