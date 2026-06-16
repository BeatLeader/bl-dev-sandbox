using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BsorV2;

public static unsafe class ReplayDummy {
    public static byte* Create(
        uint frameCount = 10,
        uint noteCount = 5,
        uint wallCount = 2,
        uint heightCount = 1,
        uint pauseCount = 0
    ) {
        // 1. Define dummy string payloads for RawReplayInfo fields
        var vStr = "2.0.0";
        var gStr = "1.34.2";
        var pUniqueStr = "76561198000000000";
        var pIdStr = "123456";
        var pNameStr = "PlayerOne";
        var platStr = "Steam";
        var trackStr = "SteamVR";
        var hmdStr = "Index";
        var lCtrlStr = "Knuckles_Left";
        var rCtrlStr = "Knuckles_Right";
        var hashStr = "0123456789ABCDEF0123456789ABCDEF";
        var modeStr = "Standard";
        var diffStr = "ExpertPlus";
        var songStr = "FitBeat";
        var mapperStr = "BeatSaber";
        var envStr = "FitBeatEnvironment";
        var modStr = "FS, NO";

        // Calculate explicit string block size based on UTF encoding properties
        var tableSize = (ushort)ReplaySectionKind.BuiltinSectionsCount;

        var headerSize = sizeof(ReplayHeader);
        var tableBytes = tableSize * sizeof(ReplaySectionsTableRow);

        var infoScalarsSize = sizeof(RawReplayInfo);
        var framesBlockSize = frameCount * sizeof(ReplayFrame);
        var notesBlockSize = noteCount * sizeof(ReplayNote);
        var wallsBlockSize = wallCount * sizeof(ReplayWall);
        var heightsBlockSize = heightCount * sizeof(ReplayHeight);
        var pausesBlockSize = pauseCount * sizeof(ReplayPause);

        // Compute memory footprint requirements for the local string chunks (Strict Length Tracking)
        uint infoStringsSize = 0;
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(vStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(gStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(pUniqueStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(pIdStr);
        infoStringsSize += (uint)Encoding.Unicode.GetByteCount(pNameStr); // UTF-16
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(platStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(trackStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(hmdStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(lCtrlStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(rCtrlStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(hashStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(modeStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(diffStr);
        infoStringsSize += (uint)Encoding.Unicode.GetByteCount(songStr);   // UTF-16
        infoStringsSize += (uint)Encoding.Unicode.GetByteCount(mapperStr); // UTF-16
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(envStr);
        infoStringsSize += (uint)Encoding.UTF8.GetByteCount(modStr);

        // Total info block layout contains both the fixed scalars size and its trailing string blob data
        var totalInfoBlockSize = (uint)infoScalarsSize + infoStringsSize;

        // 2. Track linear storage positioning layout offsets
        var currentOffset = (uint)(headerSize + tableBytes);

        var infoOffset = currentOffset;
        var infoBlobOffset = infoOffset + (uint)infoScalarsSize; // String blob starts precisely after fixed scalars
        currentOffset += totalInfoBlockSize;

        var framesOffset = currentOffset;
        currentOffset += (uint)framesBlockSize;
        var notesOffset = currentOffset;
        currentOffset += (uint)notesBlockSize;
        var wallsOffset = currentOffset;
        currentOffset += (uint)wallsBlockSize;
        var heightsOffset = currentOffset;
        currentOffset += (uint)heightsBlockSize;
        var pausesOffset = currentOffset;
        currentOffset += (uint)pausesBlockSize;

        // Allocate block space layout
        var totalBuffer = Marshal.AllocHGlobal((IntPtr)currentOffset);
        var buffer = (byte*)totalBuffer.ToPointer();

        // 3. Write Replay Header
        var header = (ReplayHeader*)buffer;
        header->Magic = ReplayDecoder.Magic;
        header->Version = new ReplayVersion { Major = 2, Minor = 0 };
        header->TableRowsCount = tableSize;

        // 4. Populate rows inside the Table Map
        var rows = (ReplaySectionsTableRow*)(buffer + headerSize);

        PopulateRow(&rows[0], (ulong)ReplaySectionKind.Info, infoOffset, totalInfoBlockSize, (uint)sizeof(RawReplayInfo), 1, infoBlobOffset);
        PopulateRow(&rows[1], (ulong)ReplaySectionKind.Frames, framesOffset, (uint)framesBlockSize, (uint)sizeof(ReplayFrame), frameCount, 0);
        PopulateRow(&rows[2], (ulong)ReplaySectionKind.Notes, notesOffset, (uint)notesBlockSize, (uint)sizeof(ReplayNote), noteCount, 0);
        PopulateRow(&rows[3], (ulong)ReplaySectionKind.Walls, wallsOffset, (uint)wallsBlockSize, (uint)sizeof(ReplayWall), wallCount, 0);
        PopulateRow(&rows[4], (ulong)ReplaySectionKind.Heights, heightsOffset, (uint)heightsBlockSize, (uint)sizeof(ReplayHeight), heightCount, 0);
        PopulateRow(&rows[5], (ulong)ReplaySectionKind.Pauses, pausesOffset, (uint)pausesBlockSize, (uint)sizeof(ReplayPause), pauseCount, 0);

        // 5. Build and pack the local string payload segment data inside the Info block boundary
        var textWritePtr = buffer + infoBlobOffset;
        var maxEndPtr = buffer + currentOffset;

        // Passing 'buffer + infoOffset' ensures offsets are stored relative to the start of the block
        var infoVOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, vStr);
        var infoGOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, gStr);
        var infoPUniqueOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, pUniqueStr);
        var infoPIdOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, pIdStr);
        var infoPNameOffset = WriteUtf16(buffer + infoOffset, ref textWritePtr, maxEndPtr, pNameStr);
        var infoPlatOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, platStr);
        var infoTrackOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, trackStr);
        var infoHmdOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, hmdStr);
        var infoLCtrlOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, lCtrlStr);
        var infoRCtrlOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, rCtrlStr);
        var infoHashOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, hashStr);
        var infoModeOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, modeStr);
        var infoDiffOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, diffStr);
        var infoSongOffset = WriteUtf16(buffer + infoOffset, ref textWritePtr, maxEndPtr, songStr);
        var infoMapperOffset = WriteUtf16(buffer + infoOffset, ref textWritePtr, maxEndPtr, mapperStr);
        var infoEnvOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, envStr);
        var infoModOffset = WriteUtf8(buffer + infoOffset, ref textWritePtr, maxEndPtr, modStr);

        // 6. Map offsets and explicit string lengths safely back to RawReplayInfo fields
        var rawInfo = (RawReplayInfo*)(buffer + infoOffset);
        rawInfo->OriginalVersion = header->Version;

        rawInfo->Version = PackUtf8(infoVOffset, vStr);
        rawInfo->GameVersion = PackUtf8(infoGOffset, gStr);
        rawInfo->PlayerUniqueId = PackUtf8(infoPUniqueOffset, pUniqueStr);
        rawInfo->PlayerId = PackUtf8(infoPIdOffset, pIdStr);
        rawInfo->PlayerName = PackUtf16(infoPNameOffset, pNameStr);
        rawInfo->Platform = PackUtf8(infoPlatOffset, platStr);
        rawInfo->TrackingSystem = PackUtf8(infoTrackOffset, trackStr);
        rawInfo->Hmd = PackUtf8(infoHmdOffset, hmdStr);
        rawInfo->LeftController = PackUtf8(infoLCtrlOffset, lCtrlStr);
        rawInfo->RightController = PackUtf8(infoRCtrlOffset, rCtrlStr);
        rawInfo->Hash = PackUtf8(infoHashOffset, hashStr);
        rawInfo->Mode = PackUtf8(infoModeOffset, modeStr);
        rawInfo->Difficulty = PackUtf8(infoDiffOffset, diffStr);
        rawInfo->SongName = PackUtf16(infoSongOffset, songStr);
        rawInfo->Mapper = PackUtf16(infoMapperOffset, mapperStr);
        rawInfo->Environment = PackUtf8(infoEnvOffset, envStr);
        rawInfo->Modifiers = PackUtf8(infoModOffset, modStr);

        // Populate scalars
        rawInfo->Bpm = 120.0f;
        rawInfo->Njs = 16.0f;
        rawInfo->Score = 1250000;
        rawInfo->Timestamp = 1672531199;

        // Frame data loops
        var frames = (ReplayFrame*)(buffer + framesOffset);
        for (uint i = 0; i < frameCount; i++) {
            frames[i].Time = i * 0.011f;
            frames[i].Fps = 90;
            frames[i].Head.Position = new Vector3 { X = 0, Y = 1.7f, Z = 0 };
            frames[i].Head.Rotation = new Quaternion { X = 0, Y = 0, Z = 0, W = 1 };
        }

        var notes = (ReplayNote*)(buffer + notesOffset);
        for (uint i = 0; i < noteCount; i++) {
            notes[i].Id = new NoteId { SpawnId = (int)i, LineIndex = 2, LineLayer = 0 };
            notes[i].EventTime = 5.0f + i;
            notes[i].EventType = NoteEventType.Good;
        }

        var walls = (ReplayWall*)(buffer + wallsOffset);
        for (uint i = 0; i < wallCount; i++) {
            walls[i].Id = new WallId { SpawnId = (int)i, LineIndex = 0, Width = 1, Duration = 2.0f };
            walls[i].InTime = 10.0f + i;
        }

        var heights = (ReplayHeight*)(buffer + heightsOffset);
        for (uint i = 0; i < heightCount; i++) {
            heights[i].Time = 0.0f;
            heights[i].Height = 1.8f;
        }

        var pauses = (ReplayPause*)(buffer + pausesOffset);
        for (uint i = 0; i < pauseCount; i++) {
            pauses[i].Time = (int)(20 + i);
            pauses[i].Duration = 5.0f;
        }

        return buffer;
    }

    private static void PopulateRow(ReplaySectionsTableRow* row, ulong sectionId, uint offset, uint size, uint itemSize, uint itemCount, uint blobOffset) {
        row->SectionOffset = offset;
        row->SectionSize = size;
        row->ItemSize = itemSize;
        row->ItemCount = itemCount;
        row->FirstItemOffset = 0;
        row->BlobOffset = blobOffset;

        *(ulong*)row->Id = sectionId;
    }

    private static uint WriteUtf8(byte* baseAddress, ref byte* currentPos, byte* maxEnd, string text) {
        var relativeOffset = (uint)(currentPos - baseAddress);
        var remainingBufferSpace = (int)(maxEnd - currentPos);

        var written = Encoding.UTF8.GetBytes(text.AsSpan(), new Span<byte>(currentPos, remainingBufferSpace));
        currentPos += written; // Clean byte count jump, no trailing null bit
        return relativeOffset;
    }

    private static uint WriteUtf16(byte* baseAddress, ref byte* currentPos, byte* maxEnd, string text) {
        var relativeOffset = (uint)(currentPos - baseAddress);
        var remainingBufferSpace = (int)(maxEnd - currentPos);

        var written = Encoding.Unicode.GetBytes(text.AsSpan(), new Span<byte>(currentPos, remainingBufferSpace));
        currentPos += written; // Clean byte count jump, no trailing null bits
        return relativeOffset;
    }

    private static RawUtf8String PackUtf8(uint offset, string value) {
        RawUtf8String str = default;
        var fields = (uint*)&str;
        fields[0] = (uint)Encoding.UTF8.GetByteCount(value);
        fields[1] = offset;
        return str;
    }

    private static RawUtf16String PackUtf16(uint offset, string value) {
        RawUtf16String str = default;
        var fields = (uint*)&str;
        fields[0] = (uint)Encoding.Unicode.GetByteCount(value);
        fields[1] = offset;
        return str;
    }
}