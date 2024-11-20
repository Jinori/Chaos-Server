// ReSharper disable InconsistentNaming

using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;

namespace Chaos.Models.Data;

public sealed record UserStatSheet : StatSheet
{
    private AdvClass _advClass;
    private BaseClass _baseClass;

    // ReSharper disable once UnassignedField.Global
    private int _currentWeight;
    private bool _master;
    private int _maxWeight;
    private long _toNextAbility;
    private long _toNextLevel;
    private long _totalAbility;
    private long _totalExp;
    private int _unspentPoints;

    public AdvClass AdvClass
    {
        get => _advClass;
        init => _advClass = value;
    }

    public BaseClass BaseClass
    {
        get => _baseClass;
        init => _baseClass = value;
    }

    public int CurrentWeight
    {
        get => _currentWeight;
        init => _currentWeight = value;
    }

    public bool Master
    {
        get => _master;
        init => _master = value;
    }

    public int MaxWeight
    {
        get => _maxWeight;
        init => _maxWeight = value;
    }

    public uint ToNextAbility
    {
        get => Convert.ToUInt32(_toNextAbility);
        init => _toNextAbility = value;
    }

    public uint ToNextLevel
    {
        get => Convert.ToUInt32(_toNextLevel);
        set => _toNextLevel = value;
    }

    public uint TotalAbility
    {
        get => Convert.ToUInt32(_totalAbility);
        init => _totalAbility = value;
    }

    public uint TotalExp
    {
        get => Convert.ToUInt32(_totalExp);
        init => _totalExp = value;
    }

    public int UnspentPoints
    {
        get => _unspentPoints;
        set => _unspentPoints = value;
    }

    public static UserStatSheet NewCharacter
        => new()
        {
            _ac = 100,
            _maxWeight = 51,
            _toNextLevel = 599,
            _str = 3,
            _int = 3,
            _wis = 3,
            _con = 3,
            _dex = 3,
            _currentHp = 49,
            _maximumHp = 49,
            _currentMp = 33,
            _maximumMp = 33,
            _level = 1,
            _master = false,
            _baseClass = BaseClass.Peasant,
            _advClass = AdvClass.None
        };

    public long AddTna(long amount)
    {
        var ret = Interlocked.Add(ref _toNextAbility, amount);

        if (_toNextAbility > uint.MaxValue)
        {
            _toNextAbility = uint.MaxValue;

            return uint.MaxValue;
        }

        return ret;
    }

    public long AddTnl(long amount)
    {
        var ret = Interlocked.Add(ref _toNextLevel, amount);

        if (_toNextLevel > uint.MaxValue)
        {
            _toNextLevel = uint.MaxValue;

            return uint.MaxValue;
        }

        return ret;
    }

    public long AddTotalAbility(long amount)
    {
        var ret = Interlocked.Add(ref _totalAbility, amount);

        if (_totalAbility > uint.MaxValue)
        {
            _totalAbility = uint.MaxValue;

            return uint.MaxValue;
        }

        return ret;
    }

    public long AddTotalExp(long amount)
    {
        var ret = Interlocked.Add(ref _totalExp, amount);

        if (_totalExp > uint.MaxValue)
        {
            _totalExp = uint.MaxValue;

            return uint.MaxValue;
        }

        return ret;
    }

    public int AddWeight(int amount) => Interlocked.Add(ref _currentWeight, amount);

    public int GivePoints(int amount) => Interlocked.Add(ref _unspentPoints, amount);

    private static ClassStatBracket GetCurrentStatBracket(Aisling source)
    {
        if (source.UserStatSheet.Master)
            return ClassStatBracket.Master;

        return source.Trackers.Enums.HasValue(ClassStatBracket.Grandmaster) 
            ? ClassStatBracket.Grandmaster 
            : ClassStatBracket.PreMaster;
    }
    
    private readonly Dictionary<BaseClass, Dictionary<ClassStatBracket, Attributes>> ClassStatCaps = new()
    {
        {
            BaseClass.Warrior, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 120, Int = 50, Wis = 50, Con = 80, Dex = 100 } },
                { ClassStatBracket.Master, new Attributes { Str = 180, Int = 80, Wis = 80, Con = 120, Dex = 150 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 215, Int = 100, Wis = 100, Con = 150, Dex = 180 } }
            }
        },
        {
            BaseClass.Monk, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 100, Int = 50, Wis = 50, Con = 120, Dex = 80 } },
                { ClassStatBracket.Master, new Attributes { Str = 150, Int = 80, Wis = 80, Con = 180, Dex = 120 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 180, Int = 100, Wis = 100, Con = 215, Dex = 150 } }
            }
        },
        {
            BaseClass.Rogue, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 100, Int = 50, Wis = 50, Con = 80, Dex = 120 } },
                { ClassStatBracket.Master, new Attributes { Str = 150, Int = 80, Wis = 80, Con = 120, Dex = 180 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 180, Int = 100, Wis = 100, Con = 150, Dex = 215 } }
            }
        },
        {
            BaseClass.Wizard, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 50, Int = 120, Wis = 100, Con = 80, Dex = 50 } },
                { ClassStatBracket.Master, new Attributes { Str = 80, Int = 180, Wis = 150, Con = 120, Dex = 80 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 100, Int = 215, Wis = 180, Con = 150, Dex = 100 } }
            }
        },
        {
            BaseClass.Priest, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 50, Int = 100, Wis = 120, Con = 80, Dex = 50 } },
                { ClassStatBracket.Master, new Attributes { Str = 80, Int = 150, Wis = 180, Con = 120, Dex = 80 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 100, Int = 180, Wis = 215, Con = 150, Dex = 100 } }
            }
        },
        
        {
            BaseClass.Peasant, new Dictionary<ClassStatBracket, Attributes>
            {
                { ClassStatBracket.PreMaster, new Attributes { Str = 20, Int = 20, Wis = 20, Con = 20, Dex = 20 } },
                { ClassStatBracket.Master, new Attributes { Str = 20, Int = 20, Wis = 20, Con = 20, Dex = 20 } },
                { ClassStatBracket.Grandmaster, new Attributes { Str = 20, Int = 20, Wis = 20, Con = 20, Dex = 20 } }
            }
        },
    };
    
    public bool IncrementStat(Stat stat, Aisling aisling, bool overrideUnspentStatCheck = false)
    {
        var currentBracket = GetCurrentStatBracket(aisling);
        var statCaps = ClassStatCaps[BaseClass][currentBracket];

        if (IsStatCapped(stat, statCaps))
        {
            aisling.SendOrangeBarMessage("You've reached the stat cap for this attribute.");
            return false;   
        }
        
        if (!overrideUnspentStatCheck && _unspentPoints == 0)
            return false;

        if (!overrideUnspentStatCheck && Interlocked.Decrement(ref _unspentPoints) < 0)
        {
            _unspentPoints = 0;
            return false;
        }

        switch (stat)
        {
            case Stat.STR:
                Interlocked.Increment(ref _str);
                break;
            case Stat.INT:
                Interlocked.Increment(ref _int);
                break;
            case Stat.WIS:
                Interlocked.Increment(ref _wis);
                break;
            case Stat.CON:
                Interlocked.Increment(ref _con);
                break;
            case Stat.DEX:
                Interlocked.Increment(ref _dex);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
        }

        return true;
    }

    private bool IsStatCapped(Stat stat, Attributes statCaps)
    {
        return stat switch
        {
            Stat.STR => _str >= statCaps.Str,
            Stat.INT => _int >= statCaps.Int,
            Stat.WIS => _wis >= statCaps.Wis,
            Stat.CON => _con >= statCaps.Con,
            Stat.DEX => _dex >= statCaps.Dex,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }


    public void SetAdvClass(AdvClass advClass) => _advClass = advClass;

    public void SetBaseClass(BaseClass baseClass) => _baseClass = baseClass;

    public void SetIsMaster(bool isMaster) => _master = isMaster;

    public void SetMaxWeight(int maxWeight) => _maxWeight = maxWeight;

    public long SubtractTna(long amount)
    {
        var ret = Interlocked.Add(ref _toNextAbility, -amount);

        if (_toNextAbility < 0)
        {
            _toNextAbility = 0;

            return 0;
        }

        return ret;
    }

    public long SubtractTnl(long amount)
    {
        var ret = Interlocked.Add(ref _toNextLevel, -amount);

        if (_toNextLevel < 0)
        {
            _toNextLevel = 0;

            return 0;
        }

        return ret;
    }

    public long SubtractTotalAbility(long amount)
    {
        var ret = Interlocked.Add(ref _totalAbility, -amount);

        if (_totalAbility < 0)
        {
            _totalAbility = 0;

            return 0;
        }

        return ret;
    }

    public long SubtractTotalExp(long amount)
    {
        var ret = Interlocked.Add(ref _totalExp, -amount);

        if (_totalExp < 0)
        {
            _totalExp = 0;

            return 0;
        }

        return ret;
    }

    public bool TrySubtractTotalAbility(long amount)
    {
        if (Interlocked.Add(ref _totalAbility, -amount) < 0)
        {
            Interlocked.Add(ref _totalAbility, amount);

            return false;
        }

        return true;
    }

    public bool TrySubtractTotalExp(long amount)
    {
        if (Interlocked.Add(ref _totalExp, -amount) < 0)
        {
            Interlocked.Add(ref _totalExp, amount);

            return false;
        }

        return true;
    }
}