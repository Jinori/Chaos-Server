using Chaos.Schemas.Aisling;

namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of the requirements of an ability
/// </summary>
public sealed record LearningRequirementsSchema
{
    /// <summary>
    ///     The items and their amounts required to learn this ability
    /// </summary>
    public ICollection<ItemRequirementSchema> ItemRequirements { get; set; } = Array.Empty<ItemRequirementSchema>();

    /// <summary>
    ///     The skills that must be learned before this ability can be learned
    /// </summary>
    public ICollection<string> PrerequisiteSkillTemplateKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The spells that must be learned before this ability can be learned
    /// </summary>
    public ICollection<string> PrerequisiteSpellTemplateKeys { get; set; } = Array.Empty<string>();

    /// <summary>
    ///     The amount of gold required to learn this ability
    /// </summary>
    public int? RequiredGold { get; set; }

    /// <summary>
    ///     The attributes required to learn this skill
    /// </summary>
    public StatsSchema? RequiredStats { get; set; }
}