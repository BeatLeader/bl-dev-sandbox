using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct ReplayNote {
    public NoteId Id;
    public float EventTime;
    public float CutTime;
    public float SpawnTime;
    public NoteEventType EventType;
    public float TimeScale;
    public float UnityTimeScale;
    public Vector3 Position;
    public Quaternion Rotation;
    public NoteScore Score;
    public CutInfo Cut;
}

public enum NoteEventType : int {
    Good = 0,
    Bad = 1,
    Miss = 2,
    Bomb = 3
}

[StructLayout(LayoutKind.Sequential)]
public struct NoteId {
    public int SpawnId;
    public int ScoringType;
    public int LineIndex;
    public int LineLayer;
    public int ColorType;
    public int CutDirection;
    public int AngleOffset;
}

[StructLayout(LayoutKind.Sequential)]
public struct NoteScore {
    public int Multiplier;
    public int TotalScore;
    public int FcScore;
    public int MaxScore;
    public int Combo;
    public int Energy;
}

[StructLayout(LayoutKind.Sequential)]
public struct CutInfo {
    [MarshalAs(UnmanagedType.U1)]
    public bool SpeedOk;

    [MarshalAs(UnmanagedType.U1)]
    public bool DirectionOk;

    [MarshalAs(UnmanagedType.U1)]
    public bool SaberTypeOk;

    [MarshalAs(UnmanagedType.U1)]
    public bool WasCutTooSoon;

    public float SaberSpeed;
    public Vector3 SaberDir;
    public int SaberType;
    public float TimeDeviation;
    public float CutDirDeviation;
    public Vector3 CutPoint;
    public Vector3 CutNormal;
    public float CutDistanceToCenter;
    public float CutAngle;
    public float BeforeCutRating;
    public float AfterCutRating;
    public int CutDistanceSign;
}