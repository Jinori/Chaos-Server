using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Mainstory.IntelligenceTrialChest;

public class IntelligenceTrialStoneScript : MerchantScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public IntelligenceTrialStoneScript(Merchant subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
    }

    public override void OnClicked(Aisling source)
    {
        if (Subject.Template.TemplateKey == "intelligence_trial_stone")
        {
            if (source.DistanceFrom(Subject) >= 3)
            {
                source.SendOrangeBarMessage("Move closer to touch the stone.");
                return;
            }

            if (source.DistanceFrom(Subject) <= 3)
            {
                if (source.Trackers.Enums.HasValue(IntelligenceTrial.StartedTrial))
                {
                    source.Trackers.Enums.Set(IntelligenceTrial.CompletedTrial);
                    source.SendOrangeBarMessage("The stone glows, you notice your gamepoints rise.");
                    source.TryGiveGamePoints(20);
                    var point = new Point(16, 16);
                    var mapinstance = SimpleCache.Get<MapInstance>("godsrealm");
                    source.TraverseMap(mapinstance, point);
                    source.SendOrangeBarMessage("Congratulations! You have completed the Trial of Intelligence!");
                }
            }
        }
    }
}
 