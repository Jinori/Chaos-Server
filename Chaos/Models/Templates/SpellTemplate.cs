using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Templates.Abstractions;

namespace Chaos.Models.Templates;

public sealed record SpellTemplate : PanelEntityTemplateBase
{
    public required WizardElement? WizardElement { get; init; }
    
    public required SpellSchools? SpellSchools { get; init; }
    public required byte CastLines { get; init; }
    public required LearningRequirements? LearningRequirements { get; init; }
    public required bool LevelsUp { get; init; }
    public required byte MaxLevel { get; init; }
    public required string? Prompt { get; set; }
    public required SpellType SpellType { get; init; }
}