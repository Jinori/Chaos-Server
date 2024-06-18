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
    public class PietWerewolfEmptyRoomMapScript : MapScriptBase
    {
        private readonly IMerchantFactory MerchantFactory;
        private readonly IMonsterFactory MonsterFactory;
        private readonly IEffectFactory EffectFactory;
        private readonly IIntervalTimer DelayedStartTimer;
        private readonly ISimpleCache SimpleCache;
        private ScriptState State;
        private readonly IIntervalTimer UpdateTimer;
        public const int UPDATE_INTERVAL_MS = 1000;

        public PietWerewolfEmptyRoomMapScript(MapInstance subject, IMonsterFactory monsterFactory, ISimpleCache simpleCache, IMerchantFactory merchantFactory, IEffectFactory effectFactory)
            : base(subject)
        {
            EffectFactory = effectFactory;
            MonsterFactory = monsterFactory;
            SimpleCache = simpleCache;
            MerchantFactory = merchantFactory;
            DelayedStartTimer = new IntervalTimer(TimeSpan.FromSeconds(20), false);
            UpdateTimer = new IntervalTimer(TimeSpan.FromMilliseconds(UPDATE_INTERVAL_MS), false);
        }

        public override void OnEntered(Creature creature)
        {
            if (creature is not Aisling aisling)
                return;

            // Check if the player has a specific quest flag and the trial is not already in progress
            if (aisling.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant) && State == ScriptState.Dormant)
            {
                // Start the combat trial
                var point = new Point(10, 7);
                var werewolftoby = MerchantFactory.Create("werewolftoby", Subject, point);
                Subject.AddEntity(werewolftoby, point);
                StartTimers();
                State = ScriptState.DelayedStart;
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
                
                HandleScriptState();
            }
        }

        private void HandleScriptState()
        {
            switch (State)
            {
                case ScriptState.Dormant:
                    if (Subject.GetEntities<Aisling>()
                        .Any(a => a.Trackers.Enums.HasValue(WerewolfOfPiet.FollowedMerchant)))
                    {
                        State = ScriptState.DelayedStart;
                    }

                    break;

                case ScriptState.DelayedStart:
                    if (DelayedStartTimer.IntervalElapsed)
                        State = ScriptState.SpawningWerewolf;

                    break;

                case ScriptState.SpawningWerewolf:
                    if (Subject.GetEntities<Monster>().Any(x => x.Name == "Werewolf"))
                    {
                        State = ScriptState.SpawnedWerewolf;
                    }
                    break;

                case ScriptState.SpawnedWerewolf:
                    if (CheckAllMonstersCleared() && Subject.GetEntities<Aisling>().Any(x => x.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed)))
                    {
                        State = ScriptState.CompletedWerewolf;
                    }

                    break;

                case ScriptState.CompletedWerewolf:
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
                var toby = Subject.GetEntities<Merchant>().Where(x => x.Name.Equals("Toby"));
                foreach (var merchant in toby) Subject.RemoveEntity(merchant);

                var point = new Point(11, 6);
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
                // Add the monster to the subject
                Subject.AddEntity(monster, monster);
            }
        }

        private bool CheckAllMonstersCleared()
        {
            if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>()))
            {
                State = ScriptState.CompletedWerewolf;

                return true;
            }

            return false;
        }

        private void HandleCompletedWerewolf()
        {
            if (!Subject.GetEntities<Aisling>().Any())
            {
                ClearMonsters();
                State = ScriptState.Dormant;
            }
            else if (!Subject.GetEntities<Monster>().Any(m => !m.Script.Is<PetScript>()))
            {
                foreach (var aisling in Subject.GetEntities<Aisling>())
                {
                    var effect = EffectFactory.Create("Werewolf");
                    aisling.Effects.Apply(aisling, effect);
                    aisling.Trackers.Enums.Set(WerewolfOfPiet.KilledandGotCursed);
                    aisling.SendOrangeBarMessage("You defeated the Werewolf but his curse lingers on...");
                    var mapinstance = SimpleCache.Get<MapInstance>("piet_empty_room");
                    var point = new Point(aisling.X, aisling.Y);
                    aisling.TraverseMap(mapinstance, point);
                }
                
                State = ScriptState.Dormant;
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
            State = ScriptState.DelayedStart; // Set the state to DelayedStart to begin the trial
            UpdateTimer.Reset(); // Reset the update timer
        }
    }

    public enum ScriptState
    {
        Dormant,
        DelayedStart,
        SpawningWerewolf,
        SpawnedWerewolf,
        CompletedWerewolf
    }
}
