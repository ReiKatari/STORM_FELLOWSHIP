using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public bool IsAuthenticated => AuthService.Instance.IsAuthenticated;
    public string UserEmail => AuthService.Instance.CurrentSession?.Email ?? "Офлайн аккаунт";
    public string UserDisplayName => AuthService.Instance.CurrentSession?.DisplayName ?? FellowshipService.Instance.CurrentUser.DisplayName;
    public string CloudStatusText => AuthService.Instance.CloudStatusText;
    public string LastSyncFormatted => CloudSyncService.Instance.LastSyncTime?.ToString("HH:mm:ss") ?? "Синхронизировано";

    [ObservableProperty]
    private int _selectedTabIndex = 0; // 0: Login, 1: Register, 2: Cloud Sync

    // Login Fields
    [ObservableProperty]
    private string _loginEmail = string.Empty;

    [ObservableProperty]
    private string _loginPassword = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    // Register Fields
    [ObservableProperty]
    private string _registerEmail = string.Empty;

    [ObservableProperty]
    private string _registerPassword = string.Empty;

    [ObservableProperty]
    private string _registerDisplayName = string.Empty;

    [ObservableProperty]
    private string _selectedRegisterAvatar = "⚡";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isLoading = false;

    public ObservableCollection<AvatarPresetItem> AvailableAvatars { get; } = new()
    {
        new("GeoLightning", "Молния", "#3B82F6"),
        new("GeoShield", "Щит", "#10B981"),
        new("GeoCrown", "Корона", "#F59E0B"),
        new("GeoGamepad", "Гейминг", "#8B5CF6"),
        new("GeoRocket", "Ракета", "#EC4899"),
        new("GeoDiamond", "Алмаз", "#06B6D4"),
        new("GeoFire", "Огонь", "#EF4444"),
        new("GeoStar", "Звезда", "#FBBF24")
    };

    public AuthViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        AuthService.Instance.AuthStateChanged += (session) =>
        {
            OnPropertyChanged(nameof(IsAuthenticated));
            OnPropertyChanged(nameof(UserEmail));
            OnPropertyChanged(nameof(UserDisplayName));
            OnPropertyChanged(nameof(CloudStatusText));
        };

        CloudSyncService.Instance.SyncCompleted += (time) =>
        {
            OnPropertyChanged(nameof(LastSyncFormatted));
        };
    }

    [RelayCommand]
    public async Task Login()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(LoginEmail) || string.IsNullOrWhiteSpace(LoginPassword))
        {
            ErrorMessage = "Заполните email и пароль";
            return;
        }

        IsLoading = true;
        try
        {
            bool ok = await AuthService.Instance.LoginAsync(LoginEmail, LoginPassword);
            if (ok)
            {
                _mainVM.RefreshUserProfileBindings();
                _mainVM.CloseAuthDialog();
                _mainVM.ShowToastNotification($"✅ Успешный вход: {LoginEmail}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task Register()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(RegisterEmail) || string.IsNullOrWhiteSpace(RegisterPassword))
        {
            ErrorMessage = "Заполните email и пароль";
            return;
        }

        IsLoading = true;
        try
        {
            bool ok = await AuthService.Instance.RegisterAsync(RegisterEmail, RegisterPassword, RegisterDisplayName, SelectedRegisterAvatar);
            if (ok)
            {
                _mainVM.RefreshUserProfileBindings();
                _mainVM.CloseAuthDialog();
                _mainVM.ShowToastNotification($"🎉 Аккаунт успешно создан и синхронизирован с облаком!");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Logout()
    {
        AuthService.Instance.Logout();
        _mainVM.RefreshUserProfileBindings();
        _mainVM.ShowToastNotification("🚪 Вы вышли из облачного аккаунта");
    }

    [RelayCommand]
    public async Task SyncNow()
    {
        IsLoading = true;
        bool ok = await CloudSyncService.Instance.SyncNowAsync();
        IsLoading = false;
        if (ok)
        {
            _mainVM.ShowToastNotification("☁️ Облачные данные успешно синхронизированы!");
        }
    }

    [RelayCommand]
    public void SelectAvatar(string? glyph)
    {
        if (!string.IsNullOrWhiteSpace(glyph))
        {
            SelectedRegisterAvatar = glyph;
        }
    }

    [RelayCommand]
    public void CloseModal()
    {
        _mainVM.CloseAuthDialog();
    }
}
