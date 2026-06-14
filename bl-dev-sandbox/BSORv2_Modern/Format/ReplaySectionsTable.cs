namespace BsorV2;

public readonly struct ReplaySectionsTable {
    public ReadOnlySpan<ReplaySectionHeader> Sections {
        get {
            unsafe {
                return new(_tablePtr, _rowsCount);
            }
        }
    }

    private readonly unsafe byte* _bufferPtr;
    private readonly unsafe byte* _tablePtr;
    private readonly ushort _rowsCount;

    public unsafe ReplaySectionsTable(byte* bufferPtr, byte* tablePtr, ushort rowsCount) {
        _bufferPtr = bufferPtr;
        _tablePtr = tablePtr;
        _rowsCount = rowsCount;
    }

    public ReadOnlySpan<byte> GetSection(in ReplaySectionHeader header) {
        unsafe {
            return new(_bufferPtr + header.SectionOffset, (int)header.SectionSize);
        }
    }

    public ReadOnlyStridedSpan<T> GetArraySection<T>(in ReplaySectionHeader header) where T : unmanaged {
        unsafe {
            return new(_bufferPtr + header.SectionOffset + header.FirstItemOffset, header.ItemCount, header.ItemSize);
        }
    }
}