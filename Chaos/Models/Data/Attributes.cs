// ReSharper disable InconsistentNaming

using System.Text;
using Chaos.DarkAges.Extensions;

namespace Chaos.Models.Data;

public record Attributes : Stats
{
    protected int _ac;
    protected int _atkSpeedPct;
    protected int _cooldownReduction;
    protected int _cooldownReductionPct;
    protected int _dmg;
    protected int _flatSkillDamage;
    protected int _flatSpellDamage;
    protected int _healBonus;
    protected int _healBonusPct;
    protected int _hit;
    protected int _magicResistance;
    protected int _maximumHp;
    protected int _maximumMp;
    protected int _skillDamagePct;
    protected int _spellDamagePct;

    public int Ac
    {
        get => _ac;
        init => _ac = value;
    }

    public int AtkSpeedPct
    {
        get => _atkSpeedPct;
        init => _atkSpeedPct = value;
    }

    public int CooldownReduction
    {
        get => _cooldownReduction;
        init => _cooldownReduction = value;
    }

    public int CooldownReductionPct
    {
        get => _cooldownReductionPct;
        init => _cooldownReductionPct = value;
    }

    public int Dmg
    {
        get => _dmg;
        init => _dmg = value;
    }

    public int FlatSkillDamage
    {
        get => _flatSkillDamage;
        set => _flatSkillDamage = value;
    }

    public int FlatSpellDamage
    {
        get => _flatSpellDamage;
        set => _flatSpellDamage = value;
    }

    public int HealBonus
    {
        get => _healBonus;
        init => _healBonus = value;
    }

    public int HealBonusPct
    {
        get => _healBonusPct;
        init => _healBonusPct = value;
    }

    public int Hit
    {
        get => _hit;
        init => _hit = value;
    }

    public int MagicResistance
    {
        get => _magicResistance;
        init => _magicResistance = value;
    }

    public int MaximumHp
    {
        get => _maximumHp;
        init => _maximumHp = value;
    }

    public int MaximumMp
    {
        get => _maximumMp;
        init => _maximumMp = value;
    }

    public int SkillDamagePct
    {
        get => _skillDamagePct;
        set => _skillDamagePct = value;
    }

    public int SpellDamagePct
    {
        get => _spellDamagePct;
        set => _spellDamagePct = value;
    }

    public virtual void Add(Attributes other)
    {
        Interlocked.Add(ref _ac, other.Ac);
        Interlocked.Add(ref _dmg, other.Dmg);
        Interlocked.Add(ref _hit, other.Hit);
        Interlocked.Add(ref _str, other.Str);
        Interlocked.Add(ref _int, other.Int);
        Interlocked.Add(ref _wis, other.Wis);
        Interlocked.Add(ref _con, other.Con);
        Interlocked.Add(ref _dex, other.Dex);
        Interlocked.Add(ref _magicResistance, other.MagicResistance);
        Interlocked.Add(ref _maximumHp, other.MaximumHp);
        Interlocked.Add(ref _maximumMp, other.MaximumMp);
        Interlocked.Add(ref _atkSpeedPct, other.AtkSpeedPct);
        Interlocked.Add(ref _skillDamagePct, other.SkillDamagePct);
        Interlocked.Add(ref _spellDamagePct, other.SpellDamagePct);
        Interlocked.Add(ref _flatSkillDamage, other.FlatSkillDamage);
        Interlocked.Add(ref _flatSpellDamage, other.FlatSpellDamage);
        Interlocked.Add(ref _cooldownReductionPct, other.CooldownReductionPct);
        Interlocked.Add(ref _healBonusPct, other.HealBonusPct);
        Interlocked.Add(ref _cooldownReduction, other.CooldownReduction);
        Interlocked.Add(ref _healBonus, other.HealBonus);
    }

    public virtual void Subtract(Attributes other)
    {
        Interlocked.Add(ref _ac, -other.Ac);
        Interlocked.Add(ref _dmg, -other.Dmg);
        Interlocked.Add(ref _hit, -other.Hit);
        Interlocked.Add(ref _str, -other.Str);
        Interlocked.Add(ref _int, -other.Int);
        Interlocked.Add(ref _wis, -other.Wis);
        Interlocked.Add(ref _con, -other.Con);
        Interlocked.Add(ref _dex, -other.Dex);
        Interlocked.Add(ref _magicResistance, -other.MagicResistance);
        Interlocked.Add(ref _maximumHp, -other.MaximumHp);
        Interlocked.Add(ref _maximumMp, -other.MaximumMp);
        Interlocked.Add(ref _atkSpeedPct, -other.AtkSpeedPct);
        Interlocked.Add(ref _skillDamagePct, -other.SkillDamagePct);
        Interlocked.Add(ref _spellDamagePct, -other.SpellDamagePct);
        Interlocked.Add(ref _flatSkillDamage, -other.FlatSkillDamage);
        Interlocked.Add(ref _flatSpellDamage, -other.FlatSpellDamage);
        Interlocked.Add(ref _cooldownReductionPct, other.CooldownReductionPct);
        Interlocked.Add(ref _healBonusPct, other.HealBonusPct);
        Interlocked.Add(ref _cooldownReduction, other.CooldownReduction);
        Interlocked.Add(ref _healBonus, other.HealBonus);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        //append all properties that are not 0
        var sb = new StringBuilder();

        sb.Append(base.ToString());
        
        if (Ac != 0)
            sb.AppendLineF($"AC: {Ac}");
        
        if (AtkSpeedPct != 0)
            sb.AppendLineF($"Attack Speed%: {AtkSpeedPct}");
        
        if (CooldownReduction != 0)
            sb.AppendLineF($"Cooldown Reduction: {CooldownReduction}");
        
        if (CooldownReductionPct != 0)
            sb.AppendLineF($"Cooldown Reduction%: {CooldownReductionPct}");
        
        if (Dmg != 0)
            sb.AppendLineF($"DMG: {Dmg}");
        
        if (FlatSkillDamage != 0)
            sb.AppendLineF($"Flat Skill DMG: {FlatSkillDamage}");
        
        if (FlatSpellDamage != 0)
            sb.AppendLineF($"Flat Spell DMG: {FlatSpellDamage}");
        
        if (HealBonus != 0)
            sb.AppendLineF($"Heal Bonus: {HealBonus}");
        
        if (HealBonusPct != 0)
            sb.AppendLineF($"Heal Bonus%: {HealBonusPct}");
        
        if (Hit != 0)
            sb.AppendLineF($"HIT: {Hit}");
        
        if (MagicResistance != 0)
            sb.AppendLineF($"MR: {MagicResistance}");
        
        if (MaximumHp != 0)
            sb.AppendLineF($"HP: {MaximumHp}");
        
        if (MaximumMp != 0)
            sb.AppendLineF($"MP: {MaximumMp}");
        
        if (SkillDamagePct != 0)
            sb.AppendLineF($"Skill DMG%: {SkillDamagePct}");
        
        if (SpellDamagePct != 0)
            sb.AppendLineF($"Spell DMG%: {SpellDamagePct}");

        return sb.ToString();
    }
}