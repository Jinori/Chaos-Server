using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates.Abstractions;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a spell template
/// </summary>
public sealed record SpellTemplateSchema : PanelEntityTemplateSchema
{
    /// <summary>
    /// The element a wizard spell uses when casting, calculated for damage
    /// </summary>
    public WizardElement? WizardElement { get; set; }
    
    /// <summary>
    ///     The number of chant lines this spell requires by default
    /// </summary>
    public byte CastLines { get; set; }

    /// <summary>
    ///     Default null<br />If set, these are the requirements for the spell to be learned
    /// </summary>
    /// <remarks>this is a test</remarks>
    public LearningRequirementsSchema? LearningRequirements { get; set; }
    /// <summary>
    ///     Defaults to null<br />Should be specified with a spell type of "Prompt", this is the prompt the spell will offer
    ///     when used in game
    /// </summary>
    public string? Prompt { get; set; }
    /// <summary>
    ///     The way the spell is cast by the player
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public SpellType SpellType { get; set; }
}