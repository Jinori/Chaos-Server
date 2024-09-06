using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.DialogScripts;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Models.Menu;

public sealed record Dialog : IScripted<IDialogScript>
{
    private readonly IDialogFactory DialogFactory;
    public object? Context { get; set; }
    public IDialogSourceEntity DialogSource { get; set; }
    public List<ItemDetails> Items { get; set; }
    public ArgumentCollection MenuArgs { get; set; }
    public string? NextDialogKey { get; set; }
    public List<DialogOption> Options { get; set; }
    public string? PrevDialogKey { get; set; }
    public List<Skill> Skills { get; set; }
    public List<byte>? Slots { get; set; }
    public List<Spell> Spells { get; set; }
    public DialogTemplate Template { get; set; }
    public string Text { get; private set; }
    public ushort? TextBoxLength { get; set; }
    public string? TextBoxPrompt { get; set; }
    public ChaosDialogType Type { get; set; }
    public bool Contextual { get; }

    /// <inheritdoc />
    public IDialogScript Script { get; }

    /// <inheritdoc />
    public ISet<string> ScriptKeys { get; }

    private Dialog(DialogTemplate template, IDialogSourceEntity dialogSource)
    {
        DialogFactory = null!;
        Template = template;
        DialogSource = dialogSource;
        ScriptKeys = null!;
        Script = null!;
        Items = new List<ItemDetails>();
        NextDialogKey = template.NextDialogKey;
        Options = template.Options.ToList();
        PrevDialogKey = template.PrevDialogKey;
        ScriptKeys = new HashSet<string>(template.ScriptKeys, StringComparer.OrdinalIgnoreCase);
        Skills = new List<Skill>();
        Spells = new List<Spell>();
        Text = template.Text;
        TextBoxLength = template.TextBoxLength;
        TextBoxPrompt = template.TextBoxPrompt;
        Type = template.Type;
        MenuArgs = new ArgumentCollection();
        Contextual = template.Contextual;
    }

    public Dialog(
        DialogTemplate template,
        IDialogSourceEntity dialogSource,
        IScriptProvider scriptProvider,
        IDialogFactory dialogFactory,
        ICollection<string>? extraScriptKeys = null)
        : this(template, dialogSource)
    {
        extraScriptKeys ??= Array.Empty<string>();

        DialogFactory = dialogFactory;
        ScriptKeys.AddRange(extraScriptKeys);
        Script = scriptProvider.CreateScript<IDialogScript, Dialog>(ScriptKeys, this);
    }

    public Dialog(
        IDialogSourceEntity dialogSource,
        IDialogFactory dialogFactory,
        ChaosDialogType type,
        string text)
    {
        Text = text;
        Type = type;
        DialogSource = dialogSource;
        Template = null!;
        DialogFactory = dialogFactory;
        TextBoxLength = null;
        NextDialogKey = null;
        PrevDialogKey = null;
        Script = new CompositeDialogScript();
        Options = new List<DialogOption>();
        ScriptKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Skills = new List<Skill>();
        Spells = new List<Spell>();
        Items = new List<ItemDetails>();
        MenuArgs = new ArgumentCollection();
    }

    public void Close(Aisling source)
    {
        Type = ChaosDialogType.CloseDialog;
        Options.Clear();
        NextDialogKey = null;
        source.Client.SendDisplayDialog(this);
        source.ActiveDialog.TryRemove(this);
        source.DialogHistory.Clear();
    }

    public void Display(Aisling source)
    {
        source.ActiveDialog.Set(this);

        Script.OnDisplaying(source);
        
        CheckSkills(source);
        CheckSpells(source);

        //if a different dialog was displayed while this one was being displayed
        if (source.ActiveDialog.Get() != this)
            return;

        if (!Text.EqualsI("skip"))
            source.Client.SendDisplayDialog(this);

        Script.OnDisplayed(source);

        //if a different dialog was displayed while this one was being displayed
        if (source.ActiveDialog.Get() != this)
            return;

        if (Text.EqualsI("skip"))
        {
            if (!string.IsNullOrEmpty(NextDialogKey))
                Next(source);
            else
                Close(source);
        }
    }

    public void InjectTextParameters(params object[] parameters) => Text = Text.Inject(parameters);

    public void Next(Aisling source, byte? optionIndex = null)
    {
        if (optionIndex is 0)
            optionIndex = null;

        //for some reason some of these types add a +1 to the pursuit id when you respond
        //we're using the pursuit id as an option selector
        //so for any non-menu type, option index should be null (because there are no options)
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (Type)
        {
            case ChaosDialogType.ShowItems:
            case ChaosDialogType.ShowPlayerItems:
            case ChaosDialogType.ShowSpells:
            case ChaosDialogType.ShowSkills:
            case ChaosDialogType.ShowPlayerSpells:
            case ChaosDialogType.ShowPlayerSkills:
            case ChaosDialogType.Normal:
            case ChaosDialogType.DialogTextEntry:
            case ChaosDialogType.Speak:
            case ChaosDialogType.CreatureMenu:
            case ChaosDialogType.Protected:
            case ChaosDialogType.CloseDialog:
                optionIndex = null;

                break;
        }

        source.DialogHistory.Push(this);
        Script.OnNext(source, optionIndex);

        //if a different dialog was displayed, do not continue this dialog
        if (source.ActiveDialog.Get() != this)
            return;

        var nextDialogKey = optionIndex.HasValue
            ? Options.ElementAtOrDefault(optionIndex.Value - 1)
                     ?.DialogKey
            : NextDialogKey;

        if (!string.IsNullOrEmpty(nextDialogKey))
        {
            if (nextDialogKey.EqualsI("close"))
            {
                Close(source);

                return;
            }

            if (nextDialogKey.EqualsI("top"))
            {
                if (DialogSource is MapEntity mapEntity && !mapEntity.WithinRange(source))
                {
                    Close(source);

                    return;
                }

                DialogSource.Activate(source);

                return;
            }

            var nextDialog = DialogFactory.Create(nextDialogKey, DialogSource);

            if (nextDialog.Contextual)
            {
                nextDialog.MenuArgs = new ArgumentCollection(MenuArgs);
                nextDialog.Context = DeepClone.Create(Context);
            }

            nextDialog.Display(source);
        }
    }

    public void Previous(Aisling source)
    {
        //if no prev dialog key, close
        //if source is a map entity that's out of range, close
        if (string.IsNullOrEmpty(PrevDialogKey) || (DialogSource is MapEntity mapEntity && !mapEntity.WithinRange(source)))
            Close(source);
        else
        {
            var prevDialog = source.DialogHistory.PopUntil(d => d.Template.TemplateKey.EqualsI(PrevDialogKey));
            source.DialogHistory.TryPeek(out var prevPrevDialog);

            //if PreviousDialogKey references something that didn't happen, close the dialog
            if (prevDialog is null)
            {
                Close(source);

                throw new InvalidOperationException(
                    $"Attempted to from dialogKey \"{Template.TemplateKey}\" to dialogKey \"{PrevDialogKey
                    }\" but no dialog with that key was found in the history.");
            }

            var newPrevDialog = DialogFactory.Create(PrevDialogKey, DialogSource);

            //if the dialog is contextual, copy the context and menu args from the previous dialog
            if (newPrevDialog.Contextual)
            {
                newPrevDialog.MenuArgs = new ArgumentCollection(prevPrevDialog?.MenuArgs);
                newPrevDialog.Context = DeepClone.Create(prevPrevDialog?.Context);
            }

            Script.OnPrevious(source);
            prevDialog.Display(source);
        }
    }

    public void Reply(Aisling source, string dialogText, string? nextDialogKey = null)
    {
        var newDialog = new Dialog(
            DialogSource,
            DialogFactory,
            ChaosDialogType.Normal,
            dialogText)
        {
            NextDialogKey = nextDialogKey
        };

        newDialog.Display(source);
    }

    private void CheckSpells(Aisling source)
    {
        if (source.IsGodModeEnabled())
            return;
// Check for higher-tier spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("athar"))
        {
            source.SpellBook.RemoveByTemplateKey("beagathar");
        }

        if (source.SpellBook.ContainsByTemplateKey("morathar"))
        {
            source.SpellBook.RemoveByTemplateKey("athar");
            source.SpellBook.RemoveByTemplateKey("beagathar");
        }

        if (source.SpellBook.ContainsByTemplateKey("ardathar"))
        {
            source.SpellBook.RemoveByTemplateKey("morathar");
            source.SpellBook.RemoveByTemplateKey("athar");
            source.SpellBook.RemoveByTemplateKey("beagathar");
        }

        if (source.SpellBook.ContainsByTemplateKey("moratharmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("atharmeall");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("ardatharmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("moratharmeall");
            source.SpellBook.RemoveByTemplateKey("atharmeall");
        }

        if (source.SpellBook.ContainsByTemplateKey("atharlamh"))
        {
            source.SpellBook.RemoveByTemplateKey("beagatharlamh");
        }

        if (source.SpellBook.ContainsByTemplateKey("moratharlamh"))
        {
            source.SpellBook.RemoveByTemplateKey("atharlamh");
            source.SpellBook.RemoveByTemplateKey("beagatharlamh");
        }
        
        // Check for higher-tier Creag spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("creag"))
        {
            source.SpellBook.RemoveByTemplateKey("beagcreag");
        }

        if (source.SpellBook.ContainsByTemplateKey("morcreag"))
        {
            source.SpellBook.RemoveByTemplateKey("creag");
            source.SpellBook.RemoveByTemplateKey("beagcreag");
        }

        if (source.SpellBook.ContainsByTemplateKey("ardcreag"))
        {
            source.SpellBook.RemoveByTemplateKey("morcreag");
            source.SpellBook.RemoveByTemplateKey("creag");
            source.SpellBook.RemoveByTemplateKey("beagcreag");
        }

        if (source.SpellBook.ContainsByTemplateKey("morcreagmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("creagmeall");
        }
        if (source.SpellBook.ContainsByTemplateKey("ardcreagmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("creagmeall");
            source.SpellBook.RemoveByTemplateKey("morcreagmeall");
        }

        if (source.SpellBook.ContainsByTemplateKey("creaglamh"))
        {
            source.SpellBook.RemoveByTemplateKey("beagcreaglamh");
        }

        if (source.SpellBook.ContainsByTemplateKey("morcreaglamh"))
        {
            source.SpellBook.RemoveByTemplateKey("creaglamh");
            source.SpellBook.RemoveByTemplateKey("beagcreaglamh");
        }

        // Check for higher-tier Sal spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("sal"))
        {
            source.SpellBook.RemoveByTemplateKey("beagsal");
        }

        if (source.SpellBook.ContainsByTemplateKey("morsal"))
        {
            source.SpellBook.RemoveByTemplateKey("sal");
            source.SpellBook.RemoveByTemplateKey("beagsal");
        }

        if (source.SpellBook.ContainsByTemplateKey("ardsal"))
        {
            source.SpellBook.RemoveByTemplateKey("morsal");
            source.SpellBook.RemoveByTemplateKey("sal");
            source.SpellBook.RemoveByTemplateKey("beagsal");
        }

        if (source.SpellBook.ContainsByTemplateKey("morsalmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("salmeall");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("ardsalmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("salmeall");
            source.SpellBook.RemoveByTemplateKey("morsalmeall");
        }

        if (source.SpellBook.ContainsByTemplateKey("sallamh"))
        {
            source.SpellBook.RemoveByTemplateKey("beagsallamh");
        }

        if (source.SpellBook.ContainsByTemplateKey("morsallamh"))
        {
            source.SpellBook.RemoveByTemplateKey("sallamh");
            source.SpellBook.RemoveByTemplateKey("beagsallamh");
        }
// Check for higher-tier Srad spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("srad"))
        {
            source.SpellBook.RemoveByTemplateKey("beagsrad");
        }

        if (source.SpellBook.ContainsByTemplateKey("morsrad"))
        {
            source.SpellBook.RemoveByTemplateKey("srad");
            source.SpellBook.RemoveByTemplateKey("beagsrad");
        }

        if (source.SpellBook.ContainsByTemplateKey("ardsrad"))
        {
            source.SpellBook.RemoveByTemplateKey("morsrad");
            source.SpellBook.RemoveByTemplateKey("srad");
            source.SpellBook.RemoveByTemplateKey("beagsrad");
        }

        if (source.SpellBook.ContainsByTemplateKey("morsradmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("sradmeall");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("ardsradmeall"))
        {
            source.SpellBook.RemoveByTemplateKey("sradmeall");
            source.SpellBook.RemoveByTemplateKey("morsradmeall");
        }

        if (source.SpellBook.ContainsByTemplateKey("sradlamh"))
        {
            source.SpellBook.RemoveByTemplateKey("beagsradlamh");
        }

        if (source.SpellBook.ContainsByTemplateKey("morsradlamh"))
        {
            source.SpellBook.RemoveByTemplateKey("sradlamh");
            source.SpellBook.RemoveByTemplateKey("beagsradlamh");
        }

        // Check for higher-tier Arcane spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("arcanemissile"))
        {
            source.SpellBook.RemoveByTemplateKey("arcanebolt");
        }

        if (source.SpellBook.ContainsByTemplateKey("arcaneblast"))
        {
            source.SpellBook.RemoveByTemplateKey("arcanemissile");
            source.SpellBook.RemoveByTemplateKey("arcanebolt");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("arcaneexplosion"))
        {
            source.SpellBook.RemoveByTemplateKey("arcaneblast");
            source.SpellBook.RemoveByTemplateKey("arcanemissile");
            source.SpellBook.RemoveByTemplateKey("arcanebolt");
        }

        // Check for higher-tier Trap spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("stilettotrap"))
        {
            source.SpellBook.RemoveByTemplateKey("needletrap");
        }

        if (source.SpellBook.ContainsByTemplateKey("bolttrap"))
        {
            source.SpellBook.RemoveByTemplateKey("needletrap");
            source.SpellBook.RemoveByTemplateKey("stilettotrap");
        }

        if (source.SpellBook.ContainsByTemplateKey("coiledbolttrap"))
        {
            source.SpellBook.RemoveByTemplateKey("needletrap");
            source.SpellBook.RemoveByTemplateKey("stilettotrap");
            source.SpellBook.RemoveByTemplateKey("bolttrap");
        }

        if (source.SpellBook.ContainsByTemplateKey("springtrap"))
        {
            source.SpellBook.RemoveByTemplateKey("needletrap");
            source.SpellBook.RemoveByTemplateKey("stilettotrap");
            source.SpellBook.RemoveByTemplateKey("bolttrap");
            source.SpellBook.RemoveByTemplateKey("coiledbolttrap");
        }

        if (source.SpellBook.ContainsByTemplateKey("maidentrap"))
        {
            source.SpellBook.RemoveByTemplateKey("needletrap");
            source.SpellBook.RemoveByTemplateKey("stilettotrap");
            source.SpellBook.RemoveByTemplateKey("bolttrap");
            source.SpellBook.RemoveByTemplateKey("coiledbolttrap");
            source.SpellBook.RemoveByTemplateKey("springtrap");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("pitfalltrap"))
        {
            source.SpellBook.RemoveByTemplateKey("needletrap");
            source.SpellBook.RemoveByTemplateKey("stilettotrap");
            source.SpellBook.RemoveByTemplateKey("bolttrap");
            source.SpellBook.RemoveByTemplateKey("coiledbolttrap");
            source.SpellBook.RemoveByTemplateKey("springtrap");
            source.SpellBook.RemoveByTemplateKey("maidentrap");
        }

        // Check for higher-tier Priest spells and remove lower-tier ones accordingly
        if (source.SpellBook.ContainsByTemplateKey("pramh"))
        {
            source.SpellBook.RemoveByTemplateKey("beagpramh");
        }

        if (source.SpellBook.ContainsByTemplateKey("revive"))
        {
            source.SpellBook.RemoveByTemplateKey("beothaich");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("resurrection"))
        {
            source.SpellBook.RemoveByTemplateKey("beothaich");
            source.SpellBook.RemoveByTemplateKey("revive");
        }

        if (source.SpellBook.ContainsByTemplateKey("warcry"))
        {
            source.SpellBook.RemoveByTemplateKey("battlecry");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("howl"))
        {
            source.SpellBook.RemoveByTemplateKey("goad");
        }
        
        if (source.SpellBook.ContainsByTemplateKey("roar"))
        {
            source.SpellBook.RemoveByTemplateKey("battlecry");
            source.SpellBook.RemoveByTemplateKey("howl");
        }

    }

    private void CheckSkills(Aisling source)
    {
        if (source.IsGodModeEnabled())
            return;
        // Check for higher-tier Warrior skills and remove lower-tier ones
        if (source.SkillBook.ContainsByTemplateKey("cleave"))
        {
            source.SkillBook.RemoveByTemplateKey("scathe");
        }

        if (source.SkillBook.ContainsByTemplateKey("clobber"))
        {
            source.SkillBook.RemoveByTemplateKey("strike");
        }

        if (source.SkillBook.ContainsByTemplateKey("wallop"))
        {
            source.SkillBook.RemoveByTemplateKey("strike");
            source.SkillBook.RemoveByTemplateKey("clobber");
        }

        if (source.SkillBook.ContainsByTemplateKey("pulverize"))
        {
            source.SkillBook.RemoveByTemplateKey("strike");
            source.SkillBook.RemoveByTemplateKey("clobber");
            source.SkillBook.RemoveByTemplateKey("wallop");
        }

        if (source.SkillBook.ContainsByTemplateKey("thrash"))
        {
            source.SkillBook.RemoveByTemplateKey("strike");
            source.SkillBook.RemoveByTemplateKey("clobber");
            source.SkillBook.RemoveByTemplateKey("wallop");
            source.SkillBook.RemoveByTemplateKey("pulverize");
        }

        if (source.SkillBook.ContainsByTemplateKey("sunder"))
        {
            source.SkillBook.RemoveByTemplateKey("slash");
        }

        if (source.SkillBook.ContainsByTemplateKey("tempestblade"))
        {
            source.SkillBook.RemoveByTemplateKey("windblade");
        }

        if (source.SkillBook.ContainsByTemplateKey("paralyzeforce"))
        {
            source.SkillBook.RemoveByTemplateKey("groundstomp");
        }

        if (source.SkillBook.ContainsByTemplateKey("madsoul"))
        {
            source.SkillBook.RemoveByTemplateKey("flurry");
        }

        if (source.SkillBook.ContainsByTemplateKey("charge"))
        {
            source.SkillBook.RemoveByTemplateKey("bullrush");
        }

        // Check for higher-tier Monk skills and remove lower-tier ones
        if (source.SkillBook.ContainsByTemplateKey("doublepunch"))
        {
            source.SkillBook.RemoveByTemplateKey("punch");
        }

        if (source.SkillBook.ContainsByTemplateKey("rapidpunch"))
        {
            source.SkillBook.RemoveByTemplateKey("punch");
            source.SkillBook.RemoveByTemplateKey("doublepunch");
        }

        if (source.SkillBook.ContainsByTemplateKey("roundhousekick"))
        {
            source.SkillBook.RemoveByTemplateKey("kick");
        }

        if (source.SkillBook.ContainsByTemplateKey("mantiskick"))
        {
            source.SkillBook.RemoveByTemplateKey("highkick");
        }

        // Check for higher-tier Rogue skills and remove lower-tier ones
        if (source.SkillBook.ContainsByTemplateKey("blitz"))
        {
            source.SkillBook.RemoveByTemplateKey("assault");
        }

        if (source.SkillBook.ContainsByTemplateKey("barrage"))
        {
            source.SkillBook.RemoveByTemplateKey("assault");
            source.SkillBook.RemoveByTemplateKey("blitz");
        }

        if (source.SkillBook.ContainsByTemplateKey("gut"))
        {
            source.SkillBook.RemoveByTemplateKey("stab");
        }

        if (source.SkillBook.ContainsByTemplateKey("skewer"))
        {
            source.SkillBook.RemoveByTemplateKey("pierce");
        }

    }

    public void ReplyToUnknownInput(Aisling source) => Reply(source, DialogString.UnknownInput);

    #region Dialog Options
    public int? GetOptionIndex(string optionText)
    {
        var index = Options.FindIndex(option => option.OptionText.EqualsI(optionText));

        return index == -1 ? null : index;
    }

    public string? GetOptionText(int optionIndex)
    {
        var option = Options.ElementAtOrDefault(optionIndex - 1);

        return option?.OptionText;
    }

    public bool HasOption(string optionText) => GetOptionIndex(optionText) is not null;

    public DialogOption GetOption(string optionText) => Options.First(option => option.OptionText.EqualsI(optionText));

    public void InsertOption(int index, string optionText, string dialogKey)
    {
        if (index >= Options.Count)
            AddOption(optionText, dialogKey);
        else
            Options.Insert(
                index,
                new DialogOption
                {
                    OptionText = optionText,
                    DialogKey = dialogKey
                });
    }

    public void AddOption(string optionText, string dialogKey)
        => Options.Add(
            new DialogOption
            {
                OptionText = optionText,
                DialogKey = dialogKey
            });

    public void AddOptions(params (string OptionText, string DialogKey)[] options)
        => Options.AddRange(
            options.Select(
                option => new DialogOption
                {
                    OptionText = option.OptionText,
                    DialogKey = option.DialogKey
                }));
    #endregion
}