namespace BsorV2;

public class LazyString {
    public string Managed {
        get {
            if (field == null) {
                unsafe {
                    field = _utf8String != null ? 
                        _utf8String.Value.ToManaged(_buffer) : 
                        _utf16String!.Value.ToManaged(_buffer);
                }
            }
            
            return field;
        }
    }

    private readonly RawUtf8String? _utf8String;
    private readonly RawUtf16String? _utf16String;
    private readonly unsafe byte* _buffer;

    public unsafe LazyString(byte* buffer, RawUtf8String utf8String) {
        _buffer = buffer;
        _utf8String = utf8String;
    }

    public unsafe LazyString(byte* buffer, RawUtf16String utf16String) {
        _buffer = buffer;
        _utf16String = utf16String;
    }
}