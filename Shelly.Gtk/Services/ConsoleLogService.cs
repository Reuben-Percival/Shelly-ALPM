using System.Text;

namespace Shelly.Gtk.Services;

public class ConsoleLogService(ILockoutService lockoutService) : TextWriter
{
    private TextWriter? _originalOut;
    private TextWriter? _originalError;
    private bool _isStarted;

    public void Start()
    {
        if (_isStarted) return;
        _originalOut = Console.Out;
        _originalError = Console.Error;
        Console.SetOut(this);
        Console.SetError(this);
        _isStarted = true;
    }

    public void Stop()
    {
        if (!_isStarted) return;
        if (_originalOut != null) Console.SetOut(_originalOut);
        if (_originalError != null) Console.SetError(_originalError);
        _isStarted = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop();
        }
        base.Dispose(disposing);
    }

    public override void WriteLine(string? value)
    {
        _originalOut?.WriteLine(value);
        if (!string.IsNullOrEmpty(value))
        {
            lockoutService.ParseLog(value);
        }
    }

    public override void Write(string? value)
    {
        _originalOut?.Write(value);
    }

    public override void Write(char value)
    {
        _originalOut?.Write(value);
    }

    public override Encoding Encoding => _originalOut?.Encoding ?? Encoding.UTF8;

    public override void Flush()
    {
        _originalOut?.Flush();
        _originalError?.Flush();
    }
}
