using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplaySectionHeader {
    public ReadOnlySpan<byte> RawName {
        get {
            unsafe {
                return new(Unsafe.AsPointer(ref Id[0]), 64);
            }
        }
    }

    public ReadOnlySpan<byte> Name {
        get {
            var fullSpan = RawName;
            var nullIdx = fullSpan.IndexOf((byte)0);

            return nullIdx >= 0 ? fullSpan[..nullIdx] : fullSpan;
        }
    }

    private unsafe fixed byte Id[64];

    public readonly uint SectionOffset;
    public readonly uint SectionSize;
    public readonly uint ItemSize;
    public readonly uint ItemCount;
    public readonly uint FirstItemOffset;
}