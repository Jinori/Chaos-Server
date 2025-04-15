using System.Collections.Frozen;
using Chaos.Common.Utilities;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.DubhaimCastle;

public class DubhaimAltarScript(ReactorTile subject, IItemFactory itemFactory, IMonsterFactory monsterFactory)
    : ReactorTileScriptBase(subject)
{
    private readonly IMonsterFactory MonsterFactory = monsterFactory;

    private static readonly FrozenDictionary<string, SummonInfo> SUMMONABLES = new Dictionary<string, SummonInfo>
    { 
        //@formatter:off
        ["blooddiamond"]        = new("dc_kindlefiend",             true, true,  "Kindlefiend forges itself from the Blood Diamond!"),
        ["centipedegland"]      = new("crypt_centipede5",           false,false,  "A centipede forms around the Centipede Gland!"),
        ["koboldskull"]         = new("astrid_koboldc",             false,false,  "A kobold rises from the Kobold Skull's lingering magic!"),
        ["mantiseye"]           = new("ew_mantis4",                 false,false,  "From the glassy Mantis Eye, a mantis rebuilds itself!"),
        ["rawhoney"]            = new("ew_bee3",                    false,false,  "A bee forms from the enchanted Raw Honey!"),
        ["rawwax"]              = new("ew_bee3",                    false,false,  "Molten Raw Wax molds itself into wings— a bee reborn!"),
        ["vipergland"]          = new("eg_viper",                   false,false,  "A viper grows from the toxic Viper Gland!"),
        ["wolflock"]            = new("ew_wolf5",                   false,false,  "A wolf knits itself from the ragged Wolf Lock!"),
        ["wolfteeth"]           = new("ew_wolf5",                   false,false,  "Wolf Teeth fuse together, birthing a snarling wolf!"),
        ["blackshockerpiece"]   = new("nm_blackshocker",            false,true,  "A black shocker crackles to life from its Shocker Piece!"),
        ["blueshockerpiece"]    = new("nm_blueshocker",             false,true,  "A blue shocker sparks alive from the Blue Shocker Piece!"),
        ["direwolflock"]        = new("mg_direwolf",                false,true,  "Dire Wolf fur twists into muscle; a dire wolf lumbers forth!"),
        ["goldshockerpiece"]    = new("nm_goldshocker",             false,true,  "A gold shocker hums into being from the Gold Shocker Piece!"),
        ["iceelementalflame"]   = new("mg_iceelemental",            false,true,  "Frostfire swirls; an ice elemental condenses from its Flame!"),
        ["iceskeletonskull"]    = new("mg_iceskeleton",             false,true,  "An ice skeleton rises from the frozen Skeleton Skull!"),
        ["icesporesac"]         = new("mg_icespore",                false,true,  "An ice spore blooms from the Ice Spore Sac!"),
        ["losganntail"]         = new("or_losgann",                 false,true,  "The Losgann Tail thrashes— a losgann leaps alive!"),
        ["redshockerpiece"]     = new("nm_redshocker",              false,true,  "A red shocker surges from the Red Shocker Piece!"),
        ["ruidhteartoe"]        = new("or_ruidhtear",               false,true,  "Arcane mist swirls; a ruidhtear reforms around its Toe!"),
        ["batwing"]             = new("crypt_bat7",                 false,false,  "Tattered Bat Wing stitches together, summoning a bat!"),
        ["giantbatwing"]        = new("crypt_giantbat10",           false,false,  "A giant bat materializes from its shadowy Wing!"),
        ["kardifur"]            = new("crypt_kardi15",              false,false,  "The Kardi Fur writhes— a kardi prowler revives!"),
        ["marauderspine"]       = new("crypt_marauder15",           false,false,  "A marauder rises, rebuilt around its Spine!"),
        ["mimicteeth"]          = new("crypt_mimic11",              false,false,  "Mimic Teeth snap together, forming a hungry mimic!"),
        ["scorpionsting"]       = new("crypt_scorpion9",            false,false,  "Toxic barbs sprout— a scorpion forms from its Sting!"),
        ["scorpiontail"]        = new("crypt_scorpion9",            false,false,  "The Scorpion Tail coils and spawns a full scorpion!"),
        ["spidereye"]           = new("crypt_spider3",              false,false,  "Silken threads knit from the Spider Eye, shaping a spider!"),
        ["spidersilk"]          = new("crypt_spider3",              false,false,  "Living Spider Silk weaves a spider’s body anew!"),
        ["succubushair"]        = new("crypt_succubus15",           false,false,  "Succubus Hair coils, shaping a seductive succubus!"),
        ["whitebatwing"]        = new("crypt_whitebat13",           false,false,  "Pale membrane reforms; a white bat glides forth!"),
        ["giantantwing"]        = new("pf_giant_ant",               false,false,  "A giant ant rebuilds itself from the Ant Wing!"),
        ["silverwolfmanehair"]  = new("pf_silver_wolf",             false,false,  "Silver Wolf Mane weaves into a silver wolf!"),
        ["trentroot"]           = new("pf_tree_demon",              false,false,  "The Trent Root sprouts into a raging tree demon!"),
        ["ancientbone"]         = new("bone_collector",             false,false,  "Powdery Ancient Bone spins into a towering Bone Collector!"),
        ["anemoneantenna"]      = new("sewer_anemone6",             false,false,  "Tendrils unfurl; an anemone regenerates from its Antenna!"),
        ["beesting"]            = new("ew_bee3",                    false,false,  "Venom crystallizes— a bee reforms around the Bee Sting!"),
        ["blackcattail"]        = new("black_cat",                  false,false,  "Midnight fur flows; a black cat reappears tail‑first!"),
        ["blackwidowsilk"]      = new("black_widow",                false,false,  "Black Widow Silk twists into a venomous widow!"),
        ["brawlfishscale"]      = new("sewer_brawlfish6",           false,false,  "A brawlfish emerges from the Brawl Fish Scale!"),
        ["crabclaw"]            = new("sewer_crab3",                false,false,  "The Crab Claw clicks— a crab rebuilds its shell!"),
        ["crabshell"]           = new("sewer_crab3",                false,false,  "Fragments of Crab Shell snap together, reviving a crab!"),
        ["faeriewing"]          = new("ww_faerie2",                 false,false,  "Pixie dust spins a faerie from the Faerie Wing!"),
        ["flesh"]               = new("flesh_golem10",              false,false,  "Quivering Flesh stacks— a grotesque golem stands anew!"),
        ["fomorianrag"]         = new("fomorian_horror19",          false,false,  "The Fomorian Rag reknits into a hulking horror!"),
        ["frogleg"]             = new("sewer_frog2",                false,false,  "The Frog Leg flexes— a frog bubbles back to life!"),
        ["frogtongue"]          = new("sewer_frog2",                false,false,  "A Frog Tongue lashes, pulling the frog into being!"),
        ["gargoylefiendskull"]  = new("dc_basement_gargoylefiend5", false,false,  "Stone gathers; a gargoyle fiend carves from its Skull!"),
        ["gargoyleskull"]       = new("dc_basement_gargoyle5",      false,false,  "Cracked Gargoyle Skull draws in rock, shaping a gargoyle!"),
        ["ghastskull"]          = new("dc_ghast3",                  false,false,  "Ectoplasmic fumes seep— a ghast rises from its Skull!"),
        ["goblinskull"]         = new("ww_goblin_warrior",          false,false,  "A goblin warrior’s flesh regrows over the Goblin Skull!"),
        ["gogsmaw"]             = new("sewer_gog9",                 false,false,  "The Gog’s Maw snaps shut— a full gog oozes into place!"),
        ["goldbeetalichead"]    = new("shinewood_goldbeetalic",     false,true,  "A gold beetalic assembles from its severed Head!"),
        ["goo"]                 = new("ad_glupe5",                  false,false,  "The formless Goo quivers, shaping into a hungry glupe!"),
        ["gremlinear"]          = new("sewer_gremlin10",            false,false,  "Mischief sparks; a gremlin reforms from the Gremlinear!"),
        ["gruesomeflyantenna"]  = new("mehadi_gruesomefly",         false,false,  "Buzzing dread— a gruesomefly forms from its Antenna!"),
        ["gruesomeflywing"]     = new("mehadi_gruesomefly",         false,false,  "Torn Wing beats once— a gruesomefly rebuilds mid‑air!"),
        ["hobgoblinskull"]      = new("ww_hobgoblin5",              false,false,  "A hobgoblin armors up around the Hobgoblin Skull!"),
        ["krakenflank"]         = new("sewer_kraken10",             false,false,  "Sea brine surges; a kraken regrows from its Flank!"),
        ["krakententacle"]      = new("sewer_kraken10",             false,false,  "Suckers pulse— a kraken forms from the severed Tentacle!"),
        ["leechtail"]           = new("mehadi_leech",               false,false,  "The Leech Tail elongates, regenerating a ravenous leech!"),
        ["mummybandage"]        = new("mummy10",                    false,false,  "Dusty Bandages twist— a mummy shambles forth!"),
        ["mushroom"]            = new("ww_shrieker6",               false,false,  "Spore clouds burst— a shrieker uproots to life!"),
        ["nagetiertalon"]       = new("nagetier_dieter5",           false,false,  "Claws click— a Nagetier Dieter rebuilds from its Talon!"),
        ["polypsac"]            = new("ad_polyp9",                  false,false,  "The Polyp Sac inflates, spawning a writhing polyp!"),
        ["qualgeisthead"]       = new("qualgeist8",                 false,false,  "Spectral winds swirl— a qualgeist reforms from its Head!"),
        ["redtentacle"]         = new("karlopos_kraken",            false,false,  "Crimson water foams— a kraken rises from the Red Tentacle!"),
        ["rockcobblerscale"]    = new("sewer_rock_cobbler10",       false,false,  "Pebbles fuse to the Rock Cobbler Scale, reviving a cobbler!"),
        ["royalwax"]            = new("ew_bee3",                    false,false,  "Royal Wax liquefies and shapes itself into a regal bee!"),
        ["satyrhoof"]           = new("unseelie_satyr19",           false,false,  "An unseelie satyr grows from the Satyr Hoof!"),
        ["scarletbeetleantennae"] = new("scarlet_beetle4",          false,false,  "Crimson sparks— a scarlet beetle rebuilds from its Antennae!"),
        ["sporesac"]            = new("ad_spore8",                  false,false,  "The Spore Sac ruptures, birthing a drifting spore!"),
        ["trentwood"]           = new("wilderness_tree",            false,false,  "Splintered Trent Wood sprouts limbs— a tree demon awakens!"),
        ["turtleshell"]         = new("sewer_turtle5",              false,false,  "The empty Turtle Shell seals, and a turtle creeps out!"),
        ["viperegg"]            = new("anala_viper",                false,false,  "Scales crack— an anala viper slithers from the Egg!"),
        ["viperfang"]           = new("malini_viper",               false,false,  "A malini viper’s body grows around its lone Fang!"),
        ["vipervenom"]          = new("malini_viper",               false,false,  "Venom congeals, shaping a malini viper anew!"),
        ["wispcore"]            = new("ww_wisp4",                   false,false,  "Arcane motes orbit the Wisp Core, sparking a wisp!"),
        ["wispflame"]           = new("ww_wisp4",                   false,false,  "Ghost‑light flares from the Wisp Flame, forming a wisp!"),
        ["wolffur"]             = new("astrid_wolfc",               false,false,  "Stray Wolf Fur knits sinew— a wolf reappears!"),
        ["zombieflesh"]         = new("hm_zombie4",                 false,false,  "Putrid Zombie Flesh reassembles, staggering into undeath!"),
        ["golemremains"]        = new("chaos_lavagolem",            false,true,  "Smoldering Golem Remains fuse, forging a lava golem!"),
        ["skeletonbones"]       = new("chaos_skeletonguard",        false,true, "Loose Skeleton Bones snap together— a guard stands ready!"),
        ["grimsuccubushair"]    = new("chaos_grimsuccubus",         false,true, "Cursed Succubus Hair coils— a grim succubus appears!"),
        ["porbossclaw"]         = new("chaos_porboss",              false,true,  "A Porboss Claw digs in; the full porboss hulks into view!"),
        ["kabungkltail"]        = new("chaos_kabungkl",             false,true,  "The Kabungkl Tail lashes— a kabungkl stitches together!"),
        //@formatter:on
    }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    private readonly record struct SummonInfo(
        string MonsterKey,
        bool RequiresGrandmaster,
        bool RequiresMaster,
        string SpawnMessage);

    public override void OnItemDroppedOn(Creature source, GroundItem groundItem)
    {
        if (source is not Aisling aisling)
            return;

        if (!SUMMONABLES.TryGetValue(groundItem.Item.Template.TemplateKey, out var summon))
            return;

        if (summon.RequiresGrandmaster && !aisling.IsFullGrandMaster())
        {
            ReturnItemWithMessage("You must be a Grand Master to do what you were about to do.");

            return;
        }

        if (summon.RequiresMaster && !aisling.UserStatSheet.Master)
        {
            ReturnItemWithMessage("You must be a Master to do what you were about to do.");

            return;
        }

        if (aisling.MapInstance
                   .GetEntities<Monster>()
                   .Any(m => m.Template.TemplateKey.EqualsI(summon.MonsterKey)))
        {
            ReturnItemWithMessage($"{summon.MonsterKey} is already summoned.");

            return;
        }

        var point = new Point(aisling.X - IntegerRandomizer.RollSingle(3), aisling.Y - IntegerRandomizer.RollSingle(3));

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

        return;

        void ReturnItemWithMessage(string msg)
        {
            aisling.GiveItemOrSendToBank(itemFactory.Create(groundItem.Item.Template.TemplateKey));
            aisling.MapInstance.RemoveEntity(groundItem);
            aisling.SendOrangeBarMessage(msg);
        }
    }
}