using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplayHeight {
    public float Height;
    public float Time;
}