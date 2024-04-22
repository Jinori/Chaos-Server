using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class ClassDedicationScript : DialogScriptBase
{
    private const int REQUIRED_EXPERIENCE = 60000000;

    private readonly Dictionary<BaseClass, (int requiredHealth, int requiredMana)> BaseClassRequirements = new()
    {
        { BaseClass.Monk, (8400, 3400) },
        { BaseClass.Warrior, (8400, 2500) },
        { BaseClass.Rogue, (6600, 3300) },
        { BaseClass.Wizard, (6000, 6000) },
        { BaseClass.Priest, (6600, 6250) }
    };

    private readonly Dictionary<string, BaseClass> ClassNameMappings = new()
    {
        { "aoife_selectwizard", BaseClass.Wizard },
        { "aoife_selectwarrior", BaseClass.Warrior },
        { "aoife_selectrogue", BaseClass.Rogue },
        { "aoife_selectpriest", BaseClass.Priest },
        { "aoife_selectmonk", BaseClass.Monk }
    };
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public ClassDedicationScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject) => ClientRegistry = clientRegistry;

    private void DedicateUserToClass(Aisling source, BaseClass baseClass, string templateKey)
    {
        if (templateKey.Equals(Subject.Template.TemplateKey, StringComparison.CurrentCultureIgnoreCase))
        {
            SetUserToLevel1Stats(source, baseClass);

            source.Legend.AddUnique(
                new LegendMark(
                    $"Dedicated to {baseClass.ToString()}",
                    "dedicated",
                    GetMarkIcon(baseClass),
                    MarkColor.Cyan,
                    1,
                    GameTime.Now));
        }
    }

    private MarkIcon GetMarkIcon(BaseClass baseClass) =>
        baseClass switch
        {
            BaseClass.Wizard  => MarkIcon.Wizard,
            BaseClass.Warrior => MarkIcon.Warrior,
            BaseClass.Rogue   => MarkIcon.Rogue,
            BaseClass.Priest  => MarkIcon.Priest,
            BaseClass.Monk    => MarkIcon.Monk,
            _                 => MarkIcon.Yay
        };

    public override void OnDisplaying(Aisling source)
    {
        const string CLASS_CONFIRM_REQUIREMENTS = "aoife_confirmrequirements";
        const string CLASS_CHOOSE_NEW = "aoife_choosenewclass";
        var currentTemplateKey = Subject.Template.TemplateKey.ToLower();

        switch (currentTemplateKey)
        {
            case CLASS_CONFIRM_REQUIREMENTS when source.Legend.ContainsKey("dedicated"):
                Subject.Reply(source, "You have already dedicated yourself to a new path.");
                Subject.PrevDialogKey = null;
                Subject.NextDialogKey = null;

                return;
            case CLASS_CONFIRM_REQUIREMENTS when source.UserStatSheet.Level != 99:
                Subject.Reply(source, "You are not Level 99 and cannot dedicate your class.");
                Subject.PrevDialogKey = null;
                Subject.NextDialogKey = null;

                return;
            case CLASS_CONFIRM_REQUIREMENTS when source.UserStatSheet.Master:
                Subject.Reply(source, "You are already a Master of your class and cannot rededicate yourself.");
                Subject.PrevDialogKey = null;
                Subject.NextDialogKey = null;

                return;
            case CLASS_CONFIRM_REQUIREMENTS:
            {
                var requiredStats = BaseClassRequirements[source.UserStatSheet.BaseClass];

                string builtReply;

                if ((source.UserStatSheet.MaximumHp >= requiredStats.requiredHealth)
                    && (source.UserStatSheet.MaximumMp >= requiredStats.requiredMana))
                    builtReply = "You have enough vitality to continue.";
                else
                {
                    Subject.Reply(
                        source,
                        $"You do not have the required {requiredStats.requiredHealth} health and {requiredStats.requiredMana
                        } mana to continue.");

                    return;
                }

                if (source.Inventory.CountOf("strong health potion") >= 10)
                    builtReply += " Looks like you've also brought enough strong health potions";
                else
                {
                    builtReply += " but you do not have the required strong health potion. Come back with what you need.";
                    Subject.Reply(source, builtReply);

                    return;
                }

                if (source.UserStatSheet.TotalExp >= REQUIRED_EXPERIENCE)
                {
                    builtReply += " and you've hunted enough experience! Let's continue.";
                    Subject.Reply(source, builtReply, "aoife_chooseNewClass");
                }
                else
                {
                    builtReply += " but you do not have the required experience of 60 million.";
                    Subject.Reply(source, builtReply);
                }

                break;
            }
            case CLASS_CHOOSE_NEW:
            {
                if (Subject.GetOptionIndex(source.UserStatSheet.BaseClass.ToString()).HasValue)
                {
                    var s = Subject.GetOptionIndex(source.UserStatSheet.BaseClass.ToString())!.Value;
                    Subject.Options.RemoveAt(s);
                }

                break;
            }
        }

        foreach (var mapping in ClassNameMappings)
            DedicateUserToClass(source, mapping.Value, mapping.Key);
    }

    private void SetUserToLevel1Stats(Aisling source, BaseClass baseClass)
    {
        if (source.Bank.Contains("Pet Collar"))
            source.Bank.TryWithdraw("Pet Collar", 1, out _);
        
        if (source.Inventory.Contains("Pet Collar")) 
            source.Inventory.RemoveQuantity("Pet Collar", 1);
        
        source.Inventory.RemoveQuantity("strong health potion", 10);
        source.UserStatSheet.SetLevel(1);
        source.UserStatSheet.SubtractTotalExp(source.UserStatSheet.TotalExp);

        if (source.UserStatSheet.ToNextLevel >= 1)
            source.UserStatSheet.SubtractTnl(source.UserStatSheet.ToNextLevel);

        source.UserStatSheet.AddTnl(599);
        source.UserStatSheet.Str = 3;
        source.UserStatSheet.Int = 3;
        source.UserStatSheet.Wis = 3;
        source.UserStatSheet.Con = 3;
        source.UserStatSheet.Dex = 3;
        source.UserStatSheet.SetMaxWeight(51);
        source.UserStatSheet.SetBaseClass(baseClass);

        var unspentpoints = source.UserStatSheet.UnspentPoints;
        if (unspentpoints > 19)
        {
            var equals = source.UserStatSheet.UnspentPoints.Equals(19);
        }
        source.Trackers.Enums.Remove<PentagramQuestStage>();
        source.Trackers.Enums.Remove<NightmareQuestStage>();

        var statBuyCost = new Attributes
        {
            MaximumHp = source.UserStatSheet.MaximumHp - 100,
            MaximumMp = source.UserStatSheet.MaximumMp - 100
        };

        source.UserStatSheet.Subtract(statBuyCost);
        source.Client.SendAttributes(StatUpdateType.Full);
        source.Refresh(true);

        var player = ClientRegistry
            .Select(c => c.Aisling);

        foreach (var aisling in player)
            aisling.SendOrangeBarMessage($"{source.Name} has dedicated to the path of {baseClass.ToString()}.");
    }
}