using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.SkillScripts.Abstractions;

public abstract class ConfigurableSkillScriptBase : ConfigurableScriptBase<Skill>, ISkillScript
{
    /// <inheritdoc />
    protected ConfigurableSkillScriptBase(Skill subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool CanUse(ActivationContext context) => context.Source.IsAlive;

    /// <inheritdoc />
    public virtual void OnForgotten(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnLearned(Aisling aisling) { }

    /// <inheritdoc />
    public virtual void OnUse(ActivationContext context) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}