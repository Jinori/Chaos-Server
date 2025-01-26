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
    public ICollection<AbilityRequirementSchema> PrerequisiteSkills { get; set; } = Array.Empty<AbilityRequirementSchema>();

    /// <summary>
    ///     The spells that must be learned before this ability can be learned
    /// </summary>
    public ICollection<AbilityRequirementSchema> PrerequisiteSpells { get; set; } = Array.Empty<AbilityRequirementSchema>();

    /// <summary>
    ///     The amount of gold required to learn this ability
    /// </summary>
    public int? RequiredGold { get; set; }

    /// <summary>
    ///     The attributes required to learn this skill
    /// </summary>
    public StatsSchema? RequiredStats { get; set; }

    /// <summary>
    ///     The spell to remove when learning this spell
    /// </summary>
    public string? SkillSpellToUpgrade { get; set; }

    /// <summary>
    ///     incase there's a second ability you'd like to remove.
    /// </summary>
    public string? SkillSpellToUpgrade2 { get; set; }
}