using Chaos.Collections;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Extensions;

public static class StatusExtensions
{

    public static bool IsPureRogueMaster(this Aisling aisling) => aisling.Legend.ContainsKey("rogueClass") &&
                                                            !aisling.Legend.ContainsKey("dedicated") &&
                                                            aisling.UserStatSheet.Master;
    
    public static bool IsPurePriestMaster(this Aisling aisling) => aisling.Legend.ContainsKey("priestClass") &&
                                                            !aisling.Legend.ContainsKey("dedicated") &&
                                                            aisling.UserStatSheet.Master;
    
    public static bool IsPureMonkMaster(this Aisling aisling) => aisling.Legend.ContainsKey("monkClass") &&
                                                            !aisling.Legend.ContainsKey("dedicated") &&
                                                            aisling.UserStatSheet.Master;
    
    public static bool IsPureWarriorMaster(this Aisling aisling) => aisling.Legend.ContainsKey("warriorClass") &&
                                                                 !aisling.Legend.ContainsKey("dedicated") &&
                                                                 aisling.UserStatSheet.Master;
    
    public static bool IsPureWizardMaster(this Aisling aisling) => aisling.Legend.ContainsKey("wizardClass") &&
                                                                 !aisling.Legend.ContainsKey("dedicated") &&
                                                                 aisling.UserStatSheet.Master;
    
    public static bool IsGodModeEnabled(this Creature creature) => creature.Trackers.Enums.HasValue(GodMode.Yes);
    public static bool IsPramhed(this Creature creature) => creature.Effects.Contains("beagpramh") || creature.Effects.Contains("pramh");
    public static bool IsSuained(this Creature creature) => creature.Effects.Contains("suain") || creature.Effects.Contains("wolffangfist");
    public static bool IsAsgalled(this Creature creature) => creature.Effects.Contains("asgallfaileas");
    public static bool IsCradhLocked(this Creature creature) => creature.Effects.Contains("preventaffliction");
    public static bool IsClawFisted(this Creature creature) => creature.Effects.Contains("clawfist");
    public static bool IsBeagSuained(this Creature creature) => creature.Effects.Contains("beagsuain") || creature.Effects.Contains("earthpunch");
    
    public static bool IsRooted(this Creature creature) => creature.Effects.Contains("root");
    public static bool IsBattleCried(this Creature creature) => creature.Effects.Contains("battlecry");
    public static bool IsInnerFired(this Creature creature) => creature.Effects.Contains("innerfire");
    public static bool IsRuminating(this Creature creature) => creature.Effects.Contains("rumination");
    public static bool IsDetectingTraps(this Creature creature) => creature.Effects.Contains("detecttraps");
    public static bool IsEarthenStanced(this Creature creature) => creature.Effects.Contains("earthenstance");
    
    public static bool IsRockStanced(this Creature creature) => creature.Effects.Contains("rockstance");
    public static bool IsMistStanced(this Creature creature) => creature.Effects.Contains("miststance");
    public static bool IsTideStanced(this Creature creature) => creature.Effects.Contains("tidestance");
    public static bool IsThunderStanced(this Creature creature) => creature.Effects.Contains("thunderstance");
    public static bool IsLightningStanced(this Creature creature) => creature.Effects.Contains("lightningstance");
    public static bool IsSmokeStanced(this Creature creature) => creature.Effects.Contains("smokestance");
    public static bool IsFlameStanced(this Creature creature) => creature.Effects.Contains("flamestance");
    public static bool IsInLastStand(this Creature creature) => creature.Effects.Contains("laststand");
    public static bool IsBeagAited(this Creature creature) => creature.Effects.Contains("beag naomh aite");
    public static bool IsAited(this Creature creature) => creature.Effects.Contains("naomh aite");
    public static bool IsMorAited(this Creature creature) => creature.Effects.Contains("mor naomh aite");
    public static bool IsArdAited(this Creature creature) => creature.Effects.Contains("ard naomh aite");
    
    public static bool IsBlessed(this Creature creature) => creature.Effects.Contains("blessing");
    public static bool IsHidden(this Creature creature) => creature.Effects.Contains("hide") || creature.Effects.Contains("gmhide") || creature.Effects.Contains("truehide");
    public static bool IsAmnesiad(this Creature creature) => creature.Effects.Contains("amnesia");
    public static bool IsCradhPrevented(this Creature creature) => creature.Effects.Contains("preventrecradh");
    public static bool IsDiacht(this Aisling aisling) => aisling.UserStatSheet.BaseClass == BaseClass.Diacht;
}