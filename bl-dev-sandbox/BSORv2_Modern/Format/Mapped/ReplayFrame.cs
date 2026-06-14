using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplayFrame {
    public float Time;
    public int Fps;
    public Pose Head;
    public Pose LeftHand;
    public Pose RightHand;
}

[StructLayout(LayoutKind.Sequential)]
public struct Vector3 {
    public float X, Y, Z;
}

[StructLayout(LayoutKind.Sequential)]
public struct Quaternion {
    public float X, Y, Z, W;
}

[StructLayout(LayoutKind.Sequential)]
public struct Pose {
    public Vector3 Position;
    public Quaternion Rotation;
}