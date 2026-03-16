using System.Text;

namespace DeadworksManaged;

internal sealed unsafe class NativeLogWriter : TextWriter
{
    private readonly delegate* unmanaged[Cdecl]<char*, void> _callback;

    public NativeLogWriter(delegate* unmanaged[Cdecl]<char*, void> callback)
    {
        _callback = callback;
    }

    public override Encoding Encoding => Encoding.Unicode;

    public override void WriteLine(string? value)
    {
        if (_callback == null || value is null)
            return;

        fixed (char* ptr = value)
        {
            _callback(ptr);
        }
    }

    public override void Write(string? value)
    {
        if (_callback == null || value is null)
            return;

        fixed (char* ptr = value)
        {
            _callback(ptr);
        }
    }
}
