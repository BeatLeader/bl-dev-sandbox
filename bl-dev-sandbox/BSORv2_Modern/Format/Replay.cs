namespace BsorV2;

public class Replay {
    public ref readonly ReplayInfo Info => ref _info;

    public ReadOnlyStridedSpan<ReplayFrame> Frames => _frames;

    public ReadOnlyStridedSpan<ReplayNote> Notes => _notes;

    public ReadOnlyStridedSpan<ReplayWall> Walls => _walls;

    public ReadOnlyStridedSpan<ReplayHeight> Heights => _heights;

    public ReadOnlyStridedSpan<ReplayPause> Pauses => _pauses;

    public ReadOnlySpan<ReplaySection> Sections => _sectionsTable.Sections;

    public readonly ReplayVersion Version;

    private readonly ReplayInfo _info;
    private readonly ReadOnlyStridedSpan<ReplayFrame> _frames;
    private readonly ReadOnlyStridedSpan<ReplayNote> _notes;
    private readonly ReadOnlyStridedSpan<ReplayWall> _walls;
    private readonly ReadOnlyStridedSpan<ReplayHeight> _heights;
    private readonly ReadOnlyStridedSpan<ReplayPause> _pauses;
    private readonly ReplaySectionsTable _sectionsTable;

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
        _info = info;
        _frames = frames;
        _notes = notes;
        _walls = walls;
        _heights = heights;
        _pauses = pauses;
        _sectionsTable = sectionsTable;
    }
}