using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplayPause {
    public float Duration;
    public int Time;
}