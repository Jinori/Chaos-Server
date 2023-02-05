// ReSharper disable InconsistentNaming

using Chaos.Common.Definitions;

namespace Chaos.Data;

public sealed record UserStatSheet : StatSheet
{
    public delegate void ReferentialAction(UserStatSheetRef statSheetRef);

    public delegate T ReferentialFunc<out T>(UserStatSheetRef statSheetRef);

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
        init => _toNextLevel = value;
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
        init => _unspentPoints = value;
    }

    public static UserStatSheet NewCharacter => new()
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
        _advClass = AdvClass.None,
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

    public void Assert(ReferentialAction action) => action(
        new UserStatSheetRef(
            ref _currentWeight,
            ref _totalExp,
            ref _totalAbility,
            ref _toNextLevel,
            ref _toNextAbility,
            ref _unspentPoints));

    public T Assert<T>(ReferentialFunc<T> func) => func(
        new UserStatSheetRef(
            ref _currentWeight,
            ref _totalExp,
            ref _totalAbility,
            ref _toNextLevel,
            ref _toNextAbility,
            ref _unspentPoints));

    public int GivePoints(int amount) => Interlocked.Add(ref _unspentPoints, amount);

    public void IncrementLevel() => Interlocked.Increment(ref _level);

    public bool IncrementStat(Stat stat)
    {
        //if it's 0, do nothing
        if (_unspentPoints == 0)
            return false;

        if (Interlocked.Decrement(ref _unspentPoints) < 0)
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

    public void SetAdvClass(AdvClass advClass) => _advClass = advClass;

    public void SetBaseClass(BaseClass baseClass) => _baseClass = baseClass;

    public void SetIsMaster(bool isMaster) => _master = isMaster;

    public void SetMaxWeight(int maxWeight) => _maxWeight = maxWeight;

    public long TakeTna(long amount)
    {
        var ret = Interlocked.Add(ref _toNextAbility, -amount);

        if (_toNextAbility < 0)
        {
            _toNextAbility = 0;

            return 0;
        }

        return ret;
    }

    public long TakeTnl(long amount)
    {
        var ret = Interlocked.Add(ref _toNextLevel, -amount);

        if (_toNextLevel < 0)
        {
            _toNextLevel = 0;

            return 0;
        }

        return ret;
    }

    public long TakeTotalAbility(long amount)
    {
        var ret = Interlocked.Add(ref _totalAbility, -amount);

        if (_totalAbility < 0)
        {
            _totalAbility = 0;

            return 0;
        }

        return ret;
    }

    public long TakeTotalExp(long amount)
    {
        var ret = Interlocked.Add(ref _totalExp, -amount);

        if (_totalExp < 0)
        {
            _totalExp = 0;

            return 0;
        }

        return ret;
    }

    public ref struct UserStatSheetRef
    {
        public ref int CurrentWeight;
        public ref long ToNextAbility;
        public ref long ToNextLevel;
        public ref long TotalAbility;
        public ref long TotalExp;
        public ref int UnspentPoints;

        public UserStatSheetRef(
            ref int currentWeight,
            ref long totalExp,
            ref long totalAbility,
            ref long toNextLevel,
            ref long toNextAbility,
            ref int unspentPoints
        )
        {
            CurrentWeight = ref currentWeight;
            TotalExp = ref totalExp;
            TotalAbility = ref totalAbility;
            ToNextLevel = ref toNextLevel;
            ToNextAbility = ref toNextAbility;
            UnspentPoints = ref unspentPoints;
        }
    }
}