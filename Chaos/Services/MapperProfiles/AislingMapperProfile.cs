#region
using Chaos.Collections;
using Chaos.Collections.Synchronized;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class AislingMapperProfile(
    ISimpleCache simpleCache,
    ITypeMapper mapper,
    IFactory<Exchange> exchangeFactory,
    ILoggerFactory loggerFactory,
    ICloningService<Item> itemCloner,
    IScriptProvider scriptProvider,
    IStore<Guild> guildStore) : IMapperProfile<Aisling, AislingSchema>,
                                IMapperProfile<Aisling, AttributesArgs>,
                                IMapperProfile<Aisling, DisplayAislingArgs>,
                                IMapperProfile<Aisling, OtherProfileArgs>,
                                IMapperProfile<Aisling, SelfProfileArgs>,
                                IMapperProfile<Aisling, UserIdArgs>,
                                IMapperProfile<Aisling, WorldListMemberInfo>
{
    private readonly IFactory<Exchange> ExchangeFactory = exchangeFactory;
    private readonly IStore<Guild> GuildStore = guildStore;
    private readonly ICloningService<Item> ItemCloner = itemCloner;
    private readonly ILoggerFactory LoggerFactory = loggerFactory;
    private readonly ITypeMapper Mapper = mapper;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Aisling Map(AislingSchema obj)
    {
        MapInstance mapInstance;
        IPoint point = new Point(obj.X, obj.Y);

        try
        {
            mapInstance = SimpleCache.Get<MapInstance>(obj.MapInstanceId);
        } catch (Exception e)
        {
            if (obj.FallbackLocation is not null)
                try
                {
                    mapInstance = SimpleCache.Get<MapInstance>(obj.FallbackLocation.Map);
                    point = obj.FallbackLocation;
                } catch (Exception ex)
                {
                    throw new KeyNotFoundException(
                        $"Unable to find {nameof(obj.MapInstanceId)} of \"{obj.MapInstanceId}\" or {nameof(obj.FallbackLocation)
                        } of \"{obj.FallbackLocation.Map}\"",
                        ex);
                }
            else
                throw new KeyNotFoundException(
                    $"Unable to find {nameof(obj.MapInstanceId)} of \"{obj.MapInstanceId}\", and no {nameof(obj.FallbackLocation)
                    } was specified",
                    e);
        }

        var aisling = new Aisling(
            obj.Name,
            mapInstance,
            point,
            ExchangeFactory,
            ScriptProvider,
            LoggerFactory.CreateLogger<Aisling>(),
            ItemCloner)
        {
            BodyColor = obj.BodyColor,
            BodySprite = obj.BodySprite,
            Direction = obj.Direction,
            FaceSprite = obj.FaceSprite,
            GamePoints = obj.GamePoints,
            Gender = obj.Gender,
            Gold = obj.Gold,
            HairColor = obj.HairColor,
            HairStyle = obj.HairStyle,
            Nation = obj.Nation,
            IgnoreList = new IgnoreList(obj.IgnoreList),
            Titles = new TitleList(obj.Titles),
            Options = Mapper.Map<UserOptions>(obj.UserOptions),
            UserStatSheet = Mapper.Map<UserStatSheet>(obj.StatSheet),
            IsAdmin = obj.IsAdmin,
            IsKnight = obj.IsKnight,
            IsDead = obj.IsDead,
            ChannelSettings = new SynchronizedHashSet<ChannelSettings>(Mapper.MapMany<ChannelSettings>(obj.ChannelSettings))
        };

        //lookup guild and attach
        if (!string.IsNullOrEmpty(obj.GuildName) && GuildStore.Exists(obj.GuildName))
        {
            var guild = GuildStore.Load(obj.GuildName);
            aisling.Guild = guild;
        }

        return aisling;
    }

    AislingSchema IMapperProfile<Aisling, AislingSchema>.Map(Aisling obj)
    {
        var ret = new AislingSchema
        {
            MapInstanceId = obj.MapInstance.InstanceId,
            FallbackLocation = obj.MapInstance.ShardingOptions?.ExitLocation,
            BodyColor = obj.BodyColor,
            BodySprite = obj.BodySprite,
            Direction = obj.Direction,
            FaceSprite = obj.FaceSprite,
            GamePoints = obj.GamePoints,
            Gender = obj.Gender,
            Gold = obj.Gold,
            GuildName = obj.Guild?.Name,
            HairColor = obj.HairColor,
            HairStyle = obj.HairStyle,
            Name = obj.Name,
            Nation = obj.Nation,
            X = obj.X,
            Y = obj.Y,
            IsAdmin = obj.IsAdmin,
            IsDead = obj.IsDead,
            StatSheet = Mapper.Map<UserStatSheetSchema>(obj.StatSheet),
            Titles = obj.Titles.ToList(),
            UserOptions = Mapper.Map<UserOptionsSchema>(obj.Options),
            IgnoreList = obj.IgnoreList.ToList(),
            ChannelSettings = Mapper.MapMany<ChannelSettingsSchema>(obj.ChannelSettings)
                                    .ToList()
        };

        return ret;
    }

    public Aisling Map(AttributesArgs obj) => throw new NotImplementedException();

    AttributesArgs IMapperProfile<Aisling, AttributesArgs>.Map(Aisling obj)
        => new()
        {
            Ability = (byte)obj.UserStatSheet.AbilityLevel,
            Ac = obj.UserStatSheet.EffectiveAc,
            Blind = obj.IsDall,
            Con = (byte)Math.Clamp(obj.UserStatSheet.EffectiveCon, byte.MinValue, byte.MaxValue),
            CurrentHp = (uint)Math.Clamp(obj.UserStatSheet.CurrentHp, 0, int.MaxValue),
            CurrentMp = (uint)Math.Clamp(obj.UserStatSheet.CurrentMp, 0, int.MaxValue),
            CurrentWeight = (short)obj.UserStatSheet.CurrentWeight,
            DefenseElement = obj.UserStatSheet.DefenseElement,
            Dex = (byte)Math.Clamp(obj.UserStatSheet.EffectiveDex, byte.MinValue, byte.MaxValue),
            Dmg = (byte)Math.Clamp(obj.UserStatSheet.EffectiveDmg, byte.MinValue, byte.MaxValue),
            GamePoints = (uint)obj.GamePoints,
            Gold = (uint)obj.Gold,
            Hit = (byte)Math.Clamp(obj.UserStatSheet.EffectiveHit, byte.MinValue, byte.MaxValue),
            Int = (byte)Math.Clamp(obj.UserStatSheet.EffectiveInt, byte.MinValue, byte.MaxValue),
            IsAdmin = obj.IsAdmin,
            IsSwimming = obj.MapInstance.Template.Tiles[obj.X, obj.Y].IsWater,
            Level = (byte)obj.UserStatSheet.Level,
            HasUnreadMail = obj.MailBox.Any(post => post.IsHighlighted),
            MagicResistance = (byte)Math.Clamp(obj.UserStatSheet.EffectiveMagicResistance / 10.0, byte.MinValue, byte.MaxValue),
            MaximumHp = obj.UserStatSheet.EffectiveMaximumHp,
            MaximumMp = obj.UserStatSheet.EffectiveMaximumMp,
            MaxWeight = (short)obj.UserStatSheet.MaxWeight,
            OffenseElement = obj.UserStatSheet.OffenseElement,
            Str = (byte)Math.Clamp(obj.UserStatSheet.EffectiveStr, byte.MinValue, byte.MaxValue),
            ToNextAbility = obj.UserStatSheet.ToNextAbility,
            ToNextLevel = obj.UserStatSheet.ToNextLevel,
            TotalAbility = obj.UserStatSheet.TotalAbility,
            TotalExp = obj.UserStatSheet.TotalExp,
            UnspentPoints = (byte)obj.UserStatSheet.UnspentPoints,
            Wis = (byte)Math.Clamp(obj.UserStatSheet.EffectiveWis, byte.MinValue, byte.MaxValue)
        };

    public Aisling Map(DisplayAislingArgs obj) => throw new NotImplementedException();

    DisplayAislingArgs IMapperProfile<Aisling, DisplayAislingArgs>.Map(Aisling obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj), "Aisling object cannot be null.");

        var hasArenaTeam = obj.Trackers.Enums.TryGetValue(out ArenaTeam arenaTeam);
        var gearFlags = obj.Trackers.Flags.TryGetFlag(out InvisibleGear invisibleGear);

        var weapon = obj.Equipment[EquipmentSlot.Weapon];
        var armor = obj.Equipment[EquipmentSlot.Armor];
        var shield = obj.Equipment[EquipmentSlot.Shield];
        var overHelm = obj.Equipment[EquipmentSlot.OverHelm];
        var helmet = obj.Equipment[EquipmentSlot.Helmet];
        var boots = obj.Equipment[EquipmentSlot.Boots];
        var acc1 = obj.Equipment[EquipmentSlot.Accessory1];
        var acc2 = obj.Equipment[EquipmentSlot.Accessory2];
        var acc3 = obj.Equipment[EquipmentSlot.Accessory3];
        var overcoat = obj.Equipment[EquipmentSlot.Overcoat];
        var pantsColor = overcoat?.Template.PantsColor ?? armor?.Template.PantsColor;

        ushort headSprite = 0;
        ushort shieldSprite = 0;
        byte bootsSprite = 0;
        DisplayColor headColor;
        DisplayColor bootsColor;

        var shouldOverrideHeadSprite = obj.Equipment?.Any(item => item?.Template?.OverridesHeadSprite == true) ?? false;
        var shouldOverrideBootsSprite = obj.Equipment?.Any(item => item?.Template?.OverridesBootsSprite == true) ?? false;

        // Apply invisible gear flags only if the player is NOT in an arena team
        var hideHelmet = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideHelmet);
        var hideWeapon = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideWeapon);
        var hideShield = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideShield);
        var hideBoots = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideBoots);
        var hideArmor = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideArmor);
        var hideAcc1 = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideAccessoryOne);
        var hideAcc2 = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideAccessoryTwo);
        var hideAcc3 = !hasArenaTeam && invisibleGear.HasFlag(InvisibleGear.HideAccessoryThree);

        // Determine head sprite
        if (hideHelmet)
            headSprite = (ushort)obj.HairStyle;
        else if (shouldOverrideHeadSprite)
            headSprite = 0;
        else if (overHelm?.ItemSprite?.DisplaySprite is not null)
            headSprite = overHelm.ItemSprite.DisplaySprite;
        else if (helmet?.ItemSprite?.DisplaySprite is not null)
            headSprite = helmet.ItemSprite.DisplaySprite;
        else
            headSprite = (ushort)obj.HairStyle;

        // Determine shield sprite
        if (hideShield)
            shieldSprite = 0;
        else
            shieldSprite = shield?.ItemSprite?.DisplaySprite ?? 0;

        // Determine head color
        if (headSprite != 0)
        {
            if ((overHelm?.Template != null) && (overHelm.Template.IsDyeable || (overHelm.Color != DisplayColor.Default)))
                headColor = overHelm.Color;
            else if ((helmet != null) && (helmet.Template.IsDyeable || (helmet.Color != DisplayColor.Default)))
                headColor = helmet.Color;
            else if ((helmet != null) && (helmet.Template.IsDyeable || (helmet.Color != DisplayColor.Default)))
                headColor = obj.HairColor;
            else
                headColor = obj.HairColor;
        } else
            headColor = DisplayColor.Default;

        if (hideHelmet)
            headColor = obj.HairColor;

        // Determine boots sprite
        if (hideBoots || shouldOverrideBootsSprite)
            bootsSprite = 0;
        else if (boots?.ItemSprite.DisplaySprite is not null)
            bootsSprite = (byte)boots.ItemSprite.DisplaySprite;

        // Determine boots color
        bootsColor = bootsSprite != 0 ? boots?.Color ?? DisplayColor.Default : DisplayColor.Default;

        // Handle arena team overrides
        if (hasArenaTeam)
        {
            headSprite = (ushort)obj.HairStyle;

            headColor = arenaTeam switch
            {
                ArenaTeam.Blue  => DisplayColor.Cyan,
                ArenaTeam.Green => DisplayColor.Lime,
                ArenaTeam.Gold  => DisplayColor.NeonYellow,
                ArenaTeam.Red   => DisplayColor.Fire,
                _               => headColor
            };

            overcoat = null;
            acc1 = null;
            acc2 = null;
            acc3 = null;

            // Reset any hidden gear to make sure arena team members display everything
            hideHelmet = hideWeapon = hideShield = hideBoots = hideArmor = hideAcc1 = hideAcc2 = hideAcc3 = false;
        }

        return new DisplayAislingArgs
        {
            AccessoryColor1 = hideAcc1 ? DisplayColor.Default : acc1?.Color ?? DisplayColor.Default,
            AccessoryColor2 = hideAcc2 ? DisplayColor.Default : acc2?.Color ?? DisplayColor.Default,
            AccessoryColor3 = hideAcc3 ? DisplayColor.Default : acc3?.Color ?? DisplayColor.Default,
            AccessorySprite1 = hideAcc1 ? (ushort)0 : acc1?.ItemSprite?.DisplaySprite ?? 0,
            AccessorySprite2 = hideAcc2 ? (ushort)0 : acc2?.ItemSprite?.DisplaySprite ?? 0,
            AccessorySprite3 = hideAcc3 ? (ushort)0 : acc3?.ItemSprite?.DisplaySprite ?? 0,
            ArmorSprite1 = (ushort)(hideArmor ? 0 : armor?.ItemSprite?.DisplaySprite ?? 0),
            ArmorSprite2 = (ushort)(hideArmor ? 0 : armor?.ItemSprite?.DisplaySprite ?? 0),
            BodyColor = obj.BodyColor,
            BodySprite = obj.BodySprite,
            PantsColor = hideArmor ? DisplayColor.Default : pantsColor,
            BootsColor = bootsColor,
            BootsSprite = bootsSprite,
            Direction = obj.Direction,
            FaceSprite = (byte)obj.FaceSprite,
            Gender = obj.Gender,
            GroupBoxText = obj.GroupBox?.Name,
            HeadColor = headColor,
            HeadSprite = headSprite,
            Id = obj.Id,
            IsDead = obj.IsDead,
            IsHidden = false,
            IsTransparent = obj.Visibility is not VisibilityType.Normal,
            LanternSize = obj.LanternSize,
            Name = obj.Name,
            NameTagStyle = NameTagStyle.NeutralHover,
            OvercoatColor = overcoat?.Color ?? DisplayColor.Default,
            OvercoatSprite = overcoat?.ItemSprite?.DisplaySprite ?? 0,
            X = obj.X,
            Y = obj.Y,
            RestPosition = obj.RestPosition,
            ShieldSprite = (byte)shieldSprite,
            Sprite = obj.Sprite == 0 ? null : obj.Sprite,
            WeaponSprite = (ushort)(hideWeapon ? 0 : weapon?.ItemSprite?.DisplaySprite ?? 0)
        };
    }

    public Aisling Map(OtherProfileArgs obj) => throw new NotImplementedException();

    OtherProfileArgs IMapperProfile<Aisling, OtherProfileArgs>.Map(Aisling obj)
        => new()
        {
            DisplayClass = obj.UserStatSheet.AdvClass != AdvClass.None
                ? obj.UserStatSheet.AdvClass.ToString()
                : obj.UserStatSheet.Master
                    ? "Master"
                    : obj.UserStatSheet.BaseClass.ToString(),
            BaseClass = obj.UserStatSheet.BaseClass,
            Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>)!,
            GroupOpen = obj.Options.AllowGroup,
            GuildName = obj.Guild?.Name,
            GuildRank = obj.GuildRank,
            Id = obj.Id,
            LegendMarks = Mapper.MapMany<LegendMarkInfo>(obj.Legend)
                                .ToList(),
            Name = obj.Name,
            Nation = obj.Nation,
            Portrait = obj.Portrait,
            ProfileText = obj.ProfileText,
            SocialStatus = obj.Options.SocialStatus,
            Title = obj.Titles.FirstOrDefault()
        };

    public Aisling Map(SelfProfileArgs obj) => throw new NotImplementedException();

    SelfProfileArgs IMapperProfile<Aisling, SelfProfileArgs>.Map(Aisling obj)
        => new()
        {
            DisplayClass = obj.UserStatSheet.AdvClass != AdvClass.None
                ? obj.UserStatSheet.AdvClass.ToString()
                : obj.UserStatSheet.Master
                    ? "Master"
                    : obj.UserStatSheet.BaseClass.ToString(),
            BaseClass = obj.UserStatSheet.BaseClass,
            Equipment = obj.Equipment.ToDictionary(i => (EquipmentSlot)i.Slot, Mapper.Map<ItemInfo>),
            GroupOpen = obj.Options.AllowGroup,
            GroupString = obj.Group?.ToString(),
            GuildName = obj.Guild?.Name,
            GuildRank = obj.GuildRank,
            EnableMasterAbilityMetaData = obj.UserStatSheet.Master,
            EnableMasterQuestMetaData = obj.UserStatSheet.Master,
            LegendMarks = Mapper.MapMany<LegendMarkInfo>(obj.Legend)
                                .ToList(),
            Name = obj.Name,
            Nation = obj.Nation,
            Portrait = obj.Portrait,
            ProfileText = obj.ProfileText,
            SpouseName = null, //TODO: when we implement marraige i guess
            Title = obj.Titles.FirstOrDefault()
        };

    public Aisling Map(UserIdArgs obj) => throw new NotImplementedException();

    UserIdArgs IMapperProfile<Aisling, UserIdArgs>.Map(Aisling obj)
        => new()
        {
            BaseClass = obj.UserStatSheet.BaseClass,
            Direction = obj.Direction,
            Id = obj.Id
        };

    public Aisling Map(WorldListMemberInfo obj) => throw new NotImplementedException();

    WorldListMemberInfo IMapperProfile<Aisling, WorldListMemberInfo>.Map(Aisling obj)
        => new()
        {
            BaseClass = obj.UserStatSheet.BaseClass,
            Color = WorldListColor.White,
            IsMaster = obj.UserStatSheet.Master,
            Name = obj.Name,
            SocialStatus = obj.Options.SocialStatus,
            Title = obj.Titles.FirstOrDefault()
        };
}