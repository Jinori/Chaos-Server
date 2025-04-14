using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.DubhaimCastle;

public class DubhaimAltarScript(ReactorTile subject, IItemFactory itemFactory, IMonsterFactory monsterFactory)
    : ReactorTileScriptBase(subject)
{
    private readonly IMonsterFactory MonsterFactory = monsterFactory;
    
    private static readonly Dictionary<string, SummonInfo> SUMMONABLES = new()
    { 
        ["blood_diamond"]       = new SummonInfo("dc_kindlefiend",  true, true,  "Kindlefiend forges itself from the Blood Diamond!"),
["centipedegland"]      = new SummonInfo("crypt_centipede5",  false,false,  "A centipede forms around the Centipede Gland!"),
["koboldskull"]         = new SummonInfo("astrid_koboldc",    false,false,  "A kobold rises from the Kobold Skull's lingering magic!"),
["mantiseye"]           = new SummonInfo("ew_mantis4",        false,false,  "From the glassy Mantis Eye, a mantis rebuilds itself!"),
["rawhoney"]            = new SummonInfo("ew_bee3",           false,false,  "A bee forms from the enchanted Raw Honey!"),
["rawwax"]              = new SummonInfo("ew_bee3",           false,false,  "Molten Raw Wax molds itself into wings— a bee reborn!"),
["vipergland"]          = new SummonInfo("eg_viper",          false,false,  "A viper grows from the toxic Viper Gland!"),
["wolflock"]            = new SummonInfo("ew_wolf5",          false,false,  "A wolf knits itself from the ragged Wolf Lock!"),
["wolfteeth"]           = new SummonInfo("ew_wolf5",          false,false,  "Wolf Teeth fuse together, birthing a snarling wolf!"),
["blackshockerpiece"]   = new SummonInfo("nm_blackshocker",   false,true,  "A black shocker crackles to life from its Shocker Piece!"),
["blueshockerpiece"]    = new SummonInfo("nm_blueshocker",    false,true,  "A blue shocker sparks alive from the Blue Shocker Piece!"),
["direwolflock"]        = new SummonInfo("mg_direwolf",       false,true,  "Dire Wolf fur twists into muscle; a dire wolf lumbers forth!"),
["goldshockerpiece"]    = new SummonInfo("nm_goldshocker",    false,true,  "A gold shocker hums into being from the Gold Shocker Piece!"),
["iceelementalflame"]   = new SummonInfo("mg_iceelemental",   false,true,  "Frostfire swirls; an ice elemental condenses from its Flame!"),
["iceskeletonskull"]    = new SummonInfo("mg_iceskeleton",    false,true,  "An ice skeleton rises from the frozen Skeleton Skull!"),
["icesporesac"]         = new SummonInfo("mg_icespore",       false,true,  "An ice spore blooms from the Ice Spore Sac!"),
["losganntail"]         = new SummonInfo("or_losgann",        false,true,  "The Losgann Tail thrashes— a losgann leaps alive!"),
["redshockerpiece"]     = new SummonInfo("nm_redshocker",     false,true,  "A red shocker surges from the Red Shocker Piece!"),
["ruidhteartoe"]        = new SummonInfo("or_ruidhtear",      false,true,  "Arcane mist swirls; a ruidhtear reforms around its Toe!"),
["batwing"]             = new SummonInfo("crypt_bat7",        false,false,  "Tattered Bat Wing stitches together, summoning a bat!"),
["giantbatwing"]        = new SummonInfo("crypt_giantbat10",  false,false,  "A giant bat materializes from its shadowy Wing!"),
["kardifur"]            = new SummonInfo("crypt_kardi15",     false,false,  "The Kardi Fur writhes— a kardi prowler revives!"),
["marauderspine"]       = new SummonInfo("crypt_marauder15",  false,false,  "A marauder rises, rebuilt around its Spine!"),
["mimicteeth"]          = new SummonInfo("crypt_mimic11",     false,false,  "Mimic Teeth snap together, forming a hungry mimic!"),
["scorpionsting"]       = new SummonInfo("crypt_scorpion9",   false,false,  "Toxic barbs sprout— a scorpion forms from its Sting!"),
["scorpiontail"]        = new SummonInfo("crypt_scorpion9",   false,false,  "The Scorpion Tail coils and spawns a full scorpion!"),
["spidereye"]           = new SummonInfo("crypt_spider3",     false,false,  "Silken threads knit from the Spider Eye, shaping a spider!"),
["spidersilk"]          = new SummonInfo("crypt_spider3",     false,false,  "Living Spider Silk weaves a spider’s body anew!"),
["succubushair"]        = new SummonInfo("crypt_succubus15",  false,false,  "Succubus Hair coils, shaping a seductive succubus!"),
["whitebatwing"]        = new SummonInfo("crypt_whitebat13",  false,false,  "Pale membrane reforms; a white bat glides forth!"),
["giantantwing"]        = new SummonInfo("pf_giant_ant",      false,false,  "A giant ant rebuilds itself from the Ant Wing!"),
["silverwolfmanehair"]  = new SummonInfo("pf_silver_wolf",    false,false,  "Silver Wolf Mane weaves into a silver wolf!"),
["trentroot"]           = new SummonInfo("pf_tree_demon",     false,false,  "The Trent Root sprouts into a raging tree demon!"),
["ancientbone"]         = new SummonInfo("bone_collector",    false,false,  "Powdery Ancient Bone spins into a towering Bone Collector!"),
["anemoneantenna"]      = new SummonInfo("sewer_anemone6",    false,false,  "Tendrils unfurl; an anemone regenerates from its Antenna!"),
["beesting"]            = new SummonInfo("ew_bee3",           false,false,  "Venom crystallizes— a bee reforms around the Bee Sting!"),
["blackcattail"]        = new SummonInfo("black_cat",         false,false,  "Midnight fur flows; a black cat reappears tail‑first!"),
["blackwidowsilk"]      = new SummonInfo("black_widow",       false,false,  "Black Widow Silk twists into a venomous widow!"),
["brawlfishscale"]      = new SummonInfo("sewer_brawlfish6",  false,false,  "A brawlfish emerges from the Brawl Fish Scale!"),
["crabclaw"]            = new SummonInfo("sewer_crab3",       false,false,  "The Crab Claw clicks— a crab rebuilds its shell!"),
["crabshell"]           = new SummonInfo("sewer_crab3",       false,false,  "Fragments of Crab Shell snap together, reviving a crab!"),
["faeriewing"]          = new SummonInfo("ww_faerie2",        false,false,  "Pixie dust spins a faerie from the Faerie Wing!"),
["flesh"]               = new SummonInfo("flesh_golem10",     false,false,  "Quivering Flesh stacks— a grotesque golem stands anew!"),
["fomorianrag"]         = new SummonInfo("fomorian_horror19", false,false,  "The Fomorian Rag reknits into a hulking horror!"),
["frogleg"]             = new SummonInfo("sewer_frog2",       false,false,  "The Frog Leg flexes— a frog bubbles back to life!"),
["frogtongue"]          = new SummonInfo("sewer_frog2",       false,false,  "A Frog Tongue lashes, pulling the frog into being!"),
["gargoylefiendskull"]  = new SummonInfo("dc_basement_gargoylefiend5", false,false,  "Stone gathers; a gargoyle fiend carves from its Skull!"),
["gargoyleskull"]       = new SummonInfo("dc_basement_gargoyle5",      false,false,  "Cracked Gargoyle Skull draws in rock, shaping a gargoyle!"),
["ghastskull"]          = new SummonInfo("dc_ghast3",         false,false,  "Ectoplasmic fumes seep— a ghast rises from its Skull!"),
["goblinskull"]         = new SummonInfo("ww_goblin_warrior", false,false,  "A goblin warrior’s flesh regrows over the Goblin Skull!"),
["gogsmaw"]             = new SummonInfo("sewer_gog9",        false,false,  "The Gog’s Maw snaps shut— a full gog oozes into place!"),
["goldbeetalichead"]    = new SummonInfo("shinewood_goldbeetalic",     false,true,  "A gold beetalic assembles from its severed Head!"),
["goo"]                 = new SummonInfo("ad_glupe5",         false,false,  "The formless Goo quivers, shaping into a hungry glupe!"),
["gremlinear"]          = new SummonInfo("sewer_gremlin10",   false,false,  "Mischief sparks; a gremlin reforms from the Gremlinear!"),
["gruesomeflyantenna"]  = new SummonInfo("mehadi_gruesomefly", false,false,  "Buzzing dread— a gruesomefly forms from its Antenna!"),
["gruesomeflywing"]     = new SummonInfo("mehadi_gruesomefly", false,false,  "Torn Wing beats once— a gruesomefly rebuilds mid‑air!"),
["hobgoblinskull"]      = new SummonInfo("ww_hobgoblin5",     false,false,  "A hobgoblin armors up around the Hobgoblin Skull!"),
["krakenflank"]         = new SummonInfo("sewer_kraken10",    false,false,  "Sea brine surges; a kraken regrows from its Flank!"),
["krakententacle"]      = new SummonInfo("sewer_kraken10",    false,false,  "Suckers pulse— a kraken forms from the severed Tentacle!"),
["leechtail"]           = new SummonInfo("mehadi_leech",      false,false,  "The Leech Tail elongates, regenerating a ravenous leech!"),
["mummybandage"]        = new SummonInfo("mummy10",           false,false,  "Dusty Bandages twist— a mummy shambles forth!"),
["mushroom"]            = new SummonInfo("ww_shrieker6",      false,false,  "Spore clouds burst— a shrieker uproots to life!"),
["nagetiertalon"]       = new SummonInfo("nagetier_dieter5",  false,false,  "Claws click— a Nagetier Dieter rebuilds from its Talon!"),
["polypsac"]            = new SummonInfo("ad_polyp9",         false,false,  "The Polyp Sac inflates, spawning a writhing polyp!"),
["qualgeisthead"]       = new SummonInfo("qualgeist8",        false,false,  "Spectral winds swirl— a qualgeist reforms from its Head!"),
["redtentacle"]         = new SummonInfo("karlopos_kraken",   false,false,  "Crimson water foams— a kraken rises from the Red Tentacle!"),
["rockcobblerscale"]    = new SummonInfo("sewer_rock_cobbler10", false,false,  "Pebbles fuse to the Rock Cobbler Scale, reviving a cobbler!"),
["royalwax"]            = new SummonInfo("ew_bee3",           false,false,  "Royal Wax liquefies and shapes itself into a regal bee!"),
["satyrhoof"]           = new SummonInfo("unseelie_satyr19",  false,false,  "An unseelie satyr grows from the Satyr Hoof!"),
["scarletbeetleantennae"] = new SummonInfo("scarlet_beetle4", false,false,  "Crimson sparks— a scarlet beetle rebuilds from its Antennae!"),
["sporesac"]            = new SummonInfo("ad_spore8",         false,false,  "The Spore Sac ruptures, birthing a drifting spore!"),
["trentwood"]           = new SummonInfo("wilderness_tree",   false,false,  "Splintered Trent Wood sprouts limbs— a tree demon awakens!"),
["turtleshell"]         = new SummonInfo("sewer_turtle5",     false,false,  "The empty Turtle Shell seals, and a turtle creeps out!"),
["viperegg"]            = new SummonInfo("anala_viper",       false,false,  "Scales crack— an anala viper slithers from the Egg!"),
["viperfang"]           = new SummonInfo("malini_viper",      false,false,  "A malini viper’s body grows around its lone Fang!"),
["vipervenom"]          = new SummonInfo("malini_viper",      false,false,  "Venom congeals, shaping a malini viper anew!"),
["wispcore"]            = new SummonInfo("ww_wisp4",          false,false,  "Arcane motes orbit the Wisp Core, sparking a wisp!"),
["wispflame"]           = new SummonInfo("ww_wisp4",          false,false,  "Ghost‑light flares from the Wisp Flame, forming a wisp!"),
["wolffur"]             = new SummonInfo("astrid_wolfc",      false,false,  "Stray Wolf Fur knits sinew— a wolf reappears!"),
["zombieflesh"]         = new SummonInfo("hm_zombie4",        false,false,  "Putrid Zombie Flesh reassembles, staggering into undeath!"),
["golemremains"]        = new SummonInfo("chaos_lavagolem",   false,true,  "Smoldering Golem Remains fuse, forging a lava golem!"),
["skeletonbones"]       = new SummonInfo("chaos_skeletonguard", false,true, "Loose Skeleton Bones snap together— a guard stands ready!"),
["grimsuccubushair"]    = new SummonInfo("chaos_grimsuccubus", false,true, "Cursed Succubus Hair coils— a grim succubus appears!"),
["porbossclaw"]         = new SummonInfo("chaos_porboss",     false,true,  "A Porboss Claw digs in; the full porboss hulks into view!"),
["kabungkltail"]        = new SummonInfo("chaos_kabungkl",    false,true,  "The Kabungkl Tail lashes— a kabungkl stitches together!"),
 };

    private readonly record struct SummonInfo(string MonsterKey,
        bool RequiresGrandmaster,
        bool RequiresMaster,
        string SpawnMessage);

    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        if (source is not Aisling aisling)
            return;
        
        if (!SUMMONABLES.TryGetValue(groundItem.Item.Template.TemplateKey, out var summon))
            return;

        if (summon.RequiresGrandmaster &&
            !aisling.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster))
        {
            ReturnItemWithMessage("You must be a Grand Master to do what you were about to do.");
            return;
        }
        
        if (summon.RequiresMaster &&
            !aisling.UserStatSheet.Master)
        {
            ReturnItemWithMessage("You must be a Master to do what you were about to do.");
            return;
        }

        if (aisling.MapInstance.GetEntities<Monster>()
                   .Any(m => m.Template.TemplateKey == summon.MonsterKey))
        {
            ReturnItemWithMessage($"{summon.MonsterKey} is already summoned.");
            return;
        }
        
        var point   = new Point(aisling.X - IntegerRandomizer.RollSingle(3),
            aisling.Y - IntegerRandomizer.RollSingle(3));

        var monster = MonsterFactory.Create(summon.MonsterKey, aisling.MapInstance, point);
        aisling.MapInstance.AddEntity(monster, point);
        aisling.Trackers.Counters.AddOrIncrement("dubaltar");
        aisling.SendOrangeBarMessage(summon.SpawnMessage);
        
        if (aisling.Trackers.Counters.TryGetValue("dubaltar", out var counter) && (counter == 1000))
        {
            aisling.SendOrangeBarMessage("Some things in this world take time...");
            aisling.TryGiveGamePoints(100);
            aisling.Trackers.Flags.AddFlag(AvailableCloaks.Black);
            aisling.Titles.Add("Demon Supplier");
        }

        aisling.MapInstance.RemoveEntity(groundItem);

        void ReturnItemWithMessage(string msg)
        {
            aisling.GiveItemOrSendToBank(itemFactory.Create(groundItem.Item.Template.TemplateKey));
            aisling.MapInstance.RemoveEntity(groundItem);
            aisling.SendOrangeBarMessage(msg);
        }
    }
}