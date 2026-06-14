using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BsorV2;

public class StructAlignmentTests {
    public static void AlignsStructuresProperly() {
        Type[] structsToTest = [
            typeof(ReplayHeader),
            typeof(RawReplayInfo),
            typeof(ReplayFrame),
            typeof(ReplayNote),
            typeof(ReplayWall),
            typeof(ReplayPause),
            typeof(ReplayHeight),
        ];

        foreach (var type in structsToTest) {
            var report = new System.Text.StringBuilder();
            report.AppendLine($"\n=== {type.Name} ===");
            
            var failedFieldInfo = "";
            var success = VerifyPackedAlignmentRecursive(type, 0, "", report, ref failedFieldInfo);
            
            Console.WriteLine(report.ToString());

            if (!success) {
                throw new($"Target structure '{type.Name}' contains unaligned packed fields! Details:\n{failedFieldInfo}");
            }
        }
    }
    
    private static bool VerifyPackedAlignmentRecursive(Type type, int simulatedAbsoluteOffset, string prefix, System.Text.StringBuilder report, ref string failedFieldInfo) {
        if (type.IsEnum) {
            type = Enum.GetUnderlyingType(type);
        }
        
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var isValid = true;
        
        foreach (var field in fields) {
            var fieldDisplayName = string.IsNullOrEmpty(prefix) ? field.Name : $"{prefix}.{field.Name}";
            var requiredAlignment = GetNaturalAlignment(field.FieldType);
            var isMisaligned = (simulatedAbsoluteOffset % requiredAlignment) != 0;
            
            var status = " [OK] ";
            if (isMisaligned) {
                status = " [CRITICAL: PACKED MISALIGNMENT] ";
                isValid = false;
                if (string.IsNullOrEmpty(failedFieldInfo)) {
                    failedFieldInfo = $"Field '{fieldDisplayName}' (Type: {field.FieldType.Name}) at packed offset {simulatedAbsoluteOffset} violates its {requiredAlignment}-byte native hardware boundary alignment requirement.";
                }
            }

            report.AppendLine($"  -> Offset {simulatedAbsoluteOffset:D3} | Type: {field.FieldType.Name,-12} | Field: {fieldDisplayName,-26} {status}");
            
            var isCustomStruct = field.FieldType is { IsValueType: true, IsPrimitive: false, IsEnum: false };
            if (isCustomStruct) {
                if (!VerifyPackedAlignmentRecursive(field.FieldType, simulatedAbsoluteOffset, fieldDisplayName, report, ref failedFieldInfo)) {
                    isValid = false;
                }
            }

            var fieldSize = GetFieldSizeRuntime(field.FieldType);
            simulatedAbsoluteOffset += fieldSize;
        }

        return isValid;
    }
    
    private static int GetFieldSizeRuntime(Type fieldType) {
        if (fieldType.IsPointer) return IntPtr.Size;
        try {
            var method = typeof(Unsafe).GetMethod(nameof(Unsafe.SizeOf))?.MakeGenericMethod(fieldType);
            return (int)(method?.Invoke(null, null) ?? Marshal.SizeOf(fieldType));
        } catch {
            return Marshal.SizeOf(fieldType);
        }
    }
    
    private static int GetNaturalAlignment(Type type) {
        if (type.IsEnum) type = Enum.GetUnderlyingType(type);

        if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(bool)) return 1;
        if (type == typeof(short) || type == typeof(ushort) || type == typeof(char)) return 2;
        if (type == typeof(int) || type == typeof(uint) || type == typeof(float)) return 4;
        if (type == typeof(long) || type == typeof(ulong) || type == typeof(double) || type.IsPointer) return 8;
        
        if (type is { IsValueType: true, IsPrimitive: false }) {
            var maxAlign = 1;
            foreach (var f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                maxAlign = Math.Max(maxAlign, GetNaturalAlignment(f.FieldType));
            }
            return maxAlign;
        }

        return IntPtr.Size; 
    }
}