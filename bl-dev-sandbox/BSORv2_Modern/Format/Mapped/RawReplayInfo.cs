using System.Runtime.InteropServices;

namespace BsorV2;

[StructLayout(LayoutKind.Sequential)]
public struct RawReplayInfo {
    public ReplayVersion OriginalVersion;

    private unsafe fixed byte _padding1[2];
    
    public RawUtf8String Version;
    public RawUtf8String GameVersion;

    public RawUtf8String PlayerUniqueId;
    public RawUtf8String PlayerId;
    public RawUtf16String PlayerName;

    public RawUtf8String Platform;
    public RawUtf8String TrackingSystem;
    public RawUtf8String Hmd;
    public RawUtf8String LeftController;
    public RawUtf8String RightController;

    public RawUtf8String Hash;
    public RawUtf8String Mode;
    public RawUtf8String Difficulty;
    public float Bpm;
    public float Njs;
    public RawUtf16String SongName;
    public RawUtf16String Mapper;

    public int Score;
    public int Timestamp;
    public int TimestampEnd;
    public float StartTime;
    public float EndTime;
    public int EndType;
    public float Speed;

    [MarshalAs(UnmanagedType.U1)]
    public bool Multiplayer;

    [MarshalAs(UnmanagedType.U1)]
    public bool LeftHanded;
    
    private unsafe fixed byte _padding2[2];

    public RawUtf8String Environment;
    public float Height;
    public float JumpDistance;
    public RawUtf8String Modifiers;

    public Vector3 LeftSaberPosition;
    public Quaternion LeftSaberRotation;

    public Vector3 RightSaberPosition;
    public Quaternion RightSaberRotation;

    public Vector3 RoomPosition;
    public float RoomRotation;
}