using System.Text.RegularExpressions;

namespace Shelly.Gtk.Services;

public partial class LockoutService : ILockoutService
{
    private static readonly Regex FlatpakProgressPattern =
        FlatpakRegex();

    public event EventHandler<ILockoutService.LockoutStatusEventArgs>? StatusChanged;

    public bool IsLocked { get; private set; }

    public double Progress { get; private set; }

    public bool IsIndeterminate { get; private set; } = true;

    public string? Description { get; private set; }

    public void Show(string description, double progress = 0, bool isIndeterminate = true)
    {
        _consoleLogService ??= new ConsoleLogService(this);
        _consoleLogService.Start();

        IsLocked = true;
        Description = description;
        Progress = progress;
        IsIndeterminate = isIndeterminate;
        NotifyChanged();
    }

    private void Update(string? description = null, double? progress = null, bool? isIndeterminate = null)
    {
        if (description != null) Description = description;
        if (progress != null) Progress = progress.Value;
        if (isIndeterminate != null) IsIndeterminate = isIndeterminate.Value;
        NotifyChanged();
    }

    public void Hide()
    {
        IsLocked = false;
        _consoleLogService?.Stop();
        NotifyChanged();
    }

    private ConsoleLogService? _consoleLogService;

    private class LogObserver(LockoutService service) : IObserver<string?>
    {
        public void OnCompleted() => service.Hide();
        public void OnError(Exception error) => service.Hide();
        public void OnNext(string? value) => service.ParseLog(value);
    }

    public IObserver<string?> GetLogObserver()
    {
        return new LogObserver(this);
    }

    public void ParseLog(string? logLine)
    {
        if (string.IsNullOrEmpty(logLine)) return;

        var match = FlatpakProgressPattern.Match(logLine);
        if (match.Success)
        {
            if (double.TryParse(match.Groups[1].Value, out var progress))
            {
                var description = match.Groups[2].Value;
                Update(description, progress, false);
            }
        }
    }

    private void NotifyChanged()
    {
        StatusChanged?.Invoke(this, new ILockoutService.LockoutStatusEventArgs
        {
            IsLocked = IsLocked,
            Description = Description,
            Progress = Progress,
            IsIndeterminate = IsIndeterminate
        });
    }

    [GeneratedRegex(@"\[DEBUG_LOG\]\s*Progress:\s*(\d+)%\s*-\s*(.+)", RegexOptions.Compiled)]
    private static partial Regex FlatpakRegex();
}