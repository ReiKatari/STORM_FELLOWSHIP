namespace StormFellowship.Services;

public interface ITrayService
{
    bool IsInitialized { get; }
    void Initialize(nint hwnd, string tooltip = "STORM FELLOWSHIP v0.0.1");
    void ShowNotification(string title, string message);
    void MinimizeToTray();
    void RestoreFromTray();
    void Dispose();
}
