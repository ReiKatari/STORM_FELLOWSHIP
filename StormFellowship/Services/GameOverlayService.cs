using System.Windows;
using StormFellowship.Views.Overlay;

namespace StormFellowship.Services;

public class GameOverlayService
{
    private static GameOverlayService? _instance;
    public static GameOverlayService Instance => _instance ??= new GameOverlayService();

    private GameOverlayWindow? _overlayWindow;
    public bool IsOverlayActive => _overlayWindow != null && _overlayWindow.IsVisible;

    public event Action<bool>? OverlayStateChanged;

    public void ToggleOverlay()
    {
        if (IsOverlayActive)
        {
            HideOverlay();
        }
        else
        {
            ShowOverlay();
        }
    }

    public void ShowOverlay()
    {
        try
        {
            if (_overlayWindow == null)
            {
                _overlayWindow = new GameOverlayWindow();
                _overlayWindow.Closed += (s, e) => _overlayWindow = null;
            }

            _overlayWindow.Show();
            OverlayStateChanged?.Invoke(true);
        }
        catch
        {
            // Fallback safe
        }
    }

    public void HideOverlay()
    {
        try
        {
            _overlayWindow?.Hide();
            OverlayStateChanged?.Invoke(false);
        }
        catch { }
    }
}
