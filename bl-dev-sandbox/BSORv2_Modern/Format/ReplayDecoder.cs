using System.Runtime.CompilerServices;
using static BsorV2.ReplaySectionKind;
using static BsorV2.ReplayDecoderResult;

namespace BsorV2;

public static class ReplayDecoder {
    public const int Magic = 0x442d3d69;

    public static unsafe ReplayDecoderResult TryDecode(byte* buffer, out Replay? replay) {
        var header = (ReplayHeader*)buffer;

        if (header->Magic != Magic) {
            replay = null;
            return InvalidMagic();
        }

        var tableSize = header->TableRowsCount;
        var rows = (ReplaySectionsTableRow*)(buffer + sizeof(ReplayHeader));

        ReplayInfo? info = null;
        ReadOnlyStridedSpan<ReplayFrame>? frames = null;
        ReadOnlyStridedSpan<ReplayNote>? notes = null;
        ReadOnlyStridedSpan<ReplayWall>? walls = null;
        ReadOnlyStridedSpan<ReplayHeight>? heights = null;
        ReadOnlyStridedSpan<ReplayPause>? pauses = null;

        for (var i = 0; i < ReplaySectionKind.BuiltinSectionsCount; i++) {
            var row = &rows[i];
            var id = *(ReplaySectionKind*)row->Id;

            switch (id) {
                case Info:
                    var infoPtr = (RawReplayInfo*)(buffer + row->SectionOffset);
                    info = ToReplayInfo(buffer, infoPtr);
                    break;

                case Frames:
                    frames = ToSpan<ReplayFrame>(buffer, row);
                    break;

                case Notes:
                    notes = ToSpan<ReplayNote>(buffer, row);
                    break;

                case Walls:
                    walls = ToSpan<ReplayWall>(buffer, row);
                    break;

                case Heights:
                    heights = ToSpan<ReplayHeight>(buffer, row);
                    break;

                case Pauses:
                    pauses = ToSpan<ReplayPause>(buffer, row);
                    break;

                default:
                    replay = null;
                    return InvalidBlockPacking();
            }
        }

        if (info == null) {
            replay = null;
            return MissingBlock(Info);
        }

        if (frames == null) {
            replay = null;
            return MissingBlock(Frames);
        }

        if (notes == null) {
            replay = null;
            return MissingBlock(Notes);
        }

        if (walls == null) {
            replay = null;
            return MissingBlock(Walls);
        }

        if (heights == null) {
            replay = null;
            return MissingBlock(Heights);
        }

        if (pauses == null) {
            replay = null;
            return MissingBlock(Pauses);
        }

        var skippedSections = ReplaySectionKind.BuiltinSectionsCount;

        var sectionsRemainderPtr = buffer + sizeof(ReplayHeader) + skippedSections * sizeof(ReplaySectionsTableRow);
        var sectionsRemainder = tableSize - skippedSections;

        var sectionsTable = new ReplaySectionsTable(buffer, sectionsRemainderPtr, (ushort)sectionsRemainder);

        replay = new(
            header->Version,
            info.Value,
            frames.Value,
            notes.Value,
            walls.Value,
            heights.Value,
            pauses.Value,
            sectionsTable
        );

        return Ok();
    }

    private static unsafe ReplayInfo ToReplayInfo(byte* buffer, RawReplayInfo* rawInfo) {
        return new() {
            OriginalVersion = rawInfo->OriginalVersion,

            Version = rawInfo->Version.ToLazy(buffer),
            GameVersion = rawInfo->GameVersion.ToLazy(buffer),

            PlayerUniqueId = rawInfo->PlayerUniqueId.ToLazy(buffer),
            PlayerId = rawInfo->PlayerId.ToLazy(buffer),
            PlayerName = rawInfo->PlayerName.ToLazy(buffer),

            Platform = rawInfo->Platform.ToLazy(buffer),
            TrackingSystem = rawInfo->TrackingSystem.ToLazy(buffer),
            Hmd = rawInfo->Hmd.ToLazy(buffer),
            LeftController = rawInfo->LeftController.ToLazy(buffer),
            RightController = rawInfo->RightController.ToLazy(buffer),

            Hash = rawInfo->Hash.ToLazy(buffer),
            Mode = rawInfo->Mode.ToLazy(buffer),
            Difficulty = rawInfo->Difficulty.ToLazy(buffer),

            Bpm = rawInfo->Bpm,
            Njs = rawInfo->Njs,
            SongName = rawInfo->SongName.ToLazy(buffer),
            Mapper = rawInfo->Mapper.ToLazy(buffer),

            Score = rawInfo->Score,
            Timestamp = rawInfo->Timestamp,
            TimestampEnd = rawInfo->TimestampEnd,
            StartTime = rawInfo->StartTime,
            EndTime = rawInfo->EndTime,
            EndType = rawInfo->EndType,
            Speed = rawInfo->Speed,

            Multiplayer = rawInfo->Multiplayer,
            LeftHanded = rawInfo->LeftHanded,

            Environment = rawInfo->Environment.ToLazy(buffer),
            Height = rawInfo->Height,
            JumpDistance = rawInfo->JumpDistance,
            Modifiers = rawInfo->Modifiers.ToLazy(buffer),

            LeftSaberPosition = rawInfo->LeftSaberPosition,
            LeftSaberRotation = rawInfo->LeftSaberRotation,

            RightSaberPosition = rawInfo->RightSaberPosition,
            RightSaberRotation = rawInfo->RightSaberRotation,

            RoomPosition = rawInfo->RoomPosition,
            RoomRotation = rawInfo->RoomRotation
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe ReadOnlyStridedSpan<T> ToSpan<T>(byte* buffer, ReplaySectionsTableRow* row) where T : unmanaged {
        return new(buffer + row->SectionOffset, (uint)row->ItemCount, row->ItemSize);
    }
}