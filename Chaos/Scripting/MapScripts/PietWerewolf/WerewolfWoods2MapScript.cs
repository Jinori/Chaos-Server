using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.MapScripts.PietWerewolf
{
    public class WerewolfWoods2MapScript : MapScriptBase
    {
        private readonly IMerchantFactory MerchantFactory;
        private readonly IMonsterFactory MonsterFactory;
        private readonly IIntervalTimer DelayedStartTimer;
        private readonly ISimpleCache SimpleCache;
        private ScriptState2 State;
        private readonly IIntervalTimer UpdateTimer;
        public const int UPDATE_INTERVAL_MS = 1000;

        public WerewolfWoods2MapScript(MapInstance subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache, IMerchantFactory merchantFactory, IEffectFactory effectFactory)
            : base(subject)
        {
            MonsterFactory = monsterFactory;
            SimpleCache = simpleCache;
            MerchantFactory = merchantFactory;
            DelayedStartTimer = new IntervalTimer(TimeSpan.FromSeconds(14), false);
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard) && State == ScriptState2.Dormant)
            {
                aisling.SendOrangeBarMessage("The woods behind you close off the way out.");
                
                if (Subject.GetEntities<Merchant>().Any(x => x.Name == "Master Werewolf"))
                    return;
                
                var point = new Point(13, 9);
                var masterwerewolf = MerchantFactory.Create("masterwerewolfmerchant", Subject, point);
                Subject.AddEntity(masterwerewolf, point);
                StartTimers();
                State = ScriptState2.DelayedStart;
            }
        }

        public override void Update(TimeSpan delta)
        {
            UpdateTimer.Update(delta);
            DelayedStartTimer.Update(delta);

            if (UpdateTimer.IntervalElapsed)
            {
                if (!Subject.GetEntities<Aisling>().Any())
                    return;
                
                HandleScriptState2();
            }
        }

        private void HandleScriptState2()
        {
            switch (State)
            {
                case ScriptState2.Dormant:
                    if (Subject.GetEntities<Aisling>()
                        .Any(a => a.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)))
                    {
                        State = ScriptState2.DelayedStart;
                    }

                    break;

                case ScriptState2.DelayedStart:
                    if (DelayedStartTimer.IntervalElapsed)
                        State = ScriptState2.SpawningWerewolf;

                    break;

                case ScriptState2.SpawningWerewolf:
                    SpawnMonsters("masterwerewolf", 1);
                    State = ScriptState2.SpawnedWerewolf;
                    break;

                case ScriptState2.SpawnedWerewolf:
                    if (CheckAllMonstersCleared())
                    {
                        State = ScriptState2.CompletedWerewolf;
                    }

                    break;

                case ScriptState2.CompletedWerewolf:
                    HandleCompletedWerewolf();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SpawnMonsters(string monsterType, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var point = new Point(13, 9);
                var groupLevel = Subject.GetEntities<Aisling>().Select(aisling => aisling.StatSheet.Level).ToList();
                var monster = MonsterFactory.Create(monsterType, Subject, point);

                // Calculate the excess vitality over 20,000 for any player
                var excessVitality = Subject.GetEntities<Aisling>()
                    .Where(aisling => !aisling.Trackers.Enums.HasValue(GodMode.Yes))
                    .Select(aisling => (aisling.StatSheet.MaximumHp + aisling.StatSheet.MaximumMp) - 20000)
                    .Where(excess => excess > 0)
                    .Sum(excess => excess / 1000);

                var attrib = new Attributes
                {
                    AtkSpeedPct = groupLevel.Count * 8 + 3,
                    MaximumHp = (int)(monster.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 600),
                    MaximumMp = (int)(monster.StatSheet.MaximumHp + groupLevel.Average() * groupLevel.Count * 600),
                    Int = (int)(monster.StatSheet.Int + groupLevel.Average() * groupLevel.Count / 8),
                    Str = (int)(monster.StatSheet.Str + groupLevel.Average() * groupLevel.Count / 6),
                    SkillDamagePct = (int)(monster.StatSheet.SkillDamagePct + groupLevel.Average() / 3 +
                                           groupLevel.Count + 20),
                    SpellDamagePct = (int)(monster.StatSheet.SpellDamagePct + groupLevel.Average() / 3 +
                                           groupLevel.Count + 20)
                };

                // Adjust the attributes based on the excess vitality
                if (excessVitality > 0)
                {
                    // Example adjustments: increase stats by a percentage of the excess vitality
                    attrib = attrib with
                    {
                        MaximumHp = (int)(excessVitality * 0.05)
                    }; // 5% of excess vitality added to HP
                    attrib = attrib with
                    {
                        MaximumMp = (int)(excessVitality * 0.05)
                    }; // 5% of excess vitality added to MP
                    attrib.Int += (int)(excessVitality * 0.01); // 1% of excess vitality added to Int
                    attrib.Str += (int)(excessVitality * 0.01); // 1% of excess vitality added to Str
                    attrib.SkillDamagePct +=
                        (int)(excessVitality * 0.01); // 1% of excess vitality added to Skill Damage
                    attrib.SpellDamagePct +=
                        (int)(excessVitality * 0.01); // 1% of excess vitality added to Spell Damage
                }

                // Add the attributes to the monster
                monster.StatSheet.AddBonus(attrib);
                // Add HP and MP to the monster
                monster.StatSheet.SetHealthPct(100);
                monster.StatSheet.SetManaPct(100);
                
                var masterwerewolf = Subject.GetEntities<Merchant>().Where(x => x.Name == "Master Werewolf").ToList();
                foreach (var merchant in masterwerewolf)
                    Subject.RemoveEntity(merchant);
                // Add the monster to the subject
                Subject.AddEntity(monster, monster);
            }
        }

        private bool CheckAllMonstersCleared()
        {
            if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>()))
            {
                State = ScriptState2.CompletedWerewolf;

                return true;
            }

            return false;
        }

        private void HandleCompletedWerewolf()
        {
            if (!Subject.GetEntities<Aisling>().Any())
            {
                ClearMonsters();
                State = ScriptState2.Dormant;
            }
            else if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>()))
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    if (aisling.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard))
                    {
                        aisling.Trackers.Enums.Set(WerewolfOfPiet.KilledWerewolf);   
                    }
                    aisling.SendOrangeBarMessage("You defeated the Master Werewolf.");
                    var mapinstance = SimpleCache.Get<MapInstance>("werewolf_woods2");
                    var point = new Point(aisling.X, aisling.Y);
                    aisling.TraverseMap(mapinstance, point);
                }
                
                State = ScriptState2.Dormant;
            }
        }

        private void ClearMonsters()
        {
            foreach (var monster in Subject.GetEntities<Monster>())
            {
                Subject.RemoveEntity(monster);
            }
        }

        public void StartTimers()
        {
            State = ScriptState2.DelayedStart; // Set the state to DelayedStart to begin the trial
            UpdateTimer.Reset(); // Reset the update timer
        }
    }

    public enum ScriptState2
    {
        Dormant,
        DelayedStart,
        SpawningWerewolf,
        SpawnedWerewolf,
        CompletedWerewolf
    }
}
