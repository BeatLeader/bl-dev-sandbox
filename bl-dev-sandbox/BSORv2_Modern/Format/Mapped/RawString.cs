using System.Runtime.InteropServices;
using System.Text;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public readonly struct RawUtf8String {
    private readonly int _length;
    private readonly int _ptr;

    public unsafe string ToManaged(byte* buffer) {
        return Encoding.UTF8.GetString(buffer + _ptr, _length);
    }

    public unsafe LazyString ToLazy(byte* buffer) {
        return new(buffer, this);
    }
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct RawUtf16String {
    private readonly int _length;
    private readonly int _ptr;

    public unsafe string ToManaged(byte* buffer) {
        return new((char*)(buffer + _ptr), 0, _length / sizeof(char));
    }
    
    public unsafe LazyString ToLazy(byte* buffer) {
        return new(buffer, this);
    }
}