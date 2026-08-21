namespace StormFellowship.Services;

public interface IAudioService
{
    bool IsMuted { get; set; }
    bool IsDeafened { get; set; }
    bool IsPushToTalkEnabled { get; set; }
    string PushToTalkKey { get; set; }
    double VadSensitivityThreshold { get; set; }
    double InputVolume { get; set; }
    double OutputVolume { get; set; }
    double CurrentMicLevel { get; }
    bool IsSpeaking { get; }
    bool IsNoiseSuppressionEnabled { get; set; }
    bool IsEchoCancellationEnabled { get; set; }
    bool Is3DPositionalAudioEnabled { get; set; }
    AudioDirectionMode DirectionMode { get; set; }

    event Action<double>? MicLevelChanged;
    event Action<bool>? SpeakingStateChanged;

    void PlaySoundCue(SoundCueType cue);
    void PlayTestChime();
    void StartMicMonitoring();
    void StopMicMonitoring();
}
