namespace BsorV2;

public struct ReplayInfo {
    public required ReplayVersion OriginalVersion;
    public required LazyString Version;
    public required LazyString GameVersion;

    public required LazyString PlayerUniqueId;
    public required LazyString PlayerId;
    public required LazyString PlayerName;

    public required LazyString Platform;
    public required LazyString TrackingSystem;
    public required LazyString Hmd;
    public required LazyString LeftController;
    public required LazyString RightController;

    public required LazyString Hash;
    public required LazyString Mode;
    public required LazyString Difficulty;
    public required float Bpm;
    public required float Njs;
    public required LazyString SongName;
    public required LazyString Mapper;

    public required int Score;
    public required uint Timestamp;
    public required uint TimestampEnd;
    public required float StartTime;
    public required float EndTime;
    public required int EndType;
    public required float Speed;

    public required bool Multiplayer;
    public required bool LeftHanded;

    public required LazyString Environment;
    public required float Height;
    public required float JumpDistance;
    public required LazyString Modifiers;

    public required Vector3 LeftSaberPosition;
    public required Quaternion LeftSaberRotation;

    public required Vector3 RightSaberPosition;
    public required Quaternion RightSaberRotation;

    public required Vector3 RoomPosition;
    public required float RoomRotation;
}