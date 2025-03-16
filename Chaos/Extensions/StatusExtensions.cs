using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;

namespace Chaos.Extensions;

public static class StatusExtensions
{
    public static bool IsOnline(this Aisling aisling, IClientRegistry<IChaosWorldClient> clientRegistry) =>
        clientRegistry.Select(client => client.Aisling).Any(a => a.Name.EqualsI(aisling.Name));

    public static bool IsAited(this Creature creature) => creature.Effects.Contains("Naomh Aite");
    public static bool IsAmnesiad(this Creature creature) => creature.Effects.Contains("Amnesia");
    public static bool IsArdAited(this Creature creature) => creature.Effects.Contains("Ard Naomh Aite");
    public static bool IsAsgalled(this Creature creature) => creature.Effects.Contains("Asgall Faileas");
    public static bool IsBattleCried(this Creature creature) => creature.Effects.Contains("Battle Cry");
    public static bool IsBeagAited(this Creature creature) => creature.Effects.Contains("Beag Naomh Aite");
    public static bool IsBeagSuained(this Creature creature) =>
        creature.Effects.Contains("beagsuain") || creature.Effects.Contains("Earth Punch");
    public static bool IsBlessed(this Creature creature) => creature.Effects.Contains("Blessing");
    public static bool IsClawFisted(this Creature creature) => creature.Effects.Contains("Claw Fist");
    public static bool IsConfused(this Creature creature) => creature.Effects.Contains("bewilderment");
    public static bool IsCradhLocked(this Creature creature) => creature.Effects.Contains("Prevent Affliction");
    public static bool IsCradhPrevented(this Creature creature) => creature.Effects.Contains("Prevent Recradh");
    public static bool IsDetectingTraps(this Creature creature) => creature.Effects.Contains("Detect Traps");
    public static bool IsDiacht(this Aisling aisling) => aisling.UserStatSheet.BaseClass == BaseClass.Diacht;
    public static bool IsEarthenStanced(this Creature creature) => creature.Effects.Contains("Earthen Stance");
    public static bool IsFlameStanced(this Creature creature) => creature.Effects.Contains("Flame Stance");
    public static bool IsGodModeEnabled(this Creature creature) => creature.Trackers.Enums.HasValue(GodMode.Yes);
    public static bool IsHidden(this Creature creature) =>
        creature.Effects.Contains("hide") || creature.Effects.Contains("gm hide") || creature.Effects.Contains("true hide");
    public static bool IsHotChocolated(this Creature creature) => creature.Effects.Contains("Hot Chocolate");
    public static bool IsInLastStand(this Creature creature) => creature.Effects.Contains("Last Stand");
    public static bool IsInnerFired(this Creature creature) => creature.Effects.Contains("Inner Fire");
    public static bool IsLightningStanced(this Creature creature) => creature.Effects.Contains("Lightning Stance");
    public static bool IsMistStanced(this Creature creature) => creature.Effects.Contains("Mist Stance");
    public static bool IsMorAited(this Creature creature) => creature.Effects.Contains("Mor Naomh Aite");
    public static bool IsPramhed(this Creature creature) => creature.Effects.Contains("Beag Pramh") || creature.Effects.Contains("Pramh");
    public static bool IsPureMonkMaster(this Aisling aisling) =>
        aisling.Legend.ContainsKey("monkClass") && !aisling.Legend.ContainsKey("dedicated") && aisling.UserStatSheet.Master;
    public static bool IsPurePriestMaster(this Aisling aisling) =>
        aisling.Legend.ContainsKey("priestClass") && !aisling.Legend.ContainsKey("dedicated") && aisling.UserStatSheet.Master;
    public static bool IsPureRogueMaster(this Aisling aisling) =>
        aisling.Legend.ContainsKey("rogueClass") && !aisling.Legend.ContainsKey("dedicated") && aisling.UserStatSheet.Master;
    public static bool IsPureWarriorMaster(this Aisling aisling) =>
        aisling.Legend.ContainsKey("warriorClass") && !aisling.Legend.ContainsKey("dedicated") && aisling.UserStatSheet.Master;
    public static bool IsPureWizardMaster(this Aisling aisling) =>
        aisling.Legend.ContainsKey("wizardClass") && !aisling.Legend.ContainsKey("dedicated") && aisling.UserStatSheet.Master;
    public static bool IsRockStanced(this Creature creature) => creature.Effects.Contains("Rock Stance");
    public static bool IsRooted(this Creature creature) => creature.Effects.Contains("Root");
    public static bool IsRuminating(this Creature creature) => creature.Effects.Contains("Rumination");
    public static bool IsSmokeStanced(this Creature creature) => creature.Effects.Contains("Smoke Stance");
    public static bool IsStoned(this Creature creature) => creature.Effects.Contains("Stoned");
    public static bool IsSuained(this Creature creature) =>
        creature.Effects.Contains("Suain") || creature.Effects.Contains("Wolf Fang Fist");

    public static bool IsIntimidated(this Creature creature) => creature.Effects.Contains("Intimidate");
    public static bool IsThunderStanced(this Creature creature) => creature.Effects.Contains("Thunder Stance");
    public static bool IsTideStanced(this Creature creature) => creature.Effects.Contains("Tide Stance");
    public static bool IsTormented(this Creature creature) => creature.Effects.Contains("Torment");
}