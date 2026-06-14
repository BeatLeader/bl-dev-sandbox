using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BsorV2;

[DebuggerDisplay("Length = {Length}")]
[DebuggerTypeProxy(typeof(ReadOnlyStridedSpanDebugProxy<>))]
public readonly struct ReadOnlyStridedSpan<T> where T : unmanaged {
    private readonly unsafe byte* _ptr;
    private readonly uint _length;
    private readonly ulong _stride;

    public uint Length => _length;
    public bool IsEmpty => _length == 0;

    public unsafe ReadOnlyStridedSpan(byte* ptr, uint length, ulong stride) {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
            throw new ArgumentException("Value cannot be a reference type", nameof(ptr));
        }

        _ptr = ptr;
        _length = length;
        _stride = stride;
    }

    public ref readonly T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)index >= _length) {
                throw new IndexOutOfRangeException();
            }

            unsafe {
                return ref Unsafe.AsRef<T>(_ptr + (ulong)index * _stride);
            }
        }
    }
}

internal sealed class ReadOnlyStridedSpanDebugProxy<T> where T : unmanaged {
    private readonly ReadOnlyStridedSpan<T> _span;

    public ReadOnlyStridedSpanDebugProxy(ReadOnlyStridedSpan<T> span) {
        _span = span;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] Items {
        get {
            var items = new T[_span.Length];
            for (var i = 0; i < items.Length; i++) {
                items[i] = _span[i];
            }
            return items;
        }
    }
}