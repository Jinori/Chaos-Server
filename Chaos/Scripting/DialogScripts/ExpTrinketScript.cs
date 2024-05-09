using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts;

public class ExpTrinketScript : DialogScriptBase
{
    private readonly Dictionary<string, Action<Aisling>> Actions;

    public ExpTrinketScript(Dialog subject) : base(subject) =>
        Actions = new Dictionary<string, Action<Aisling>>
        {
            ["exptrinket_initial"] = OnDisplayingInitial,
            ["exptrinket_starttimer"] = StartExpTimer,
            ["exptrinket_checkexprates"] = CheckExpRates,
            ["exptrinket_resetexptimer"] = ResetExpValues
        };

    public override void OnDisplaying(Aisling source)
    {
        if (Actions.TryGetValue(Subject.Template.TemplateKey.ToLower(), out var action))
            action(source);
    }

    private void OnDisplayingInitial(Aisling source)
    {
        if (source.Trackers.Enums.TryGetValue(out ExpTimerStage stage) && stage == ExpTimerStage.Tracking)
        {
            RemoveOption("Start Timer");
            InsertOption("Stop Timer", "exptrinket_starttimer");
        }
    }

    private void RemoveOption(string optionName)
    {
        var index = Subject.Options.FindIndex(o => o.OptionText == optionName);
        if (index != -1)
            Subject.Options.RemoveAt(index);
    }

    private void InsertOption(string optionText, string dialogKey)
    {
        if (Subject.Options.All(o => o.OptionText != optionText))
            Subject.Options.Insert(0, new DialogOption { DialogKey = dialogKey, OptionText = optionText });
    }

    private void StartExpTimer(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out ExpTimerStage stage);

        switch (stage)
        {
            case ExpTimerStage.None:
            case ExpTimerStage.Stopped:
                SetExpTimer(source, ExpTimerStage.Tracking, source.UserStatSheet.TotalExp, DateTime.UtcNow);
                Subject.Reply(source, "Timer has started tracking your experience!");
                break;
        
            case ExpTimerStage.Tracking:
                SetExpTimer(source, ExpTimerStage.Stopped, source.SavedExpBoxed, DateTime.UtcNow);
                Subject.Reply(source, "Timer has stopped.");
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

            Subject.Reply(source, $"You have gained {expGained:N0} EXP in {duration.TotalMinutes:N0} minutes, which is approximately {expPerHour:N0} EXP per hour.");
        }
        else
        {
            Subject.Reply(source, "No active EXP tracking session.");
        }
    }
    private void ResetExpValues(Aisling source)
    {
        SetExpTimer(source, ExpTimerStage.Stopped, 0, DateTime.MinValue);
        Subject.Reply(source, "EXP tracking values have been reset.");
    }

}