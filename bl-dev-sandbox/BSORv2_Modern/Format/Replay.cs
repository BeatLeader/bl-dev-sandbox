namespace BsorV2;

public class Replay {
    public readonly ReplayVersion Version;
    public readonly ReplayInfo Info;
    public readonly ReadOnlyStridedSpan<ReplayFrame> Frames;
    public readonly ReadOnlyStridedSpan<ReplayNote> Notes;
    public readonly ReadOnlyStridedSpan<ReplayWall> Walls;
    public readonly ReadOnlyStridedSpan<ReplayHeight> Heights;
    public readonly ReadOnlyStridedSpan<ReplayPause> Pauses;
    public readonly ReplaySectionsTable SectionsTable;

    public Replay(
        in ReplayVersion version,
        in ReplayInfo info,
        ReadOnlyStridedSpan<ReplayFrame> frames,
        ReadOnlyStridedSpan<ReplayNote> notes,
        ReadOnlyStridedSpan<ReplayWall> walls,
        ReadOnlyStridedSpan<ReplayHeight> heights,
        ReadOnlyStridedSpan<ReplayPause> pauses,
        in ReplaySectionsTable sectionsTable
    ) {
        Version = version;
        Info = info;
        Frames = frames;
        Notes = notes;
        Walls = walls;
        Heights = heights;
        Pauses = pauses;
        SectionsTable = sectionsTable;
    }
}