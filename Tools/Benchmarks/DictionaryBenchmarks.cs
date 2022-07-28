using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace Benchmarks;

[MemoryDiagnoser]
public class DictionaryBenchmarks
{
    private readonly ConcurrentDictionary<string, string> ConcurrentDictionary = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> NormalDictionary = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> ValuesList = new();
    [Params(
        10,
        50,
        100,
        500,
        1000)]
    public int NumRecords;

    [Benchmark]
    public void ConcurrentSelectBenchmark()
    {
        var result = ConcurrentDictionary.Select(kvp => kvp.Value).ToList();
    }

    [Benchmark]
    public void ConcurrentValuesBenchmark()
    {
        var result = ConcurrentDictionary.Values.ToList();
    }

    [Benchmark(Baseline = true)]
    public void ListBenchmark()
    {
        var result = ValuesList.ToList();
    }

    [Benchmark]
    public void NormalSelectBenchmark()
    {
        var result = NormalDictionary.Select(kvp => kvp.Value).ToList();
    }

    [Benchmark]
    public void NormalValuesBenchmark()
    {
        var result = NormalDictionary.Values.ToList();
    }

    [GlobalSetup]
    public void Setup()
    {
        Randomizer.Seed = new Random(12345);

        var faker = new Faker();

        var randomKeys = Enumerable
                         .Range(0, NumRecords)
                         .Select(_ => faker.Random.String2(3, 15) + faker.UniqueIndex)
                         .ToList();

        var randomValues = Enumerable
                           .Range(0, NumRecords)
                           .Select(_ => faker.Random.String2(10, 20))
                           .ToList();

        foreach ((var key, var value) in randomKeys.Zip(randomValues))
        {
            NormalDictionary.Add(key, value);
            ConcurrentDictionary.TryAdd(key, value);
            ValuesList.Add(value);
        }
    }
}