using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.MainStory.EarthGuardian;

public sealed class EarthGuardianBossGroupScalingScript(Monster subject, ISpellFactory spellFactory)
    : MonsterScriptBase(subject)
{
    private static bool _groupBonusApplied;

    private void AddSpellsBasedOnGroupLevel(IEnumerable<int> groupLevel)
    {
        var averageLevel = groupLevel.Average();

        switch (averageLevel)
        {
            case > 40 and < 55:
                AddSpellsToBoss("creaglamh", "cradh");

                break;
            case > 55:
                AddSpellsToBoss("morcreaglamh", "morcradh");

                break;
        }
    }

    private void AddSpellsToBoss(string spellName1, string spellName2)
    {
        Subject.Spells.Add(spellFactory.Create(spellName1));
        Subject.Spells.Add(spellFactory.Create(spellName2));
    }

    private Attributes CreateGroupBonusAttributes(IReadOnlyCollection<int> groupLevel)
    {
        var averageLevel = (int)groupLevel.Average();
        var groupCount = groupLevel.Count;

        return new Attributes
        {
            Con = averageLevel,
            Dex = averageLevel,
            Int = averageLevel + 15,
            Str = averageLevel + 25,
            Wis = averageLevel,
            AtkSpeedPct = groupCount * 4,
            MaximumHp = averageLevel * groupCount * 1000,
            MaximumMp = averageLevel * groupCount * 1000,
            SkillDamagePct = groupCount * 3,
            SpellDamagePct = groupCount * 3
        };
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        // If bonus is already applied or there's no valid target, return
        if (_groupBonusApplied || (Target == null) || !ShouldMove || !Map.GetEntities<Aisling>().Any())
            return;

        var groupLevel = Map.GetEntitiesWithinRange<Aisling>(Subject, 12).Select(aisling => aisling.StatSheet.Level).ToList();

        // Create bonus attributes based on the group level
        var attrib = CreateGroupBonusAttributes(groupLevel);

        // Check the group level and add spells accordingly
        AddSpellsBasedOnGroupLevel(groupLevel);

        // Add the attributes to the monster
        Subject.StatSheet.AddBonus(attrib);
        // Add HP and MP to the monster
        Subject.StatSheet.SetHealthPct(100);
        Subject.StatSheet.SetManaPct(100);
        _groupBonusApplied = true;
    }
}