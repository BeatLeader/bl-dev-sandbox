namespace BsorV2;

public readonly struct ReplaySectionsTable {
    public ReadOnlySpan<ReplaySection> Sections {
        get {
            unsafe {
                return new(_ptr, _size);
            }
        }
    }

    private readonly unsafe byte* _ptr;
    private readonly ushort _size;

    public unsafe ReplaySectionsTable(byte* ptr, ushort size) {
        _ptr = ptr;
        _size = size;
    }
}