using BenchmarkDotNet.Running;

namespace Benchmarks;

public static class Program
{
    public static void Main() => BenchmarkRunner.Run<StringProcessorBenchmarks>();
}