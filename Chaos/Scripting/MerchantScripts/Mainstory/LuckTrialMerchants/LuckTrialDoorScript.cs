using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Models.World;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Services.Storage;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MerchantScripts.LuckTrialMerchants;

public class LuckTrialDoorScript : MerchantScriptBase
{
    private readonly ISimpleCache SimpleCache;

    /// <inheritdoc />
    public LuckTrialDoorScript(Merchant subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
    }

    public override void OnClicked(Aisling source)
    {

        if (Subject.Template.TemplateKey == "doorsuccess")
        {
            if (source.DistanceFrom(Subject) >= 4)
            {
                source.SendOrangeBarMessage("Move closer to the door to choose it.");
                return;
            }

            if (source.DistanceFrom(Subject) <= 3)
            {
                if (source.Trackers.Enums.HasValue(LuckTrial.StartedTrial))
                {
                    source.Trackers.Enums.Set(LuckTrial.SucceededFirst);
                    source.SendOrangeBarMessage("The door opens and lets you through.");
                    var point = new Point(7, 48);
                    source.WarpTo(point);
                    foreach (var merchant in source.MapInstance.GetEntities<Merchant>())
                        if (merchant.Template.TemplateKey != "trialofluckchest")
                        {
                            source.MapInstance.RemoveEntity(merchant);
                        }

                    return;
                }

                if (source.Trackers.Enums.HasValue(LuckTrial.SucceededFirst))
                {
                    source.Trackers.Enums.Set(LuckTrial.SucceededSecond);
                    source.SendOrangeBarMessage("The door opens and lets you through.");
                    var point = new Point(7, 27);
                    source.WarpTo(point);
                    foreach (var merchant in source.MapInstance.GetEntities<Merchant>())
                        if (merchant.Template.TemplateKey != "trialofluckchest")
                        {
                            source.MapInstance.RemoveEntity(merchant);
                        }

                    return;
                }

                if (source.Trackers.Enums.HasValue(LuckTrial.SucceededSecond))
                {
                    source.Trackers.Enums.Set(LuckTrial.SucceededThird);
                    source.SendOrangeBarMessage("The door opens and lets you through.");
                    var point = new Point(7, 9);
                    source.WarpTo(point);
                    foreach (var merchant in source.MapInstance.GetEntities<Merchant>())
                        if (merchant.Template.TemplateKey != "trialofluckchest")
                        {
                            source.MapInstance.RemoveEntity(merchant);
                        }

                    return;
                }
            }
        }

        if (Subject.Template.TemplateKey == "doorfail")
        {
            if (source.DistanceFrom(Subject) >= 4)
            {
                source.SendOrangeBarMessage("Move closer to the door to choose it.");
                return;
            }

            if (source.DistanceFrom(Subject) <= 3)
            {
                source.Trackers.Enums.Set(LuckTrial.StartedTrial);
                source.SendOrangeBarMessage("The door locks and sends you flying.");
                var point = new Point(7, 71);
                source.WarpTo(point);

                foreach (var merchant in source.MapInstance.GetEntities<Merchant>())
                    if (merchant.Template.TemplateKey != "trialofluckchest")
                    {
                        source.MapInstance.RemoveEntity(merchant);
                    }

                return;
            }
        }

        if (Subject.Template.TemplateKey == "trialofluckchest")
        {
            if (source.DistanceFrom(Subject) >= 4)
            {
                source.SendOrangeBarMessage("Move closer to the chest to open it.");
                return;
            }

            if (source.DistanceFrom(Subject) <= 3)
            {
                if (source.Trackers.Enums.HasValue(LuckTrial.CompletedTrial))
                {
                    source.Trackers.Enums.Set(LuckTrial.CompletedTrial2);
                    source.SendOrangeBarMessage("The chest flies open! You discover a bag of gold!");
                    source.TryGiveGold(25000);
                    var point = new Point(16, 16);
                    var mapinstance = SimpleCache.Get<MapInstance>("godsrealm");
                    source.TraverseMap(mapinstance, point);
                    source.SendOrangeBarMessage("Congratulations! You have completed the Trial of Luck!");
                }
            }
        }
    }
}
 