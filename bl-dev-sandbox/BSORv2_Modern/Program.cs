using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BsorV2;

static class Program {
    public static unsafe void Main(string[] args) {
        // Tests that all structures are aligned properly
        // StructAlignmentTests.AlignsStructuresProperly();

        var ptr = ReplayDummy.Create(100000, 100000, 100000, 100000, 100000);

        const int iterations = 1000;

        var minTicks = long.MaxValue;
        var maxTicks = long.MinValue;
        long totalTicks = 0;

        Console.WriteLine($"Warming up and benchmarking {iterations} decodes...");

        ReplayDecoder.TryDecode(ptr, out _);

        // Run the benchmark loop
        for (var i = 0; i < iterations; i++) {
            var start = Stopwatch.GetTimestamp();

            var result = ReplayDecoder.TryDecode(ptr, out var replay);

            // Data can be read like this
            //ref readonly var frame = ref replay!.Frames[10];

            var end = Stopwatch.GetTimestamp();
            var elapsedTicks = end - start;

            if (result.Succeeded) {
                if (elapsedTicks < minTicks) minTicks = elapsedTicks;
                if (elapsedTicks > maxTicks) maxTicks = elapsedTicks;
                totalTicks += elapsedTicks;
            }
        }

        // Convert the tracked high-resolution timestamp ticks to standard TimeSpans
        var minTime = Stopwatch.GetElapsedTime(0, minTicks);
        var maxTime = Stopwatch.GetElapsedTime(0, maxTicks);
        var avgTime = Stopwatch.GetElapsedTime(0, totalTicks / iterations);

        // Print results formatted clearly
        Console.WriteLine("\n--- Benchmark Results ---");
        Console.WriteLine($"Min Time: {minTime.TotalMilliseconds:F4} ms ({minTime})");
        Console.WriteLine($"Max Time: {maxTime.TotalMilliseconds:F4} ms ({maxTime})");
        Console.WriteLine($"Avg Time: {avgTime.TotalMilliseconds:F4} ms ({avgTime})");

        // Clean up the native memory allocated by the dummy generator
        Marshal.FreeHGlobal((IntPtr)ptr);
    }
}