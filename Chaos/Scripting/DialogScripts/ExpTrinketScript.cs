using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class ExpTrinketScript : DialogScriptBase
{
    private readonly ISimpleCache SimpleCache;
    private readonly Dictionary<string, Action<Aisling>> Actions;

    
    public ExpTrinketScript(Dialog subject, ISimpleCache simpleCache) : base(subject)
    {
        SimpleCache = simpleCache;
        Actions = new Dictionary<string, Action<Aisling>>
        {
            ["exptrinket_starttimer"] = StartExpTimer,
            ["exptrinket_checkexprates"] = CheckExpRates,
            ["exptrinket_resetexptimer"] = ResetExpValues,
            ["exptrinket_sharerates"] = ShareExpRates,
            ["exptrinket_portaljeweler"] = PortalJeweler
        };
    }

    public override void OnDisplaying(Aisling source)
    {
        if (Actions.TryGetValue(Subject.Template.TemplateKey.ToLower(), out var action))
            action(source);
    }

    private void PortalJeweler(Aisling source)
    {
        var targetMap = SimpleCache.Get<MapInstance>("rucesion_jeweler");
        source.TraverseMap(targetMap, new Point(5, 10));
        source.SendActiveMessage("Sceallog Taithi's sand flows in the direction of home.");
    }
    
    private void StartExpTimer(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ExpTimerStage stage);

        switch (stage)
        {
            case ExpTimerStage.None:
            case ExpTimerStage.Stopped:
                SetExpTimer(source, ExpTimerStage.Tracking, source.UserStatSheet.TotalExp, DateTime.UtcNow);
                Subject.Reply(source, "As the sands flow, so shall your wisdom. The journey of growth starts now.");
                break;
        
            case ExpTimerStage.Tracking:
                SetExpTimer(source, ExpTimerStage.Stopped, source.SavedExpBoxed, DateTime.UtcNow);
                Subject.Reply(source, "The sands pause; your tale awaits its next breath. Reflect on what has passed.");
                break;
        }
    }

    private void SetExpTimer(Aisling source, ExpTimerStage stage, long exp, DateTime time)
    {
        source.Trackers.Enums.Set(stage);
        source.SavedExpBoxed = (uint)exp;
        source.ExpTrinketStartTime = time;
    }
    
    private void CheckExpRates(Aisling source)
    {
        if (source.Trackers.Enums.TryGetValue(out ExpTimerStage stage) && stage == ExpTimerStage.Tracking)
        {
            var duration = DateTime.UtcNow - source.ExpTrinketStartTime;
            var expGained = source.UserStatSheet.TotalExp - source.SavedExpBoxed;
            var expPerHour = expGained / duration.TotalHours;

            Subject.Reply(source, $"In the span of {duration.TotalMinutes:N0} minutes, you have harvested {expGained:N0} points of experience, weaving at a rate of about {expPerHour:N0} EXP per hour. May this knowledge fortify your path.");
        }
        else
            Subject.Reply(source, "Silence prevails. When you are ready, so shall the record of deeds begin.");
    }
    
    private void ShareExpRates(Aisling source)
    {
        if (source.Trackers.Enums.TryGetValue(out ExpTimerStage stage) && (stage == ExpTimerStage.Tracking))
        {
            var duration = DateTime.UtcNow - source.ExpTrinketStartTime;
            var expGained = source.UserStatSheet.TotalExp - source.SavedExpBoxed;
            var expPerHour = expGained / duration.TotalHours;

            if (source.Group != null)
                foreach (var member in source.Group)
                    member.SendServerMessage(
                        ServerMessageType.ScrollWindow,
                        $"In the span of {duration.TotalMinutes:N0} minutes, you have harvested {expGained
                            :N0} points of experience, weaving at a rate of about {expPerHour
                            :N0} EXP per hour." + Environment.NewLine +  "May this knowledge fortify your path.");
            
            Subject.Close(source);
        }
    }
    private void ResetExpValues(Aisling source)
    {
        SetExpTimer(source, ExpTimerStage.Stopped, 0, DateTime.MinValue);
        Subject.Reply(source, "Reset, but not forgotten. Carry forward the lessons, leave behind the weight.");
    }

}