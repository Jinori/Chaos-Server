// ReSharper disable InconsistentNaming

using System.Text;
using Chaos.DarkAges.Extensions;

namespace Chaos.Models.Data;

public record Stats
{
    protected int _con;
    protected int _dex;
    protected int _int;
    protected int _str;
    protected int _wis;

    public int Con
    {
        get => _con;
        init => _con = value;
    }

    public int Dex
    {
        get => _dex;
        init => _dex = value;
    }

    public int Int
    {
        get => _int;
        init => _int = value;
    }

    public int Str
    {
        get => _str;
        init => _str = value;
    }

    public int Wis
    {
        get => _wis;
        init => _wis = value;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        //only return the properties that are not 0
        var sb = new StringBuilder();
        if (Str != 0)
            sb.AppendLineF($"Str: {Str}");
        
        if(Int != 0)
            sb.AppendLineF($"Int: {Int}");
        
        if(Wis != 0)
            sb.AppendLineF($"Wis: {Wis}");

        if (Con != 0)
            sb.AppendLineF($"Con: {Con}");

        if (Dex != 0)
            sb.AppendLineF($"Dex: {Dex}");

        return sb.ToString();
    }
}