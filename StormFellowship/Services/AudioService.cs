using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace StormFellowship.Services;

public class AudioService : IAudioService
{
    private static AudioService? _instance;
    public static AudioService Instance => _instance ??= new AudioService();

    private readonly System.Timers.Timer _levelTimer;
    private readonly Random _random = new();
    
    private bool _isMuted;
    private bool _isDeafened;
    private bool _isPushToTalkEnabled = false;
    private string _pushToTalkKey = "Mouse4";
    private double _vadSensitivityThreshold = 35.0; // 0-100%
    private double _currentMicLevel = 0.0;
    private bool _isSpeaking = false;
    private bool _isNoiseSuppressionEnabled = true;
    private bool _isEchoCancellationEnabled = true;
    private bool _is3DPositionalAudioEnabled = true;

    public event Action<double>? MicLevelChanged;
    public event Action<bool>? SpeakingStateChanged;

    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (_isMuted != value)
            {
                _isMuted = value;
                PlaySoundCue(value ? SoundCueType.Mute : SoundCueType.Unmute);
            }
        }
    }

    public bool IsDeafened
    {
        get => _isDeafened;
        set
        {
            if (_isDeafened != value)
            {
                _isDeafened = value;
                PlaySoundCue(value ? SoundCueType.Deafen : SoundCueType.Undeafen);
            }
        }
    }

    public bool IsPushToTalkEnabled
    {
        get => _isPushToTalkEnabled;
        set => _isPushToTalkEnabled = value;
    }

    public string PushToTalkKey
    {
        get => _pushToTalkKey;
        set => _pushToTalkKey = value;
    }

    public double VadSensitivityThreshold
    {
        get => _vadSensitivityThreshold;
        set => _vadSensitivityThreshold = value;
    }

    public double CurrentMicLevel => _currentMicLevel;
    public bool IsSpeaking => _isSpeaking;
    public bool IsNoiseSuppressionEnabled { get => _isNoiseSuppressionEnabled; set => _isNoiseSuppressionEnabled = value; }
    public bool IsEchoCancellationEnabled { get => _isEchoCancellationEnabled; set => _isEchoCancellationEnabled = value; }
    public bool Is3DPositionalAudioEnabled { get => _is3DPositionalAudioEnabled; set => _is3DPositionalAudioEnabled = value; }

    public AudioService()
    {
        _levelTimer = new System.Timers.Timer(50); // 20 FPS VU-meter updates
        _levelTimer.Elapsed += OnLevelTimerElapsed;
        StartMicMonitoring();
    }

    public void StartMicMonitoring()
    {
        if (!_levelTimer.Enabled)
        {
            _levelTimer.Start();
        }
    }

    public void StopMicMonitoring()
    {
        _levelTimer.Stop();
    }

    private void OnLevelTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (IsMuted)
        {
            _currentMicLevel = 0;
            SetSpeaking(false);
            return;
        }

        // Realistic dynamic audio level generator for voice activity simulation & monitoring
        double target = _random.Next(10, 85);
        if (_random.NextDouble() > 0.6)
        {
            target += _random.Next(15, 35);
        }
        
        // Smooth lerp
        _currentMicLevel = (_currentMicLevel * 0.4) + (target * 0.6);
        if (_currentMicLevel > 100) _currentMicLevel = 100;
        if (_currentMicLevel < 0) _currentMicLevel = 0;

        bool speaking = _currentMicLevel >= _vadSensitivityThreshold;
        SetSpeaking(speaking);

        MicLevelChanged?.Invoke(_currentMicLevel);
    }

    private void SetSpeaking(bool speaking)
    {
        if (_isSpeaking != speaking)
        {
            _isSpeaking = speaking;
            SpeakingStateChanged?.Invoke(speaking);
        }
    }

    public void PlaySoundCue(SoundCueType cue)
    {
        Task.Run(() =>
        {
            try
            {
                // Synthesize smooth high-frequency professional sound beeps via NAudio WaveOut
                int freq1 = 800, freq2 = 1200, durationMs = 80;
                switch (cue)
                {
                    case SoundCueType.Mute:
                        freq1 = 700; freq2 = 450; durationMs = 70;
                        break;
                    case SoundCueType.Unmute:
                        freq1 = 450; freq2 = 750; durationMs = 70;
                        break;
                    case SoundCueType.Deafen:
                        freq1 = 600; freq2 = 350; durationMs = 90;
                        break;
                    case SoundCueType.Undeafen:
                        freq1 = 350; freq2 = 650; durationMs = 90;
                        break;
                    case SoundCueType.UserJoin:
                        freq1 = 523; freq2 = 659; durationMs = 100;
                        break;
                    case SoundCueType.UserLeave:
                        freq1 = 659; freq2 = 523; durationMs = 100;
                        break;
                    case SoundCueType.MessageReceived:
                        freq1 = 900; freq2 = 1100; durationMs = 50;
                        break;
                    case SoundCueType.CallStart:
                        freq1 = 440; freq2 = 880; durationMs = 120;
                        break;
                    case SoundCueType.CallEnd:
                        freq1 = 880; freq2 = 440; durationMs = 120;
                        break;
                }

                PlayDualTone(freq1, freq2, durationMs);
            }
            catch
            {
                // Fallback safe silent handle
            }
        });
    }

    private static void PlayDualTone(int freq1, int freq2, int durationMs)
    {
        try
        {
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            var sig1 = new SignalGenerator(44100, 2)
            {
                Gain = 0.12,
                Frequency = freq1,
                Type = SignalGeneratorType.Sin
            }.Take(TimeSpan.FromMilliseconds(durationMs / 2.0));

            var sig2 = new SignalGenerator(44100, 2)
            {
                Gain = 0.12,
                Frequency = freq2,
                Type = SignalGeneratorType.Sin
            }.Take(TimeSpan.FromMilliseconds(durationMs / 2.0));

            var playlist = new ConcatenatingSampleProvider(new[] { sig1, sig2 });

            using var waveOut = new WaveOutEvent();
            waveOut.Init(playlist);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(20);
            }
        }
        catch
        {
            // Ignore sound device in-use or unavailable
        }
    }
}
