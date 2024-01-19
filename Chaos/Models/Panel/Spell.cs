using Chaos.Extensions.Common;
using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Models.Panel;

/// <summary>
///     Represents an object that exists within the spell panel.
/// </summary>
public sealed class Spell : PanelEntityBase, IScripted<ISpellScript>
{
    public byte CastLines { get; set; }
    public byte Level { get; set; }
    public byte MaxLevel { get; set; }
    public ISpellScript Script { get; }
    public override SpellTemplate Template { get; }
    public string PanelDisplayName => $"{Template.Name} (Lev:{Level}/{MaxLevel})";

    public Spell(
        SpellTemplate template,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null,
        int? elapsedMs = null)
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;
        CastLines = template.CastLines;
        MaxLevel = template.MaxLevel;

        if (!template.LevelsUp)
            Level = MaxLevel;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<ISpellScript, Spell>(ScriptKeys, this);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Script.Update(delta);
    }

    public void Use(SpellContext context)
    {
        if (!Script.CanUse(context))
            return;
        
        Script.OnUse(context);
        BeginCooldown(context.Source);
    }
}