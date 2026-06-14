namespace BsorV2;

public readonly struct ReplaySection {
    public ReadOnlySpan<byte> Name {
        get {
            unsafe {
                return new(_namePtr, 64);
            }
        }
    }

    public ReadOnlySpan<byte> Data {
        get {
            unsafe {
                return new(_dataPtr, (int)_dataSize);
            }
        }
    }
    
    public readonly ulong ItemSize;
    public readonly ulong ItemCount;

    private readonly unsafe byte* _namePtr;
    private readonly unsafe byte* _dataPtr;
    private readonly ulong _dataSize;

    public unsafe ReplaySection(byte* namePtr, byte* dataPtr, ulong dataSize, ulong itemSize, ulong itemCount) {
        _namePtr = namePtr;
        _dataPtr = dataPtr;
        _dataSize = dataSize;
        ItemSize = itemSize;
        ItemCount = itemCount;
    }
}