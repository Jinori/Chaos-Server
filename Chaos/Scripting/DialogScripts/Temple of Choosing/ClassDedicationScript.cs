using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.DialogScripts.Temple_of_Choosing;

public class ClassDedicationScript : DialogScriptBase
{
    private const int REQUIRED_EXPERIENCE = 60000000;

    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

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
        { "miraelisgod_selectwizard", BaseClass.Wizard },
        { "miraelisgod_selectwarrior", BaseClass.Warrior },
        { "miraelisgod_selectrogue", BaseClass.Rogue },
        { "miraelisgod_selectpriest", BaseClass.Priest },
        { "miraelisgod_selectmonk", BaseClass.Monk }
    };
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;

    public ClassDedicationScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, ISpellFactory spellFactory, ISkillFactory skillFactory)
        : base(subject)
    {
        ClientRegistry = clientRegistry;
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
    }

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
            
            GiveNewAbilities(source, baseClass);
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
        const string CLASS_CONFIRM_REQUIREMENTS = "miraelisgod_confirmrequirements";
        const string CLASS_CHOOSE_NEW = "miraelisgod_choosenewclass";
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
            case CLASS_CONFIRM_REQUIREMENTS when !source.Trackers.Flags.HasFlag(MainstoryFlags.CompletedFloor3):
                Subject.Reply(source, "You must follow the Main Story Quest further.");
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
                    Subject.Reply(source, builtReply, "miraelisgod_chooseNewClass");
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

    private void GiveNewAbilities(Aisling source, BaseClass baseClass)
    {
        if (baseClass == BaseClass.Monk)
        {
            var skill = SkillFactory.Create("punch");
            var skill1 = SkillFactory.Create("kick");
            var spell = SpellFactory.Create("taunt");
            source.SkillBook.TryAddToNextSlot(skill);
            source.SkillBook.TryAddToNextSlot(skill1);
            source.SpellBook.TryAddToNextSlot(spell);
        }

        if (baseClass == BaseClass.Warrior)
        {
            var skill = SkillFactory.Create("strike");
            var skill1 = SkillFactory.Create("slash");
            source.SkillBook.TryAddToNextSlot(skill);
            source.SkillBook.TryAddToNextSlot(skill1);
        }

        if (baseClass == BaseClass.Rogue)
        {
            var skill = SkillFactory.Create("assault");
            var skill2 = SkillFactory.Create("stab");

            if (!source.SkillBook.Contains(skill2))
                source.SkillBook.TryAddToNextSlot(skill2);

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);
        }

        if (baseClass == BaseClass.Wizard)
        {
            var skill = SkillFactory.Create("energybolt");
            var spell = SpellFactory.Create("arcanebolt");

            if (!source.SpellBook.Contains(spell))
                source.SpellBook.TryAddToNextSlot(spell);

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);
        }

        if (baseClass == BaseClass.Priest)
        {
            var skill = SkillFactory.Create("blessedbolt");
            var spell = SpellFactory.Create("beagioc");
            var spell2 = SpellFactory.Create("beothaich");
            var spell3 = SpellFactory.Create("spark");

            if (!source.SpellBook.Contains(spell))
                source.SpellBook.TryAddToNextSlot(spell);

            if (!source.SpellBook.Contains(spell2))
                source.SpellBook.TryAddToNextSlot(spell2);

            if (!source.SpellBook.Contains(spell3))
                source.SpellBook.TryAddToNextSlot(spell3);

            if (!source.SkillBook.Contains(skill))
                source.SkillBook.TryAddToNextSlot(skill);
        }
    }

    private void SetUserToLevel1Stats(Aisling source, BaseClass baseClass)
    {
        if (source.Bank.Contains("Pet Collar"))
            source.Bank.TryWithdraw("Pet Collar", 1, out _);
        
        if (source.Inventory.Contains("Pet Collar")) 
            source.Inventory.RemoveQuantity("Pet Collar", 1);

        if (source.SkillBook.Contains("summonpet"))
            source.SkillBook.Remove("summonpet");
        
        source.Inventory.RemoveQuantity("strong health potion", 10);
        source.UserStatSheet.SetLevel(1);
        source.UserStatSheet.SubtractTotalExp(source.UserStatSheet.TotalExp);

        if (source.UserStatSheet.ToNextLevel >= 1)
            source.UserStatSheet.SubtractTnl(source.UserStatSheet.ToNextLevel);

        source.UserStatSheet.AddTnl(599);
        
        
        var baseStats = UserStatSheet.NewCharacter;
        
        var diff = new Attributes()
        {
            Ac = source.StatSheet.Ac - baseStats.Ac,
            MaximumHp = source.StatSheet.MaximumHp - baseStats.MaximumHp,
            MaximumMp = source.StatSheet.MaximumMp - baseStats.MaximumMp,
            Hit = source.StatSheet.Hit - baseStats.Hit,
            Dmg = source.StatSheet.Dmg - baseStats.Dmg,
            MagicResistance = source.StatSheet.MagicResistance - baseStats.MagicResistance,
            AtkSpeedPct = source.StatSheet.AtkSpeedPct - baseStats.AtkSpeedPct,
            FlatSkillDamage = source.StatSheet.FlatSkillDamage - baseStats.FlatSkillDamage,
            FlatSpellDamage = source.StatSheet.FlatSpellDamage - baseStats.FlatSpellDamage,
            SkillDamagePct = source.StatSheet.SkillDamagePct - baseStats.SkillDamagePct,
            SpellDamagePct = source.StatSheet.SpellDamagePct - baseStats.SpellDamagePct,
            Str = source.StatSheet.Str - baseStats.Str,
            Int = source.StatSheet.Int - baseStats.Int,
            Wis = source.StatSheet.Wis - baseStats.Wis,
            Con = source.StatSheet.Con - baseStats.Con,
            Dex = source.StatSheet.Dex - baseStats.Dex
        };
        
        source.UserStatSheet.Subtract(diff);
        source.UserStatSheet.SetMaxWeight(44);
        source.UserStatSheet.SetBaseClass(baseClass);

        var unspentpoints = source.UserStatSheet.UnspentPoints;
        if (unspentpoints > 19)
        {
            source.UserStatSheet.UnspentPoints = 19;
        }
        source.Trackers.Enums.Remove<PentagramQuestStage>();
        source.Trackers.Enums.Remove<NightmareQuestStage>();
        
        source.Client.SendAttributes(StatUpdateType.Full);
        source.Refresh(true);

        var player = ClientRegistry
            .Select(c => c.Aisling);

        foreach (var aisling in player)
            aisling.SendOrangeBarMessage($"{source.Name} has dedicated to the path of {baseClass.ToString()}.");
    }
}