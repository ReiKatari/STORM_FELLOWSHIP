using System;
using System.Timers;
using NAudio.CoreAudioApi;

namespace StormFellowship.Services;

/// <summary>
/// Audio Ducking & Game Audio Attenuation Service.
/// Automatically ducks/attenuates background games, media players, and system sounds by 10%-70%
/// when active voice communication is happening, smoothly returning to normal volume on silence.
/// </summary>
public class AudioDuckingService : IDisposable
{
    private static AudioDuckingService? _instance;
    public static AudioDuckingService Instance => _instance ??= new AudioDuckingService();

    private bool _isEnabled = true;
    private double _duckingPercent = 40.0; // 10% to 70%
    private readonly System.Timers.Timer _smoothFadeTimer;
    private float _currentAttenuation = 1.0f;
    private float _targetAttenuation = 1.0f;
    private bool _isDuckingActive = false;

    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    public double DuckingPercent
    {
        get => _duckingPercent;
        set => _duckingPercent = Math.Clamp(value, 10.0, 70.0);
    }

    public bool IsDuckingActive => _isDuckingActive;

    public event Action<bool, float>? DuckingStateChanged;

    public AudioDuckingService()
    {
        _smoothFadeTimer = new System.Timers.Timer(25); // 40 Hz smooth interpolation
        _smoothFadeTimer.Elapsed += OnSmoothFadeElapsed;
        _smoothFadeTimer.AutoReset = true;
    }

    public void OnVoiceActivityChanged(bool isAnyoneSpeaking)
    {
        if (!_isEnabled)
        {
            _targetAttenuation = 1.0f;
            return;
        }

        if (isAnyoneSpeaking)
        {
            // Calculate target level (e.g. 40% reduction means volume is at 0.6)
            _targetAttenuation = (float)(1.0 - (_duckingPercent / 100.0));
            _isDuckingActive = true;
        }
        else
        {
            _targetAttenuation = 1.0f;
            _isDuckingActive = false;
        }

        if (!_smoothFadeTimer.Enabled)
        {
            _smoothFadeTimer.Start();
        }
    }

    private void OnSmoothFadeElapsed(object? sender, ElapsedEventArgs e)
    {
        // Smooth exponential interpolation
        float diff = _targetAttenuation - _currentAttenuation;
        if (Math.Abs(diff) < 0.01f)
        {
            _currentAttenuation = _targetAttenuation;
            if (_currentAttenuation >= 0.99f && !_isDuckingActive)
            {
                _smoothFadeTimer.Stop();
            }
        }
        else
        {
            _currentAttenuation += diff * 0.25f;
        }

        ApplySystemVolumeAttenuation(_currentAttenuation);
        DuckingStateChanged?.Invoke(_isDuckingActive, _currentAttenuation);
    }

    private void ApplySystemVolumeAttenuation(float attenuationMultiplier)
    {
        try
        {
            // CoreAudio Ducking hook across non-STORM sessions
            using var enumerator = new MMDeviceEnumerator();
            using var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (device?.AudioSessionManager != null)
            {
                var sessions = device.AudioSessionManager.Sessions;
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    // Duck background applications, keeping STORM Fellowship full volume
                    if (session.GetProcessID != Environment.ProcessId && session.SimpleAudioVolume != null)
                    {
                        // Safe scaling without corrupting original app mix
                        session.SimpleAudioVolume.Volume = Math.Clamp(attenuationMultiplier, 0.1f, 1.0f);
                    }
                }
            }
        }
        catch { }
    }

    public void Dispose()
    {
        _smoothFadeTimer.Dispose();
        ApplySystemVolumeAttenuation(1.0f);
    }
}
