namespace BsorV2;

public enum ReplaySectionKind : ulong {
    Info = (ulong)'I' |
        (ulong)'N' << 8 |
        (ulong)'F' << 16 |
        (ulong)'O' << 24,

    Frames = (ulong)'F' |
        (ulong)'R' << 8 |
        (ulong)'A' << 16 |
        (ulong)'M' << 24 |
        (ulong)'E' << 32 |
        (ulong)'S' << 40,

    Notes = (ulong)'N' |
        (ulong)'O' << 8 |
        (ulong)'T' << 16 |
        (ulong)'E' << 24 |
        (ulong)'S' << 32,

    Walls = (ulong)'W' |
        (ulong)'A' << 8 |
        (ulong)'L' << 16 |
        (ulong)'L' << 24 |
        (ulong)'S' << 32,

    Heights = (ulong)'H' |
        (ulong)'E' << 8 |
        (ulong)'I' << 16 |
        (ulong)'G' << 24 |
        (ulong)'H' << 32 |
        (ulong)'T' << 40 |
        (ulong)'S' << 48,

    Pauses = (ulong)'P' |
        (ulong)'A' << 8 |
        (ulong)'U' << 16 |
        (ulong)'S' << 24 |
        (ulong)'E' << 32 |
        (ulong)'S' << 40
}

public static class ReplaySectionExtension {
    extension(ReplaySectionKind kind) {
        public static int BuiltinSectionsCount => 6;

        public string DebugName {
            get {
                return kind switch {
                    ReplaySectionKind.Info => "Info",
                    ReplaySectionKind.Frames => "Frames",
                    ReplaySectionKind.Notes => "Notes",
                    ReplaySectionKind.Walls => "Walls",
                    ReplaySectionKind.Heights => "Heights",
                    ReplaySectionKind.Pauses => "Pauses",
                    _ => "Unknown"
                };
            }
        }
    }
}