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
        // For custom section table, we include a 7th section row for the raw string pool text block
        var baseSections = (ushort)ReplaySectionKind.BuiltinSectionsCount;
        var tableSize = (ushort)(baseSections + 1); // +1 for the .text section

        var headerSize = (ulong)sizeof(ReplayHeader);
        var tableBytes = (ulong)tableSize * (ulong)sizeof(ReplaySectionsTableRow);

        var infoBlockSize = (ulong)sizeof(RawReplayInfo);
        var framesBlockSize = (ulong)frameCount * (ulong)sizeof(ReplayFrame);
        var notesBlockSize = (ulong)noteCount * (ulong)sizeof(ReplayNote);
        var wallsBlockSize = (ulong)wallCount * (ulong)sizeof(ReplayWall);
        var heightsBlockSize = (ulong)heightCount * (ulong)sizeof(ReplayHeight);
        var pausesBlockSize = (ulong)pauseCount * (ulong)sizeof(ReplayPause);

        // Compute memory footprint requirements for the raw string chunks (.text segment)
        // UTF-8 strings take up length bytes + 1 null terminator byte.
        // UTF-16 strings take up length * 2 bytes + 2 null terminator bytes.
        ulong textBlockSize = 0;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(vStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(gStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(pUniqueStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(pIdStr) + 1;
        textBlockSize += (ulong)Encoding.Unicode.GetByteCount(pNameStr) + 2; // UTF-16
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(platStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(trackStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(hmdStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(lCtrlStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(rCtrlStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(hashStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(modeStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(diffStr) + 1;
        textBlockSize += (ulong)Encoding.Unicode.GetByteCount(songStr) + 2;   // UTF-16
        textBlockSize += (ulong)Encoding.Unicode.GetByteCount(mapperStr) + 2; // UTF-16
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(envStr) + 1;
        textBlockSize += (ulong)Encoding.UTF8.GetByteCount(modStr) + 1;

        // 2. Track linear storage positioning layout offsets
        var currentOffset = headerSize + tableBytes;

        var infoOffset = currentOffset;
        currentOffset += infoBlockSize;
        var framesOffset = currentOffset;
        currentOffset += framesBlockSize;
        var notesOffset = currentOffset;
        currentOffset += notesBlockSize;
        var wallsOffset = currentOffset;
        currentOffset += wallsBlockSize;
        var heightsOffset = currentOffset;
        currentOffset += heightsBlockSize;
        var pausesOffset = currentOffset;
        currentOffset += pausesBlockSize;
        var textOffset = currentOffset;
        currentOffset += textBlockSize;

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

        PopulateRow(&rows[0], (ulong)ReplaySectionKind.Info, infoOffset, infoBlockSize, (ulong)sizeof(RawReplayInfo), 1);
        PopulateRow(&rows[1], (ulong)ReplaySectionKind.Frames, framesOffset, framesBlockSize, (ulong)sizeof(ReplayFrame), frameCount);
        PopulateRow(&rows[2], (ulong)ReplaySectionKind.Notes, notesOffset, notesBlockSize, (ulong)sizeof(ReplayNote), noteCount);
        PopulateRow(&rows[3], (ulong)ReplaySectionKind.Walls, wallsOffset, wallsBlockSize, (ulong)sizeof(ReplayWall), wallCount);
        PopulateRow(&rows[4], (ulong)ReplaySectionKind.Heights, heightsOffset, heightsBlockSize, (ulong)sizeof(ReplayHeight), heightCount);
        PopulateRow(&rows[5], (ulong)ReplaySectionKind.Pauses, pausesOffset, pausesBlockSize, (ulong)sizeof(ReplayPause), pauseCount);

        // Custom .text layout registration (Casting ".text" into string table slot)
        PopulateCustomRow(&rows[6], ".text", textOffset, textBlockSize, 1, 1);

        // 5. Build and pack the .text string payload segment data
        var textWritePtr = buffer + textOffset;

        // This helper sequentially packs encoded bytes and returns relative buffer offsets
        var infoVOffset = WriteUtf8(buffer, ref textWritePtr, vStr);
        var infoGOffset = WriteUtf8(buffer, ref textWritePtr, gStr);
        var infoPUniqueOffset = WriteUtf8(buffer, ref textWritePtr, pUniqueStr);
        var infoPIdOffset = WriteUtf8(buffer, ref textWritePtr, pIdStr);
        var infoPNameOffset = WriteUtf16(buffer, ref textWritePtr, pNameStr);
        var infoPlatOffset = WriteUtf8(buffer, ref textWritePtr, platStr);
        var infoTrackOffset = WriteUtf8(buffer, ref textWritePtr, trackStr);
        var infoHmdOffset = WriteUtf8(buffer, ref textWritePtr, hmdStr);
        var infoLCtrlOffset = WriteUtf8(buffer, ref textWritePtr, lCtrlStr);
        var infoRCtrlOffset = WriteUtf8(buffer, ref textWritePtr, rCtrlStr);
        var infoHashOffset = WriteUtf8(buffer, ref textWritePtr, hashStr);
        var infoModeOffset = WriteUtf8(buffer, ref textWritePtr, modeStr);
        var infoDiffOffset = WriteUtf8(buffer, ref textWritePtr, diffStr);
        var infoSongOffset = WriteUtf16(buffer, ref textWritePtr, songStr);
        var infoMapperOffset = WriteUtf16(buffer, ref textWritePtr, mapperStr);
        var infoEnvOffset = WriteUtf8(buffer, ref textWritePtr, envStr);
        var infoModOffset = WriteUtf8(buffer, ref textWritePtr, modStr);

        // 6. Map offsets back to RawReplayInfo fields
        var rawInfo = (RawReplayInfo*)(buffer + infoOffset);
        rawInfo->OriginalVersion = header->Version;

        // Re-construct string structures manually using computed relative offsets
        // (Assuming layout maps directly to a 4-byte or 8-byte offset representation field)
        *(uint*)&rawInfo->Version = infoVOffset;
        *(uint*)&rawInfo->GameVersion = infoGOffset;
        *(uint*)&rawInfo->PlayerUniqueId = infoPUniqueOffset;
        *(uint*)&rawInfo->PlayerId = infoPIdOffset;
        *(uint*)&rawInfo->PlayerName = infoPNameOffset;
        *(uint*)&rawInfo->Platform = infoPlatOffset;
        *(uint*)&rawInfo->TrackingSystem = infoTrackOffset;
        *(uint*)&rawInfo->Hmd = infoHmdOffset;
        *(uint*)&rawInfo->LeftController = infoLCtrlOffset;
        *(uint*)&rawInfo->RightController = infoRCtrlOffset;
        *(uint*)&rawInfo->Hash = infoHashOffset;
        *(uint*)&rawInfo->Mode = infoModeOffset;
        *(uint*)&rawInfo->Difficulty = infoDiffOffset;
        *(uint*)&rawInfo->SongName = infoSongOffset;
        *(uint*)&rawInfo->Mapper = infoMapperOffset;
        *(uint*)&rawInfo->Environment = infoEnvOffset;
        *(uint*)&rawInfo->Modifiers = infoModOffset;

        // Populate scalars
        rawInfo->Bpm = 120.0f;
        rawInfo->Njs = 16.0f;
        rawInfo->Score = 1250000;
        rawInfo->Timestamp = 1672531199;

        // [Frame/Note/Wall/Height/Pause loops remain unchanged below]
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

    private static void PopulateRow(ReplaySectionsTableRow* row, ulong sectionId, ulong offset, ulong size, ulong itemSize, ulong itemCount) {
        row->SectionOffset = offset;
        row->SectionSize = size;
        row->ItemSize = itemSize;
        row->ItemCount = itemCount;
        *(ulong*)row->Id = sectionId;
    }

    private static void PopulateCustomRow(ReplaySectionsTableRow* row, string name, ulong offset, ulong size, ulong itemSize, ulong itemCount) {
        row->SectionOffset = offset;
        row->SectionSize = size;
        row->ItemSize = itemSize;
        row->ItemCount = itemCount;

        // Write text string into custom section block row label identifier
        var nameSpan = name.AsSpan();
        var idPtr = row->Id;
        
        Encoding.UTF8.GetBytes(nameSpan, new Span<byte>(idPtr, 64));
    }

    private static uint WriteUtf8(byte* baseAddress, ref byte* currentPos, string text) {
        var relativeOffset = (uint)(currentPos - baseAddress);
        var written = Encoding.UTF8.GetBytes(text.AsSpan(), new Span<byte>(currentPos, text.Length * 4));
        currentPos[written] = 0; // Null-termination
        currentPos += written + 1;
        return relativeOffset;
    }

    private static uint WriteUtf16(byte* baseAddress, ref byte* currentPos, string text) {
        var relativeOffset = (uint)(currentPos - baseAddress);
        var written = Encoding.Unicode.GetBytes(text.AsSpan(), new Span<byte>(currentPos, text.Length * 4));
        currentPos[written] = 0;     // Null-termination byte 1
        currentPos[written + 1] = 0; // Null-termination byte 2
        currentPos += written + 2;
        return relativeOffset;
    }
}