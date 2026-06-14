using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplayHeader {
    public int Magic;
    public ReplayVersion Version;
    public ushort TableRowsCount;
}

[StructLayout(LayoutKind.Sequential)]
public struct ReplaySectionsTableRow {
    public unsafe fixed byte Id[64];
    public ulong SectionOffset;
    public ulong SectionSize;
    public ulong ItemSize;
    public ulong ItemCount;
}

[StructLayout(LayoutKind.Sequential)]
public struct ReplayVersion {
    public byte Major;
    public byte Minor;
}