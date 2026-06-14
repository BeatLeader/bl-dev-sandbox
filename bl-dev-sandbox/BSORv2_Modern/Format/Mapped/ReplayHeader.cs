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
    public uint SectionOffset;
    public uint SectionSize;
    public uint ItemSize;
    public uint ItemCount;
    public uint FirstItemOffset;
}

[StructLayout(LayoutKind.Sequential)]
public struct ReplayVersion {
    public byte Major;
    public byte Minor;
}