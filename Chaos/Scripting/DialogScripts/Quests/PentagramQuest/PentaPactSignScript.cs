using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.PentagramQuest;

public class PentaPactSignScript(Dialog subject) : DialogScriptBase(subject)
{
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        //You never used hasStage or stage below?
        //var hasStage = source.Trackers.Enums.TryGetValue(out PentagramQuestStage stage);
        
        var ani = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 120
        };

        if (!TryFetchArgs<string>(out var signName))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (source.Name != signName)
        {
            Subject.Reply(source, "The book shuts itself and slides back onto the shelf.");

            return;
        }
        
        source.Trackers.Enums.Set(PentagramQuestStage.SignedPact);
        source.SendOrangeBarMessage("You signed the pact and feel a surge of pain.");
        source.UserStatSheet.SetHealthPct(50);
        source.Client.SendAttributes(StatUpdateType.Vitality);
        source.Animate(ani);
        Subject.Close(source);
    }
}