using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplayWall {
    public WallId Id;
    public float InTime;
    public float OutTime;
    public float SpawnTime;
    public float InEnergy;
    public float OutEnergy;
    public int Multiplier;
}

[StructLayout(LayoutKind.Sequential)]
public struct WallId {
    public int SpawnId;
    public int LineIndex;
    public int LineLayer;
    public int Width;
    public int Height;
    public float Duration;
}