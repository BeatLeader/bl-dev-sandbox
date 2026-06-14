namespace BsorV2;

public readonly struct ReplayDecoderResult {
    public readonly bool Succeeded;
    public readonly string? Error;

    private ReplayDecoderResult(bool succeeded) {
        Succeeded = succeeded;
        Error = null;
    }

    private ReplayDecoderResult(string error) {
        Succeeded = false;
        Error = error;
    }

    public static ReplayDecoderResult MissingBlock(ReplaySectionKind block) {
        return new($"{block.DebugName} is a builtin block and must be presented");
    }

    public static ReplayDecoderResult InvalidBlockPacking() {
        return new($"Expected a builtin block, got custom. All builtin blocks must come before custom data");
    }

    public static ReplayDecoderResult InvalidMagic() {
        return new("Invalid file format");
    }

    public static ReplayDecoderResult Ok() {
        return new(true);
    }
}