using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace BsorV2;

static class Program {
    public static void Main(string[] args) {
        BenchmarkRunner.Run<ReplayDecoderBenchmark>();
    }
}

[MemoryDiagnoser]
[SimpleJob]
[RPlotExporter]
public unsafe class ReplayDecoderBenchmark {
    private byte* _ptr;

    [Params(10, 500, 1000, 5000, 10000, 50000, 100000, 500000)]
    public uint Size;

    [GlobalSetup]
    public void Setup() {
        _ptr = ReplayDummy.Create(Size, Size, Size, Size, Size);
    }

    [GlobalCleanup]
    public void Cleanup() {
        if (_ptr != null) {
            Marshal.FreeHGlobal((IntPtr)_ptr);
            _ptr = null;
        }
    }

    [Benchmark]
    public bool DecodeReplay() {
        var result = ReplayDecoder.TryDecode(_ptr, out _);
        return result.Succeeded;
    }
}