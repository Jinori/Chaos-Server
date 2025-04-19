#region
using System.Text;
using Chaos.Collections;
using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Formulae;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Legend;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.DialogScripts.Religion.Abstractions;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.FunctionalScripts.ExperienceDistribution;
using Chaos.Scripting.MonsterScripts.Boss;
using Chaos.Scripting.ReactorTileScripts.Jobs;
using Chaos.Services.Factories.Abstractions;
using Chaos.Services.Servers.Options;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.AislingScripts;

public class DefaultAislingScript : AislingScriptBase, HealAbilityComponent.IHealComponentOptions
{
    private readonly IStore<BulletinBoard> BoardStore;
    private readonly IIntervalTimer CleanupSkillsSpellsTimer;
    private readonly IIntervalTimer ClearOrangeBarTimer;
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry;
    private readonly IDialogFactory DialogFactory;
    private readonly IEffectFactory EffectFactory;
    private readonly IItemFactory ItemFactory;
    private readonly ILogger<DefaultAislingScript> Logger;
    private readonly IStore<MailBox> MailStore;

    private readonly IMerchantFactory MerchantFactory;
    private readonly IIntervalTimer OneSecondTimer;
    private readonly IIntervalTimer PickupUndineEggsTimer;
    private readonly ISimpleCache SimpleCache;
    private readonly ISkillFactory SkillFactory;
    private readonly IIntervalTimer SleepAnimationTimer;
    private readonly ISpellFactory SpellFactory;

    public IApplyDamageScript MonkFormApplyDamageScript { get; init; }

    /// <inheritdoc />
    public IApplyHealScript ApplyHealScript { get; init; }

    /// <inheritdoc />
    public int? BaseHeal { get; init; }

    /// <inheritdoc />
    public Stat? HealStat { get; init; }

    /// <inheritdoc />
    public decimal? HealStatMultiplier { get; init; }

    /// <inheritdoc />
    public decimal? PctHpHeal { get; init; }

    public decimal? PctMptoHpHeal { get; init; }

    private IExperienceDistributionScript ExperienceDistributionScript { get; }

    private Animation FlameHit { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 870
    };

    private Animation LightningStanceStrike { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 698
    };

    private Animation MistHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 646
    };

    private SocialStatus PreAfkSocialStatus { get; }

    protected virtual RelationshipBehavior RelationshipBehavior { get; }
    protected virtual RestrictionBehavior RestrictionBehavior { get; }

    private Animation TideHeal { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 849
    };

    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public DefaultAislingScript(
        Aisling subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IMerchantFactory merchantFactory,
        ISimpleCache simpleCache,
        IEffectFactory effectFactory,
        IStore<MailBox> mailStore,
        IStore<BulletinBoard> boardStore,
        ILogger<DefaultAislingScript> logger,
        IItemFactory itemFactory,
        ISpellFactory spellFactory,
        ISkillFactory skillFactory,
        IDialogFactory dialogFactory)
        : base(subject)
    {
        MailStore = mailStore;
        BoardStore = boardStore;
        Logger = logger;
        ItemFactory = itemFactory;
        SpellFactory = spellFactory;
        SkillFactory = skillFactory;
        DialogFactory = dialogFactory;
        OneSecondTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        RestrictionBehavior = new RestrictionBehavior();
        VisibilityBehavior = new VisibilityBehavior();
        RelationshipBehavior = new RelationshipBehavior();
        ExperienceDistributionScript = DefaultExperienceDistributionScript.Create();
        SleepAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(5), false);
        ClearOrangeBarTimer = new IntervalTimer(TimeSpan.FromSeconds(WorldOptions.Instance.ClearOrangeBarTimerSecs), false);
        PickupUndineEggsTimer = new IntervalTimer(TimeSpan.FromMilliseconds(100), false);

        CleanupSkillsSpellsTimer = new RandomizedIntervalTimer(
            TimeSpan.FromMinutes(3),
            25,
            RandomizationType.Balanced,
            false);
        ClientRegistry = clientRegistry;
        EffectFactory = effectFactory;
        MerchantFactory = merchantFactory;
        SimpleCache = simpleCache;
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        MonkFormApplyDamageScript = ApplyNonAttackDamageScript.Create();
        var damFormula = DamageFormulae.ElementalEffect;
        MonkFormApplyDamageScript.DamageFormula = damFormula;
        ApplyHealScript.HealFormula = HealFormulae.Default;
    }

    private void AdjustAttributesBasedOnLevel(int targetAc)
    {
        var acDifference = Subject.UserStatSheet.Ac - targetAc;
        var newAtkSpeedPct = Math.Max(0, (Subject.UserStatSheet.Dex - 3) / 3);
        var atkSpeedPctDifference = Subject.UserStatSheet.AtkSpeedPct - newAtkSpeedPct;

        Subject.UserStatSheet.Subtract(
            new Attributes
            {
                Ac = acDifference,
                AtkSpeedPct = atkSpeedPctDifference
            });
    }

    private void AdjustCharacterAttributes()
    {
        switch (Subject.UserStatSheet.Level)
        {
            case < 99:
                AdjustAttributesBasedOnLevel(100 - Subject.UserStatSheet.Level / 3);

                break;
            case 99:
                
                AdjustAttributesBasedOnLevel(67);
                Subject.Client.SendAttributes(StatUpdateType.Full);

                break;
        }
    }

    /// <inheritdoc />
    public override bool CanDropItem(Item item) => RestrictionBehavior.CanDropItem(Subject, item);

    /// <inheritdoc />
    public override bool CanDropItemOn(Aisling source, Item item) => RestrictionBehavior.CanDropItemOn(source, item, Subject);

    /// <inheritdoc />
    public override bool CanDropMoney(int amount) => RestrictionBehavior.CanDropMoney(Subject, amount);

    /// <inheritdoc />
    public override bool CanDropMoneyOn(Aisling source, int amount) => RestrictionBehavior.CanDropMoneyOn(source, amount, Subject);

    /// <inheritdoc />
    public override bool CanMove() => RestrictionBehavior.CanMove(Subject);

    /// <inheritdoc />
    public override bool CanPickupItem(GroundItem groundItem) => RestrictionBehavior.CanPickupItem(Subject, groundItem);

    /// <inheritdoc />
    public override bool CanPickupMoney(Money money) => RestrictionBehavior.CanPickupMoney(Subject, money);

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity) => VisibilityBehavior.CanSee(Subject, entity);

    /// <inheritdoc />
    public override bool CanTalk() => RestrictionBehavior.CanTalk(Subject);

    /// <inheritdoc />
    public override bool CanTurn() => RestrictionBehavior.CanTurn(Subject);

    /// <inheritdoc />
    public override bool CanUseItem(Item item) => RestrictionBehavior.CanUseItem(Subject, item);

    /// <inheritdoc />
    public override bool CanUseSkill(Skill skill) => RestrictionBehavior.CanUseSkill(Subject, skill);

    /// <inheritdoc />
    public override bool CanUseSpell(Spell spell) => RestrictionBehavior.CanUseSpell(Subject, spell);

    private void CleanupSubjectItems()
    {
        RemoveNyxItemCounters();
        ProcessItemDurabilityAndScripts(Subject.Equipment);
        ProcessItemDurabilityAndScripts(Subject.Inventory);
        ProcessItemDurabilityAndScripts(Subject.Bank);
    }

    private void ClearOrangeBarMessage()
    {
        var lastOrangeBarMessage = Subject.Trackers.LastOrangeBarMessage;
        var now = DateTime.UtcNow;

        //clear if
        //an orange bar message has ever been sent
        //and the last message was sent after the last clear
        //and the time since the last message is greater than the clear timer
        var shouldClear = lastOrangeBarMessage.HasValue
                          && (lastOrangeBarMessage > (Subject.Trackers.LastOrangeBarMessageClear ?? DateTime.MinValue))
                          && (now.Subtract(lastOrangeBarMessage.Value)
                                 .TotalSeconds
                              > WorldOptions.Instance.ClearOrangeBarTimerSecs);

        if (shouldClear)
        {
            Subject.SendServerMessage(ServerMessageType.OrangeBar1, string.Empty);
            Subject.Trackers.LastOrangeBarMessage = lastOrangeBarMessage;
            Subject.Trackers.LastOrangeBarMessageClear = now;
        }
    }

    private void EnsureDurabilityWithinLimits(Item item)
    {
        if (item.CurrentDurability > item.Template.MaxDurability)
        {
            item.CurrentDurability = item.Template.MaxDurability;
            Subject.Client.SendAttributes(StatUpdateType.Full);
        }
    }

    private void EnsureValidScriptKeys(Item item)
    {
        if (item.ScriptKeys.Count > 1)
        {
            var validKey = item.ScriptKeys.FirstOrDefault(
                key => (item.Prefix != null) && key.StartsWith(item.Prefix, StringComparison.OrdinalIgnoreCase));

            if (validKey != null)
            {
                item.ScriptKeys.Clear();
                item.ScriptKeys.Add(validKey);
                Subject.Client.SendAttributes(StatUpdateType.Vitality);
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<BoardBase> GetBoardList()
    {
        //mailbox board
        yield return MailStore.Load(Subject.Name);

        if (BoardStore.Exists("announcements"))
            yield return BoardStore.Load("announcements");

        //change this to whatever naming scheme you want to follow for guild boards
        if (Subject.Guild is not null && BoardStore.Exists(Subject.Guild.Name))
            yield return BoardStore.Load(Subject.Guild.Name);

        var religion = ReligionScriptBase.CheckDeity(Subject);

        var board = religion switch
        {
            "Miraelis"  => BoardStore.Load("miraelisShrine"),
            "Skandara"  => BoardStore.Load("skandaraShrine"),
            "Serendael" => BoardStore.Load("serendaelShrine"),
            "Theselene" => BoardStore.Load("theseleneShrine"),
            null        => null,
            _           => throw new ArgumentOutOfRangeException()
        };

        if (board != null)
            yield return board;

        var nationBoard = Subject.Nation switch
        {
            Nation.Exile      => null,
            Nation.Suomi      => BoardStore.Load("nation_board_suomi"),
            Nation.Ellas      => BoardStore.Load("nation_board_ellas"),
            Nation.Loures     => BoardStore.Load("nation_board_loures"),
            Nation.Mileth     => BoardStore.Load("nation_board_mileth"),
            Nation.Tagor      => BoardStore.Load("nation_board_tagor"),
            Nation.Rucesion   => BoardStore.Load("nation_board_rucesion"),
            Nation.Noes       => BoardStore.Load("nation_board_noes"),
            Nation.Illuminati => BoardStore.Load("nation_board_illuminati"),
            Nation.Piet       => BoardStore.Load("nation_board_piet"),
            Nation.Atlantis   => BoardStore.Load("nation_board_atlantis"),
            Nation.Abel       => BoardStore.Load("nation_board_abel"),
            Nation.Undine     => BoardStore.Load("nation_board_undine"),
            Nation.Labyrinth  => BoardStore.Load("nation_board_labyrinth"),
            _                 => throw new ArgumentOutOfRangeException()
        };

        if (nationBoard != null)
            yield return nationBoard;
    }

    private void HandleAfkEffects()
    {
        if (Subject.Effects.Contains("mount"))
            Subject.Effects.Dispel("mount");

        var isGathering = Subject.MapInstance
                                 .GetDistinctReactorsAtPoint(Subject)
                                 .Any(x => x.Script.Is<ForagingSpotScript>() || x.Script.Is<FishingSpotScript>());

        Subject.Options.SocialStatus = isGathering ? SocialStatus.Gathering : SocialStatus.DayDreaming;
    }

    private void HandleGroupMaxSize()
    {
        if (Subject.Group is { Count: > 6 })
            if (!Subject.IsOnArenaMap())
            {
                foreach (var player in Subject.Group)
                    player.SendServerMessage(ServerMessageType.GroupChat, $"Group too large for non-arena map. Kicked {Subject.Name}.");

                Subject.Group.Kick(Subject);
            }
    }

    private void HandleSkillReplacements()
    {
        string[,] skillsToReplace =
        {
            //spells to remove
            {
                "athar",
                "beagathar"
            },
            {
                "morathar",
                "athar"
            },
            {
                "morathar",
                "beagathar"
            },
            {
                "ardathar",
                "morathar"
            },
            {
                "ardathar",
                "athar"
            },
            {
                "ardathar",
                "beagathar"
            },
            {
                "moratharmeall",
                "atharmeall"
            },
            {
                "ardatharmeall",
                "ardathar"
            },
            {
                "ardatharmeall",
                "morathar"
            },
            {
                "ardatharmeall",
                "beagathar"
            },
            {
                "ardatharmeall",
                "athar"
            },
            {
                "ardatharmeall",
                "moratharmeall"
            },
            {
                "ardatharmeall",
                "atharmeall"
            },
            {
                "atharlamh",
                "beagatharlamh"
            },
            {
                "moratharlamh",
                "atharlamh"
            },
            {
                "moratharlamh",
                "beagatharlamh"
            },
            {
                "creag",
                "beagcreag"
            },
            {
                "morcreag",
                "creag"
            },
            {
                "morcreag",
                "beagcreag"
            },
            {
                "ardcreag",
                "morcreag"
            },
            {
                "ardcreag",
                "creag"
            },
            {
                "ardcreag",
                "beagcreag"
            },
            {
                "morcreagmeall",
                "creagmeall"
            },
            {
                "ardcreagmeall",
                "ardcreag"
            },
            {
                "ardcreagmeall",
                "morcreag"
            },
            {
                "ardcreagmeall",
                "beagcreag"
            },
            {
                "ardcreagmeall",
                "creag"
            },
            {
                "ardcreagmeall",
                "creagmeall"
            },
            {
                "ardcreagmeall",
                "morcreagmeall"
            },
            {
                "creaglamh",
                "beagcreaglamh"
            },
            {
                "morcreaglamh",
                "creaglamh"
            },
            {
                "morcreaglamh",
                "beagcreaglamh"
            },
            {
                "sal",
                "beagsal"
            },
            {
                "morsal",
                "sal"
            },
            {
                "morsal",
                "beagsal"
            },
            {
                "ardsal",
                "morsal"
            },
            {
                "ardsal",
                "sal"
            },
            {
                "ardsal",
                "beagsal"
            },
            {
                "morsalmeall",
                "salmeall"
            },
            {
                "ardsalmeall",
                "ardsal"
            },
            {
                "ardsalmeall",
                "morsal"
            },
            {
                "ardsalmeall",
                "beagsal"
            },
            {
                "ardsalmeall",
                "sal"
            },
            {
                "ardsalmeall",
                "salmeall"
            },
            {
                "ardsalmeall",
                "morsalmeall"
            },
            {
                "sallamh",
                "beagsallamh"
            },
            {
                "morsallamh",
                "sallamh"
            },
            {
                "morsallamh",
                "beagsallamh"
            },
            {
                "srad",
                "beagsrad"
            },
            {
                "morsrad",
                "srad"
            },
            {
                "morsrad",
                "beagsrad"
            },
            {
                "ardsrad",
                "morsrad"
            },
            {
                "ardsrad",
                "srad"
            },
            {
                "ardsrad",
                "beagsrad"
            },
            {
                "morsradmeall",
                "sradmeall"
            },
            {
                "ardsradmeall",
                "ardsrad"
            },
            {
                "ardsradmeall",
                "morsrad"
            },
            {
                "ardsradmeall",
                "beagsrad"
            },
            {
                "ardsradmeall",
                "srad"
            },
            {
                "ardsradmeall",
                "sradmeall"
            },
            {
                "ardsradmeall",
                "morsradmeall"
            },
            {
                "sradlamh",
                "beagsradlamh"
            },
            {
                "morsradlamh",
                "sradlamh"
            },
            {
                "morsradlamh",
                "beagsradlamh"
            },
            {
                "arcanemissile",
                "arcanebolt"
            },
            {
                "arcaneblast",
                "arcanemissile"
            },
            {
                "arcaneblast",
                "arcanebolt"
            },
            {
                "arcaneexplosion",
                "arcaneblast"
            },
            {
                "arcaneexplosion",
                "arcanemissile"
            },
            {
                "arcaneexplosion",
                "arcanebolt"
            },
            {
                "stilettotrap",
                "needletrap"
            },
            {
                "bolttrap",
                "needletrap"
            },
            {
                "bolttrap",
                "stilettotrap"
            },
            {
                "coiledbolttrap",
                "needletrap"
            },
            {
                "coiledbolttrap",
                "stilettotrap"
            },
            {
                "coiledbolttrap",
                "bolttrap"
            },
            {
                "springtrap",
                "needletrap"
            },
            {
                "springtrap",
                "stilettotrap"
            },
            {
                "springtrap",
                "bolttrap"
            },
            {
                "springtrap",
                "coiledbolttrap"
            },
            {
                "maidentrap",
                "needletrap"
            },
            {
                "maidentrap",
                "stilettotrap"
            },
            {
                "maidentrap",
                "bolttrap"
            },
            {
                "maidentrap",
                "coiledbolttrap"
            },
            {
                "maidentrap",
                "springtrap"
            },
            {
                "pitfalltrap",
                "needletrap"
            },
            {
                "pitfalltrap",
                "stilettotrap"
            },
            {
                "pitfalltrap",
                "bolttrap"
            },
            {
                "pitfalltrap",
                "coiledbolttrap"
            },
            {
                "pitfalltrap",
                "springtrap"
            },
            {
                "pitfalltrap",
                "maidentrap"
            },
            {
                "pramh",
                "beagpramh"
            },
            {
                "revive",
                "beothaich"
            },
            {
                "resurrection",
                "beothaich"
            },
            {
                "resurrection",
                "revive"
            },
            {
                "warcry",
                "battlecry"
            },
            {
                "howl",
                "goad"
            },
            {
                "roar",
                "goad"
            },
            {
                "roar",
                "howl"
            },
            {
                "vortex",
                "quake"
            },
            {
                "rockstance",
                "earthenstance"
            },
            {
                "thunderstance",
                "lightningstance"
            },
            {
                "miststance",
                "tidestance"
            },
            {
                "smokestance",
                "earthenstance"
            },
            {
                "whirlwind",
                "wrath"
            },
            {
                "inferno",
                "wrath"
            },
            {
                "inferno",
                "whirlwind"
            },
            {
                "warcry",
                "rage"
            },
            {
                "battlecry",
                "rage"
            },
            {
                "fury",
                "berserk"
            },

            // SkillBook removals with the new methods
            {
                "energybolt",
                "assail"
            },
            {
                "blessedbolt",
                "assail"
            },
            {
                "sunder",
                "slash"
            },
            {
                "bullrush",
                "slash"
            },
            {
                "bullrush",
                "sunder"
            },
            {
                "charge",
                "bullrush"
            },
            {
                "charge",
                "sunder"
            },
            {
                "charge",
                "slash"
            },
            {
                "cleave",
                "scathe"
            },
            {
                "devour",
                "scathe"
            },
            {
                "devour",
                "cleave"
            },
            {
                "clobber",
                "strike"
            },
            {
                "clobber",
                "assail"
            },
            {
                "flank",
                "assail"
            },
            {
                "flank",
                "clobber"
            },
            {
                "flank",
                "strike"
            },
            {
                "wallop",
                "assail"
            },
            {
                "wallop",
                "flank"
            },
            {
                "wallop",
                "strike"
            },
            {
                "wallop",
                "clobber"
            },
            {
                "pulverize",
                "assail"
            },
            {
                "pulverize",
                "strike"
            },
            {
                "pulverize",
                "clobber"
            },
            {
                "pulverize",
                "flank"
            },
            {
                "pulverize",
                "wallop"
            },
            {
                "thrash",
                "assail"
            },
            {
                "thrash",
                "strike"
            },
            {
                "thrash",
                "clobber"
            },
            {
                "thrash",
                "flank"
            },
            {
                "thrash",
                "wallop"
            },
            {
                "thrash",
                "pulverize"
            },
            {
                "slaughter",
                "assail"
            },
            {
                "slaughter",
                "strike"
            },
            {
                "slaughter",
                "clobber"
            },
            {
                "slaughter",
                "flank"
            },
            {
                "slaughter",
                "wallop"
            },
            {
                "slaughter",
                "pulverize"
            },
            {
                "slaughter",
                "thrash"
            },
            {
                "tempestblade",
                "windblade"
            },
            {
                "sever",
                "windblade"
            },
            {
                "sever",
                "tempestblade"
            },
            {
                "paralyzeforce",
                "groundstomp"
            },
            {
                "groundshattering",
                "groundstomp"
            },
            {
                "madsoul",
                "flurry"
            },
            {
                "doublepunch",
                "assail"
            },
            {
                "doublepunch",
                "punch"
            },
            {
                "rapidpunch",
                "punch"
            },
            {
                "rapidpunch",
                "assail"
            },
            {
                "rapidpunch",
                "doublepunch"
            },
            {
                "triplekick",
                "assail"
            },
            {
                "triplekick",
                "doublepunch"
            },
            {
                "triplekick",
                "punch"
            },
            {
                "triplekick",
                "rapidpunch"
            },
            {
                "dragonstrike",
                "eaglestrike"
            },
            {
                "dragonstrike",
                "phoenixstrike"
            },
            {
                "phoenixstrike",
                "eaglestrike"
            },
            {
                "battlefieldsweep",
                "relentlessdraw"
            },
            {
                "roundhousekick",
                "kick"
            },
            {
                "mantiskick",
                "highkick"
            },
            {
                "smokescreen",
                "throwsmokebomb"
            },
            {
                "assault",
                "assail"
            },
            {
                "throwsurigum",
                "assault"
            },
            {
                "throwsurigum",
                "assail"
            },
            {
                "blitz",
                "assail"
            },
            {
                "blitz",
                "assault"
            },
            {
                "blitz",
                "throwsurigum"
            },
            {
                "barrage",
                "assail"
            },
            {
                "barrage",
                "assault"
            },
            {
                "barrage",
                "blitz"
            },
            {
                "barrage",
                "throwsurigum"
            },
            {
                "backstab",
                "stab"
            },
            {
                "shadowfigure",
                "backstab"
            },
            {
                "shadowfigure",
                "stab"
            },
            {
                "essencesiphon",
                "sapneedle"
            },
            {
                "skewer",
                "pierce"
            },
            {
                "midnightslash",
                "assail"
            },
            {
                "midnightslash",
                "barrage"
            },
            {
                "midnightslash",
                "throwsurigum"
            },
            {
                "midnightslash",
                "blitz"
            },
            {
                "midnightslash",
                "assault"
            }
        };

        for (var i = 0; i < skillsToReplace.GetLength(0); i++)
            RemoveAndNotifyIfBothExist(skillsToReplace[i, 0], skillsToReplace[i, 1]);
    }

    private void HandleSleepAnimation()
    {
        var lastManualAction = Subject.Trackers.LastManualAction;

        var isAfk = !lastManualAction.HasValue
                    || ((DateTime.UtcNow - lastManualAction.Value).TotalMinutes > WorldOptions.Instance.SleepAnimationTimerMins);

        if (isAfk)
        {
            if (Subject.IsAlive)
                Subject.AnimateBody(BodyAnimation.Snore);

            HandleAfkEffects();
        } else if (Subject.Options.SocialStatus is SocialStatus.DayDreaming or SocialStatus.Gathering)
            Subject.Options.SocialStatus = PreAfkSocialStatus;
    }

    private void HandleWerewolfEffect()
    {
        if (!Subject.Trackers.Enums.HasValue(WerewolfOfPiet.KilledandGotCursed)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.SpokeToWizard)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.KilledWerewolf)
            && !Subject.Trackers.Enums.HasValue(WerewolfOfPiet.CollectedBlueFlower))
            return;

        var lightlevel = Subject.MapInstance.CurrentLightLevel;

        if ((lightlevel == LightLevel.Darkest_A)
            && !Subject.Effects.Contains("Werewolf")
            && Subject.MapInstance.AutoDayNightCycle.Equals(true))
        {
            if ((Subject.MapInstance.Template.TemplateKey != "18014") && (Subject.MapInstance.Template.TemplateKey != "18015"))
                return;

            if (Subject.Effects.Contains("Mount"))
            {
                Subject.Effects.Dispel("Mount");
                Subject.SendOrangeBarMessage("You jump off your mount due to becoming a Werewolf.");

                return;
            }

            var effect = EffectFactory.Create("Werewolf");
            Subject.Effects.Apply(Subject, effect);
        } else if ((lightlevel != LightLevel.Darkest_A) && Subject.Effects.Contains("Werewolf"))
            Subject.Effects.Terminate("Werewolf");
    }

    /// <inheritdoc />
    public override bool IsFriendlyTo(Creature creature) => RelationshipBehavior.IsFriendlyTo(Subject, creature);

    /// <inheritdoc />
    public override bool IsHostileTo(Creature creature) => RelationshipBehavior.IsHostileTo(Subject, creature);

    private void ItemUpdater(Item item)
    {
        if (item.Template.TemplateKey.EqualsI("DivineStaff") && !Subject.IsPurePriestMaster())
        {
            // Remove from all possible locations
            Subject.Inventory.RemoveByTemplateKey("DivineStaff");
            Subject.Equipment.RemoveByTemplateKey("DivineStaff");
            Subject.Bank.TryWithdraw("Divine Staff", 1, out _);

            // Only give 1 replacement
            var celestialstaff = ItemFactory.Create("celestialstaff");
            Subject.GiveItemOrSendToBank(celestialstaff);
            Subject.SendOrangeBarMessage("Your Divine Staff has been replaced with a Celestial Staff.");
        }

        if (item.Template.TemplateKey.EqualsI("invisiblehelmet"))
        {
            if (Subject.Inventory.Remove("Invisible Helmet"))
            {
                Subject.TryGiveGamePoints(100);

                Logger.WithTopics(Topics.Entities.Aisling)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} received 100 gamepoints from Invisible Helmet in inventory", Subject.Name);
            }

            if (Subject.Bank.TryWithdraw("Invisible Helmet", 1, out _))
            {
                Subject.TryGiveGamePoints(100);

                Logger.WithTopics(Topics.Entities.Aisling)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} received 100 gamepoints from Invisible Helmet in bank", Subject.Name);
            }

            if (Subject.Equipment.Remove("Invisible Helmet"))
            {
                Subject.TryGiveGamePoints(100);

                Logger.WithTopics(Topics.Entities.Aisling)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} received 100 gamepoints from Invisible Helmet in equipment", Subject.Name);
            }
        }

        if (item.Template.TemplateKey.EqualsI("invisibleshield"))
        {
            if (Subject.Inventory.Remove("Invisible Shield"))
            {
                Subject.TryGiveGamePoints(100);

                Logger.WithTopics(Topics.Entities.Aisling)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} received 100 gamepoints from Invisible Shield in inventory", Subject.Name);
            }

            if (Subject.Bank.TryWithdraw("Invisible Shield", 1, out _))
            {
                Subject.TryGiveGamePoints(100);

                Logger.WithTopics(Topics.Entities.Aisling)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} received 100 gamepoints from Invisible Shield in bank", Subject.Name);
            }

            if (Subject.Equipment.Remove("Invisible Shield"))
            {
                Subject.TryGiveGamePoints(100);

                Logger.WithTopics(Topics.Entities.Aisling)
                      .WithProperty(Subject)
                      .LogInformation("Aisling {@AislingName} received 100 gamepoints from Invisible Shield in equipment", Subject.Name);
            }
        }

        if (item.Template.TemplateKey.EqualsI("smalljewelcraftingbox"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("smalljewelcraftingbox");
            var newBox = ItemFactory.Create("novicejewelcraftingbox");
            var oldBox = ItemFactory.Create("smalljewelcraftingbox");
            newBox.Count = amount;
            Subject.Inventory.RemoveQuantityByTemplateKey("smalljewelcraftingbox", amount);
            Subject.GiveItemOrSendToBank(newBox);
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Small Jewelcrafting Box"))
        {
            var amount = Subject.Bank.CountOf("Small Jewelcrafting Box");
            var newBox = ItemFactory.Create("novicejewelcraftingbox");
            var oldBox = ItemFactory.Create("smalljewelcraftingbox");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Small Jewelcrafting Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (item.Template.TemplateKey.EqualsI("largejewelcraftingbox"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("largejewelcraftingbox");
            var newBox = ItemFactory.Create("initiatejewelcraftingbox");
            var oldBox = ItemFactory.Create("largejewelcraftingbox");
            newBox.Count = amount;
            Subject.Inventory.RemoveQuantityByTemplateKey("largejewelcraftingbox", amount);
            Subject.GiveItemOrSendToBank(newBox);
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Large Jewelcrafting Box"))
        {
            var amount = Subject.Bank.CountOf("Large Jewelcrafting Box");
            var newBox = ItemFactory.Create("initiatejewelcraftingbox");
            var oldBox = ItemFactory.Create("largejewelcraftingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Large Jewelcrafting Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (item.Template.TemplateKey.EqualsI("basicalchemybox"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("basicalchemybox");
            var newBox = ItemFactory.Create("novicealchemybox");
            var oldBox = ItemFactory.Create("basicalchemybox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Inventory.RemoveQuantityByTemplateKey("basicalchemybox", amount);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Basic Alchemy Box"))
        {
            var amount = Subject.Bank.CountOf("Basic Alchemy Box");
            var newBox = ItemFactory.Create("novicealchemybox");
            var oldBox = ItemFactory.Create("basicalchemybox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Basic Alchemy Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (item.Template.TemplateKey.EqualsI("basicarmorsmithingbox"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("basicarmorsmithingbox");
            var newBox = ItemFactory.Create("novicearmorsmithingbox");
            var oldBox = ItemFactory.Create("basicarmorsmithingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            Subject.Inventory.RemoveQuantityByTemplateKey("basicarmorsmithingbox", amount);
            newBox.Count = amount;
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Basic Armorsmithing Box"))
        {
            var amount = Subject.Bank.CountOf("Basic Armorsmithing Box");
            var newBox = ItemFactory.Create("novicearmorsmithingbox");
            var oldBox = ItemFactory.Create("basicarmorsmithingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Basic Armorsmithing Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (item.Template.TemplateKey.EqualsI("largeenchantingbox"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("largeenchantingbox");
            var newBox = ItemFactory.Create("initiateenchantingbox");
            var oldBox = ItemFactory.Create("largeenchantingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Inventory.RemoveQuantityByTemplateKey("largeenchantingbox", amount);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Large Enchanting Box"))
        {
            var amount = Subject.Bank.CountOf("Large Enchanting Box");
            var newBox = ItemFactory.Create("initiateenchantingbox");
            var oldBox = ItemFactory.Create("largeenchantingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Large Enchanting Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (item.Template.TemplateKey.EqualsI("basicweaponsmithing"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("basicweaponsmithing");
            var newBox = ItemFactory.Create("noviceweaponsmithingbox");
            var oldBox = ItemFactory.Create("basicweaponsmithingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Inventory.RemoveQuantityByTemplateKey("basicweaponsmithing", amount);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Basic Weaponsmithing Box"))
        {
            var amount = Subject.Bank.CountOf("Basic Weaponsmithing Box");
            var newBox = ItemFactory.Create("noviceweaponsmithingbox");
            var oldBox = ItemFactory.Create("basicweaponsmithingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Basic Weaponsmithing Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (item.Template.TemplateKey.EqualsI("smallenchantingbox"))
        {
            var amount = Subject.Inventory.CountOfByTemplateKey("smallenchantingbox");
            var newBox = ItemFactory.Create("noviceenchantingbox");
            var oldBox = ItemFactory.Create("smallenchantingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Inventory.RemoveQuantityByTemplateKey("smallenchantingbox", amount);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }

        if (Subject.Bank.Contains("Small Enchanting Box"))
        {
            var amount = Subject.Bank.CountOf("Small Enchanting Box");
            var newBox = ItemFactory.Create("noviceenchantingbox");
            var oldBox = ItemFactory.Create("smallenchantingbox");
            Subject.SendOrangeBarMessage($"Your {oldBox.DisplayName} ({amount}) swapped for {newBox.DisplayName} ({amount}).");
            newBox.Count = amount;
            Subject.Bank.TryWithdraw("Small Enchanting Box", amount, out _);
            Subject.GiveItemOrSendToBank(newBox);

            Logger.WithTopics(Topics.Entities.Aisling)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName} received {Amount} {NewBoxDisplayName} from trade in",
                      Subject.Name,
                      amount,
                      newBox.DisplayName);
        }
    }

    private void NotifyPlayerSkills(string keyToRemove, string keyToKeep)
    {
        var nameToKeep = SkillFactory.Create(keyToKeep);
        var nameToRemove = SkillFactory.Create(keyToRemove);
        Subject.SendOrangeBarMessage("Ability " + nameToKeep.Template.Name + " removed old ability " + nameToRemove.Template.Name + ".");
    }

    private void NotifyPlayerSpells(string keyToRemove, string keyToKeep)
    {
        var nameToKeep = SpellFactory.Create(keyToKeep);
        var nameToRemove = SpellFactory.Create(keyToRemove);
        Subject.SendOrangeBarMessage("Ability " + nameToKeep.Template.Name + " removed old ability " + nameToRemove.Template.Name + ".");
    }

    public override void OnAttacked(Creature source, int damage)
    {
        if (Subject.IsPramhed())
        {
            Subject.Effects.Dispel("Pramh");
            Subject.Effects.Dispel("Beag Pramh");
        }

        if (Subject.Effects.Contains("Wolf Fang Fist"))
            Subject.Effects.Dispel("Wolf Fang Fist");

        if (Subject.IsLightningStanced())
        {
            // Apply additional effect with a 2% chance
            if (IntegerRandomizer.RollChance(2))
                if (!source.Script.Is<ThisIsABossScript>())
                {
                    var effect = EffectFactory.Create("BeagSuain");
                    source.Effects.Apply(Subject, effect);
                }

            var nonWallPoints = Subject.SpiralSearch(3)
                                       .Where(x => !Subject.MapInstance.IsWall(x))
                                       .ToList();

            if (nonWallPoints.Count <= 0)
                return;

            // Select a random non-wall point as the top-left corner of the 2x2 area
            var topLeftPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

            // Define the 2x2 area by generating points around the top-left corner
            var areaPoints = new List<Point>
            {
                topLeftPoint,
                new(topLeftPoint.X + 1, topLeftPoint.Y),
                new(topLeftPoint.X, topLeftPoint.Y + 1),
                new(topLeftPoint.X + 1, topLeftPoint.Y + 1)
            };

            // Show animation and apply damage to all entities within the 2x2 area
            foreach (var point in areaPoints)

                // Ensure the point is within the map bounds
                if ((point.X >= 0)
                    && (point.Y >= 0)
                    && (point.X < Subject.MapInstance.Template.Width)
                    && (point.Y < Subject.MapInstance.Template.Height))
                {
                    Subject.MapInstance.ShowAnimation(LightningStanceStrike.GetPointAnimation(point));

                    // Check if an entity is standing on the point and apply damage
                    var target = Subject.MapInstance
                                        .GetEntitiesAtPoints<Creature>(point)
                                        .FirstOrDefault(x => x.IsAlive && x.IsHostileTo(Subject));

                    if (target != null)
                    {
                        var airDamage = (int)(1000 + Subject.StatSheet.EffectiveCon * 50 + Subject.StatSheet.EffectiveMaximumHp * 0.05);

                        MonkFormApplyDamageScript.ApplyDamage(
                            Subject,
                            target,
                            this,
                            airDamage, Element.Wind);
                        
                        if (target is Monster monster)
                            monster.AggroList.AddOrUpdate(Subject.Id, _ => airDamage, (_, currentAggro) => currentAggro + airDamage);
                        
                        target.ShowHealth();
                    }
                }
        }

        if (Subject.IsThunderStanced())
        {

            // Apply additional effect with a 2% chance
            if (IntegerRandomizer.RollChance(2))
                if (!source.Script.Is<ThisIsABossScript>())
                {
                    var effect = EffectFactory.Create("BeagSuain");
                    source.Effects.Apply(Subject, effect);
                }

            var nonWallPoints = Subject.SpiralSearch(3)
                                       .Where(x => !Subject.MapInstance.IsWall(x))
                                       .ToList();

            if (nonWallPoints.Count <= 0)
                return;

            // Select a random non-wall point as the top-left corner of the 2x2 area
            var topLeftPoint = nonWallPoints[Random.Shared.Next(nonWallPoints.Count)];

            // Define the 2x2 area by generating points around the top-left corner
            var areaPoints = new List<Point>
            {
                topLeftPoint,
                new(topLeftPoint.X + 1, topLeftPoint.Y),
                new(topLeftPoint.X, topLeftPoint.Y + 1),
                new(topLeftPoint.X + 1, topLeftPoint.Y + 1)
            };

            // Show animation and apply damage to all entities within the 2x2 area
            foreach (var point in areaPoints)

                // Ensure the point is within the map bounds
                if ((point.X >= 0)
                    && (point.Y >= 0)
                    && (point.X < Subject.MapInstance.Template.Width)
                    && (point.Y < Subject.MapInstance.Template.Height))
                {
                    Subject.MapInstance.ShowAnimation(LightningStanceStrike.GetPointAnimation(point));

                    // Check if an entity is standing on the point and apply damage
                    var target = Subject.MapInstance
                                        .GetEntitiesAtPoints<Creature>(point)
                                        .FirstOrDefault(x => x.IsAlive && x.IsHostileTo(Subject));

                    if (target != null)
                    {
                        var airDamage = (750 + Subject.StatSheet.EffectiveCon * 30);

                        MonkFormApplyDamageScript.ApplyDamage(
                            Subject,
                            target,
                            this,
                            airDamage, Element.Wind);
                        
                        if (target is Monster monster)
                            monster.AggroList.AddOrUpdate(Subject.Id, _ => airDamage, (_, currentAggro) => currentAggro + airDamage);
                        
                        target.ShowHealth();
                    }
                }
        }

        if (Subject.IsMistStanced())
        {
            var options = new AoeShapeOptions
            {
                Source = new Point(Subject.X, Subject.Y),
                Range = 1
            };

            var points = AoeShape.AllAround.ResolvePoints(options);

            var targets = Subject.MapInstance
                                 .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                                 .WithFilter(Subject, TargetFilter.HostileOnly)
                                 .ToList();
            
            
            foreach (var target in targets)
            {
                var flameDamage = 25 + Subject.StatSheet.EffectiveCon * 10;
                
                MonkFormApplyDamageScript.ApplyDamage(
                    Subject,
                    target,
                    this,
                    flameDamage, Element.Water);

                target.ShowHealth();
                target.Animate(MistHeal, Subject.Id);
            }
        }


        if (Subject.IsTideStanced())
        {
            var options = new AoeShapeOptions
            {
                Source = new Point(Subject.X, Subject.Y),
                Range = 1
            };

            var points = AoeShape.AllAround.ResolvePoints(options);

            var targets = Subject.MapInstance
                                 .GetEntitiesAtPoints<Creature>(points.Cast<IPoint>())
                                 .WithFilter(Subject, TargetFilter.HostileOnly)
                                 .ToList();
            
            
            foreach (var target in targets)
            {
                var flameDamage = (int)(50 + Subject.StatSheet.EffectiveCon * 13 + Subject.StatSheet.EffectiveMaximumHp * 0.01);
                
                MonkFormApplyDamageScript.ApplyDamage(
                    Subject,
                    target,
                    this,
                    flameDamage, Element.Water);

                target.ShowHealth();
                target.Animate(MistHeal, Subject.Id);
            }
        }
        
        if (Subject.Effects.Contains("Mount"))
        {
            Subject.Effects.Dispel("Mount");
            Subject.Refresh();
        }

        if (Subject.IsRuminating())
        {
            Subject.Effects.Dispel("Rumination");
            Subject.SendOrangeBarMessage("Taking damage ended your rumination.");
        }
    }

    /// <inheritdoc />
    public override void OnDeath()
    {
        var source = Subject.Trackers.LastDamagedBy;

        if (source?.MapInstance.Name.Equals("Cain's Farm") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("tutorial_hut");
            var pointS = new Point(2, 9);

            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("You were knocked out. Be more careful.");
            Subject.TraverseMap(mapInstance, pointS);

            Subject.Refresh(true);

            return;
        }

        if (source?.MapInstance.Name.Equals("Mileth Inn") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn_room1");
            var pointS = new Point(3, 2);

            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("You were knocked out. Be more careful.");
            Subject.TraverseMap(mapInstance, pointS);

            Subject.Refresh(true);

            return;
        }

        if (source?.MapInstance.Name.Equals("Macabre Mansion") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("hm_road");
            var pointS = new Point(10, 11);

            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("Count removes you from his mansion. Be more careful.");
            Subject.TraverseMap(mapInstance, pointS);

            foreach (var effect in Subject.Effects)
                effect.Subject.Effects.Terminate(effect.Name);

            Subject.Refresh(true);

            return;
        }

        Subject.IsDead = true;

        //Refresh to show ghost
        Subject.Refresh(true);
        Subject.Display();

        //Remove all effects from the player
        var effects = Subject.Effects.ToList();

        foreach (var effect in effects)
        {
            if (EffectsToPersistExtension.EffectsToPersistOnDeath.Contains(effect.Name))
                continue;

            Subject.Effects.Dispel(effect.Name);
        }

        if (source?.MapInstance.Name.Equals("Mr. Hopps's Home") == true)
        {
            var terminusSpawn = new Rectangle(source, 8, 8);

            var outline = terminusSpawn.GetOutline()
                                       .ToList();
            var terminus = MerchantFactory.Create("terminus", source.MapInstance, Point.From(source));
            Point point;

            do
                point = outline.PickRandom();
            while (!source.MapInstance.IsWalkable(point, collisionType: terminus.Type));

            source.MapInstance.AddEntity(terminus, point);
        }

        if (Subject.UserStatSheet.BaseClass is BaseClass.Priest)
        {
            var monsters = Subject.MapInstance.GetEntities<Monster>();

            foreach (var monster in monsters)
                if (monster.Name.Contains(Subject.Name))
                    monster.MapInstance.RemoveEntity(monster);
        }

        if (source?.MapInstance.Name.Equals("Nightmare") == true)
        {
            var mapInstance = SimpleCache.Get<MapInstance>("mileth_inn");
            var pointS = new Point(5, 7);

            var nightmaregearDictionary = new Dictionary<(BaseClass, Gender), string[]>
            {
                {
                    (BaseClass.Warrior, Gender.Male), [
                                                          "malecarnunplate",
                                                          "carnunhelmet"
                                                      ]
                },
                {
                    (BaseClass.Warrior, Gender.Female), [
                                                            "femalecarnunplate",
                                                            "carnunhelmet"
                                                        ]
                },
                {
                    (BaseClass.Monk, Gender.Male), ["maleaosdicpatternwalker"]
                },
                {
                    (BaseClass.Monk, Gender.Female), ["femaleaosdicpatternwalker"]
                },
                {
                    (BaseClass.Rogue, Gender.Male), [
                                                        "malemarauderhide",
                                                        "maraudermask"
                                                    ]
                },
                {
                    (BaseClass.Rogue, Gender.Female), [
                                                          "femalemarauderhide",
                                                          "maraudermask"
                                                      ]
                },
                {
                    (BaseClass.Priest, Gender.Male), [
                                                         "malecthonicdisciplerobes",
                                                         "cthonicdisciplecaputium"
                                                     ]
                },
                {
                    (BaseClass.Priest, Gender.Female), [
                                                           "morrigudisciplepellison",
                                                           "holyhairband"
                                                       ]
                },
                {
                    (BaseClass.Wizard, Gender.Male), [
                                                         "cthonicmagusrobes",
                                                         "cthonicmaguscaputium"
                                                     ]
                },
                {
                    (BaseClass.Wizard, Gender.Female), [
                                                           "morrigumaguspellison",
                                                           "magushairband"
                                                       ]
                }
            };

            Subject.TraverseMap(mapInstance, pointS);
            Subject.IsDead = false;
            Subject.StatSheet.AddHp(1);
            Subject.Trackers.Enums.Set(NightmareQuestStage.CompletedNightmareLoss1);
            Subject.Client.SendAttributes(StatUpdateType.Vitality);
            Subject.SendOrangeBarMessage("You have been defeated by your Nightmares.");

            Subject.Legend.AddOrAccumulate(
                new LegendMark(
                    "Succumbed to their Nightmares",
                    "Nightmare",
                    MarkIcon.Victory,
                    MarkColor.White,
                    1,
                    GameTime.Now));

            var gearKey = (Subject.UserStatSheet.BaseClass, Subject.Gender);

            if (nightmaregearDictionary.TryGetValue(gearKey, out var nightmaregear))
            {
                var hasGear = nightmaregear.All(
                    gearItemName => Subject.Inventory.ContainsByTemplateKey(gearItemName)
                                    || Subject.Bank.Contains(gearItemName)
                                    || Subject.Equipment.ContainsByTemplateKey(gearItemName));

                if (!hasGear)
                    foreach (var gearItemName in nightmaregear)
                    {
                        var gearItem = ItemFactory.Create(gearItemName);
                        Subject.GiveItemOrSendToBank(gearItem);
                    }
            }

            Subject.Refresh(true);

            return;
        }

        if (Subject.IsOnArenaMap())
        {
            var aislings = Subject.MapInstance.GetEntities<Aisling>();

            foreach (var aisling in aislings)
                aisling.SendServerMessage(ServerMessageType.OrangeBar1, $"{Subject.Name} was killed by {source?.Name}.");
        }

        if (MapsToNotPunishDeathExtension.MapList.Contains(Subject.MapInstance.Name))
            return;

        if (Subject.MapInstance is { IsShard: true, LoadedFromInstanceId: "guildhallmain" })
            return;

        foreach (var client in ClientRegistry)
            client.SendServerMessage(
                ServerMessageType.OrangeBar1,
                $"{Subject.Name} was killed at {Subject.MapInstance.Name} by {source?.Name ?? "The Guardians"}.");

        var itemsToBreak = Subject.Equipment
                                  .Where(
                                      x => !x.Template.AccountBound
                                           && (x.Template.EquipmentType != EquipmentType.Accessory)
                                           && (x.Template.EquipmentType != EquipmentType.OverArmor)
                                           && (x.Template.EquipmentType != EquipmentType.OverHelmet))
                                  .ToList();

        var savedItems = new List<string>();
        var lostItems = new List<string>();

        foreach (var item in itemsToBreak)
        {
            if (!IntegerRandomizer.RollChance(2))
                continue;

            var diceCount = Subject.Inventory.CountOfByTemplateKey("mithrildice");

            if (diceCount > 0)
            {
                // Use one Mithril Dice
                Subject.Inventory.RemoveQuantityByTemplateKey("mithrildice", 1);
                savedItems.Add(item.DisplayName);

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Actions.Death,
                          Topics.Actions.Penalty)
                      .WithProperty(Subject)
                      .WithProperty(item)
                      .LogInformation(
                          "{@AislingName}'s {@ItemName} was saved by Mithril Dice (Remaining Dice: {@DiceCount})",
                          Subject.Name,
                          item.DisplayName,
                          diceCount - 1);
            } else
            {
                // Remove the item from the player's equipment
                Subject.Equipment.TryGetRemove(item.Slot, out _);
                lostItems.Add(item.DisplayName);

                Logger.WithTopics(
                          Topics.Entities.Aisling,
                          Topics.Entities.Item,
                          Topics.Actions.Death,
                          Topics.Actions.Penalty)
                      .WithProperty(Subject)
                      .WithProperty(item)
                      .LogInformation("{@AislingName} has lost {@ItemName} to death. (No dice remaining)", Subject.Name, item.DisplayName);
            }
        }

        var sb = new StringBuilder();

        if (savedItems.Any() && lostItems.Any())
        {
            sb.AppendLineFColored(MessageColor.Yellow, "You lost the following items:");

            foreach (var item in lostItems)
                sb.AppendLineFColored(MessageColor.Gainsboro, $"{item}");

            sb.AppendLine("");

            sb.AppendLineFColored(MessageColor.NeonGreen, "Mithril Dice saved the following items:");

            foreach (var item in savedItems)
                sb.AppendLineFColored(MessageColor.Gainsboro, $"{item}");

            sb.AppendLine("");
            sb.AppendLineFColored(MessageColor.Orange, "Revive with Terminus or wait to be revived.");
        } else if (savedItems.Any())
        {
            sb.AppendLineFColored(MessageColor.NeonGreen, "Mithril Dice saved your items:");

            foreach (var item in savedItems)
                sb.AppendLineFColored(MessageColor.Gainsboro, $"{item}");

            sb.AppendLine("");
            sb.AppendLineFColored(MessageColor.Orange, "Revive with Terminus or wait to be revived.");
        } else if (lostItems.Any())
        {
            sb.AppendLineFColored(MessageColor.Yellow, "You lost the following items:");

            foreach (var item in lostItems)
            {
                sb.AppendLineFColored(MessageColor.Gainsboro, $"{item}");
                Subject.SendActiveMessage($"{item} broke.");
            }

            sb.AppendLine("");
            sb.AppendLineFColored(MessageColor.Orange, "Revive with Terminus or wait to be revived.");
        }

        if (sb.Length > 0)
            Subject.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());

        var tenPercent = MathEx.GetPercentOf<int>((int)Subject.UserStatSheet.TotalExp, 10);

        if (ExperienceDistributionScript.TryTakeExp(Subject, tenPercent))
        {
            Logger.WithTopics(
                      Topics.Entities.Aisling,
                      Topics.Actions.Death,
                      Topics.Actions.Penalty,
                      Topics.Entities.Experience)
                  .WithProperty(Subject)
                  .LogInformation("{@AislingName} has lost {@ExperienceAmount} experience to death", Subject.Name, tenPercent);

            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"You have lost {tenPercent} experience.");
        }

        Subject.Trackers.Counters.AddOrIncrement("deathcounter", 1);

        Subject.Legend.AddOrAccumulate(
            new LegendMark(
                "Fell in battle",
                "deathkey",
                MarkIcon.Victory,
                MarkColor.White,
                1,
                GameTime.Now));

        if (Subject.UserStatSheet.Level <= 41)
            Subject.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You have died. Press F1 to revive.");
    }

    public override void OnLogin()
    {
        if (Subject.Trackers.LastLogout is { } lastLogout && (lastLogout.AddHours(2) <= DateTime.UtcNow))
        {
            var merch = MerchantFactory.Create("terminus", Subject.MapInstance, Subject);
            var dialog = DialogFactory.Create("terminus_homeoptions", merch);
            dialog.Display(Subject);
        }

        if (Subject.Guild != null)
            foreach (var player in ClientRegistry)
                if (player.Aisling.Guild == Subject.Guild)
                    player.Aisling.SendServerMessage(
                        ServerMessageType.ActiveMessage,
                        $"{Subject.Guild.Name} member {Subject.Name} has appeared online.");
    }

    /// <inheritdoc />
    public override void OnLogout()
    {
        base.OnLogout();

        if (Subject.Effects.Contains("Mount"))
            Subject.Effects.Terminate("Mount");
        
        var guild = Subject.Guild;

        if (guild != null)
            foreach (var member in guild.GetOnlineMembers())
                member.SendServerMessage(ServerMessageType.ActiveMessage, $"({guild.Name}) - {Subject.Name} has gone offline.");
    }

    /// <inheritdoc />
    public override void OnStatIncrease(Stat stat)
    {
        switch (stat)
        {
            case Stat.STR:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumHp = 25
                    });
                Subject.UserStatSheet.SetMaxWeight(LevelUpFormulae.Default.CalculateMaxWeight(Subject));

                Subject.SendServerMessage(
                    ServerMessageType.ActiveMessage,
                    "STR increased by one and maximum health increased by twenty five.");

                break;
            case Stat.DEX:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        AtkSpeedPct = (Subject.StatSheet.Dex % 3) == 0 ? 1 : 0
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "DEX increased by one and Attack Speed increased.");

                break;
            case Stat.INT:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumMp = 20
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "INT increased by one and maximum mana increased by twenty.");

                break;
            case Stat.WIS:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumMp = 40
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "WIS increased by one and maximum mana increased by fourty.");

                break;
            case Stat.CON:
                Subject.UserStatSheet.Add(
                    new Attributes
                    {
                        MaximumHp = 50
                    });
                Subject.SendServerMessage(ServerMessageType.ActiveMessage, "CON increased by one and maximum health increased by fifty.");

                break;
        }
    }

    private void PickupUndineEggs()
    {
        var egg = Subject.MapInstance
                         .GetEntitiesAtPoints<GroundItem>(Subject)
                         .FirstOrDefault(x => x.Name is "Undine Chicken Egg" or "Undine Golden Chicken Egg");

        switch (egg?.Item.DisplayName)
        {
            case "Undine Chicken Egg":
            {
                var item = ItemFactory.Create("undinechickenegg");

                if (!Subject.Inventory.TryAddToNextSlot(item))
                {
                    Subject.SendOrangeBarMessage("You need space to pickup eggs!");

                    return;
                }

                Subject.MapInstance.RemoveEntity(egg);
                Subject.SendPersistentMessage($"{Subject.Inventory.CountOf("Undine Chicken Egg")} eggs!");

                break;
            }
            case "Undine Golden Chicken Egg":
            {
                var item = ItemFactory.Create("undinegoldenchickenegg");

                if (!Subject.Inventory.TryAddToNextSlot(item))
                {
                    Subject.SendOrangeBarMessage("You need space to pickup Golden eggs!");

                    return;
                }

                Subject.MapInstance.RemoveEntity(egg);
                Subject.SendPersistentMessage($"{Subject.Inventory.CountOf("Undine Golden Chicken Egg")} golden eggs!");
                Subject.Client.SendSound(177, false);

                foreach (var bunny in Subject.MapInstance.GetEntities<Monster>())
                    bunny.Trackers.Counters.AddOrIncrement("Frightened");

                break;
            }
        }
    }

    private void ProcessItemDurabilityAndScripts(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            ItemUpdater(item);
            EnsureDurabilityWithinLimits(item);
            EnsureValidScriptKeys(item);
        }
    }

    private void RemoveAndNotifyIfBothExist(string keyToKeep, string keyToRemove)
    {
        if (Subject.IsGodModeEnabled())
            return;

        if (Subject.SpellBook.ContainsByTemplateKey(keyToKeep) && Subject.SpellBook.ContainsByTemplateKey(keyToRemove))
        {
            Subject.SpellBook.RemoveByTemplateKey(keyToRemove);
            NotifyPlayerSpells(keyToRemove, keyToKeep);

            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Skill, Topics.Actions.Update)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName}'s ability {KeyToKeep} removed an old ability {@KeyToRemove}",
                      Subject.Name,
                      keyToKeep,
                      keyToRemove);
        } else if (Subject.SkillBook.ContainsByTemplateKey(keyToKeep) && Subject.SkillBook.ContainsByTemplateKey(keyToRemove))
        {
            Subject.SkillBook.RemoveByTemplateKey(keyToRemove);
            NotifyPlayerSkills(keyToRemove, keyToKeep);

            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Skill, Topics.Actions.Update)
                  .WithProperty(Subject)
                  .LogInformation(
                      "Aisling {@AislingName}'s ability {KeyToKeep} removed an old ability {@KeyToRemove}",
                      Subject.Name,
                      keyToKeep,
                      keyToRemove);
        }
    }

    private void RemoveInvalidSpellsAndSkills()
    {
        string[] pureOnlySpells =
        {
            "magmasurge",
            "tidalbreeze",
            "sightoffrailty",
            "diacradh",
            "hidegroup",
            "healingaura",
            "darkstorm",
            "resurrect",
            "evasion"
        };

        string[] pureOnlySkills =
        {
            "annihilate",
            "dragonstrike",
            "chaosfist",
            "madsoul",
            "onslaught",
            "sneakattack",
            "shadowfigure",
            "rupture",
            "battlefieldsweep",
            "paralyzeforce",
            "shadowfigure"
        };

        foreach (var spell in pureOnlySpells)
            RemovePureOnlySpells(spell);

        foreach (var skill in pureOnlySkills)
            RemovePureOnlySkills(skill);

        RemoveOldMonkFormSkillsSpells(Subject);
        RemoveOldWarriorSkillsSpells(Subject);
    }

    private void RemoveItemIfTitleMissing(string title1, string title2, string itemKey, string legendKey)
    {
        if (!Subject.Titles.ContainsI(title1) && !Subject.Titles.ContainsI(title2) && !Subject.IsAdmin)
        {
            Subject.Legend.Remove(legendKey, out _);
            Subject.Inventory.RemoveQuantityByTemplateKey(itemKey, 1);
        }
    }

    private void RemoveNyxItemCounters()
    {
        foreach (var counter in Subject.Trackers.Counters.ToList())
            if (counter.Key.ContainsI("NyxItem"))
                Subject.Trackers.Counters.Remove(counter.Key, out _);
    }

    private static void RemoveOldMonkFormSkillsSpells(Aisling aisling)
    {
        if (aisling.UserStatSheet.BaseClass is not BaseClass.Monk)
            return;

        if (!aisling.Trackers.Enums.TryGetValue(out MonkElementForm currentForm))
            return;

        var elementSkillsAndSpells = new Dictionary<MonkElementForm, (List<string> Skills, List<string> Spells)>
        {
            {
                MonkElementForm.Water, ([
                                            "waterpunch",
                                            "tsunamikick",
                                            "hydrosiphon"
                                        ], [
                                               "miststance",
                                               "tidestance"
                                           ])
            },
            {
                MonkElementForm.Earth, ([
                                            "earthpunch",
                                            "seismickick",
                                            "seismicslam"
                                        ], [
                                               "earthenstance",
                                               "rockstance"
                                           ])
            },
            {
                MonkElementForm.Air, ([
                                          "airpunch",
                                          "tempestkick",
                                          "cyclonetwist"
                                      ], [
                                             "thunderstance",
                                             "lightningstance"
                                         ])
            },
            {
                MonkElementForm.Fire, ([
                                           "firepunch",
                                           "dracotailkick",
                                           "moltenstrike"
                                       ], [
                                              "smokestance",
                                              "flamestance"
                                          ])
            }
        };

        foreach (var element in elementSkillsAndSpells.Where(element => element.Key != currentForm))
            RemoveSkillsAndSpells(aisling, element.Value.Skills, element.Value.Spells);
    }

    private static void RemoveOldWarriorSkillsSpells(Aisling aisling)
    {
        if (aisling.SpellBook.ContainsByTemplateKey("rage"))
            aisling.SpellBook.RemoveByTemplateKey("rage");
    }

    private void RemovePureOnlySkills(string keyToRemove)
    {
        if (Subject.IsGodModeEnabled())
            return;

        if (Subject.SkillBook.ContainsByTemplateKey(keyToRemove) && Subject.Legend.ContainsKey("dedicated"))
        {
            Subject.SkillBook.RemoveByTemplateKey(keyToRemove);
            var skill = SkillFactory.Create(keyToRemove);
            Subject.SendOrangeBarMessage($"{skill.Template.Name} has been removed.");

            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Skill, Topics.Actions.Update)
                  .WithProperty(Subject)
                  .LogInformation("Aisling {@AislingName}'s pure only skill {KeyToRemove} removed", Subject.Name, keyToRemove);
        }
    }

    private void RemovePureOnlySpells(string keyToRemove)
    {
        if (Subject.IsGodModeEnabled())
            return;

        if (Subject.SpellBook.ContainsByTemplateKey(keyToRemove) && Subject.Legend.ContainsKey("dedicated"))
        {
            Subject.SpellBook.RemoveByTemplateKey(keyToRemove);
            var spell = SpellFactory.Create(keyToRemove);
            Subject.SendOrangeBarMessage($"{spell.Template.Name} has been removed.");

            Logger.WithTopics(Topics.Entities.Creature, Topics.Entities.Skill, Topics.Actions.Update)
                  .WithProperty(Subject)
                  .LogInformation("Aisling {@AislingName}'s pure only spell {KeyToRemove} removed", Subject.Name, keyToRemove);
        }
    }

    private void RemoveRestrictedTrinkets()
    {
        RemoveItemIfTitleMissing("Expert Enchanter", "Master Enchanter", "portaltrinket", "enchantingtrinket");
        RemoveItemIfTitleMissing("Expert Weaponsmith", "Master Weaponsmith", "dmgtrinket", "weaponsmithtrinket");
        RemoveItemIfTitleMissing("Expert Armorsmith", "Master Armorsmith", "repairtrinket", "armorsmithtrinket");
        RemoveItemIfTitleMissing("Expert Jewelcrafter", "Master Jewelcrafter", "exptrinket", "jewelcrafttrinket");
    }

    private static void RemoveSkillsAndSpells(Aisling aisling, List<string> skills, List<string> spells)
    {
        foreach (var skill in skills.Where(skill => aisling.SkillBook.ContainsByTemplateKey(skill)))
            aisling.SkillBook.RemoveByTemplateKey(skill);

        foreach (var spell in spells.Where(spell => aisling.SpellBook.ContainsByTemplateKey(spell)))
            aisling.SpellBook.RemoveByTemplateKey(spell);
    }

    private void ReplaceMultipleSkills(string oldSkill, string[] newSkills)
    {
        if (Subject.SkillBook.ContainsByTemplateKey(oldSkill))
        {
            Subject.SkillBook.RemoveByTemplateKey(oldSkill);

            foreach (var newSkill in newSkills)
                if (!Subject.SkillBook.ContainsByTemplateKey(newSkill))
                {
                    Subject.SkillBook.TryAddToNextSlot(SkillFactory.Create(newSkill));
                    Subject.SendOrangeBarMessage($"{oldSkill} has been replaced by {newSkill}.");
                }
        }
    }

    private void ReplaceSkill(string oldSkill, string newSkill)
    {
        if (Subject.SkillBook.ContainsByTemplateKey(oldSkill))
        {
            Subject.SkillBook.RemoveByTemplateKey(oldSkill);

            if (!Subject.SkillBook.ContainsByTemplateKey(newSkill))
            {
                Subject.SkillBook.TryAddToNextSlot(SkillFactory.Create(newSkill));
                Subject.SendOrangeBarMessage($"{oldSkill} has been replaced by {newSkill}.");
            }
        }
    }

    public override void Update(TimeSpan delta)
    {
        SleepAnimationTimer.Update(delta);
        ClearOrangeBarTimer.Update(delta);
        OneSecondTimer.Update(delta);
        CleanupSkillsSpellsTimer.Update(delta);

        PickupUndineEggsTimer.Update(delta);

        if (PickupUndineEggsTimer.IntervalElapsed)
            PickupUndineEggs();

        if (CleanupSkillsSpellsTimer.IntervalElapsed && !Subject.IsDiacht())
        {
            CleanupSubjectItems();
            UpdateSkillBook();
            AdjustCharacterAttributes();
            RemoveRestrictedTrinkets();
            RemoveInvalidSpellsAndSkills();
            HandleSkillReplacements();
        }

        if (OneSecondTimer.IntervalElapsed)
        {
            HandleGroupMaxSize();
            HandleWerewolfEffect();
        }

        if (SleepAnimationTimer.IntervalElapsed)
            HandleSleepAnimation();

        if (ClearOrangeBarTimer.IntervalElapsed)
            ClearOrangeBarMessage();
    }

    private void UpdateSkillBook()
    {
        if (Subject.Legend.ContainsKey("dedicated") && Subject.Legend.ContainsKey("monkClass"))
            if (!Subject.SpellBook.ContainsByTemplateKey("catsHearing"))
            {
                var catsHearing = SpellFactory.Create("catsHearing");
                Subject.SpellBook.TryAddToNextSlot(catsHearing);
            }

        if (Subject.HasClass(BaseClass.Rogue))
        {
            if (!Subject.Legend.ContainsKey("monkClass"))
                Subject.SpellBook.RemoveByTemplateKey("catsHearing");
                    
            if (!Subject.SpellBook.ContainsByTemplateKey("eisdcreutair"))
            {
                var eisd = SpellFactory.Create("eisdcreutair");
                Subject.SpellBook.TryAddToNextSlot(eisd);
            }
        }

        if (Subject.Legend.ContainsKey("dedicated") && Subject.Legend.ContainsKey("rogueClass"))
        {
            if (!Subject.SpellBook.ContainsByTemplateKey("eisdcreutair"))
            {
                if (!Subject.HasClass(BaseClass.Monk))
                    Subject.SpellBook.RemoveByTemplateKey("catsHearing");

                var eisd = SpellFactory.Create("eisdcreutair");
                Subject.SpellBook.TryAddToNextSlot(eisd);
            }
        }

        ReplaceSkill("multistrike", "rupture");
        ReplaceSkill("gut", "backstab");

        ReplaceMultipleSkills(
            "surigumblitz",
            [
                "murderousintent",
                "killerinstinct"
            ]);
    }
}