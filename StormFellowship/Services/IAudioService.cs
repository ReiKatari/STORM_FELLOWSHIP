namespace StormFellowship.Services;

public interface IAudioService
{
    bool IsMuted { get; set; }
    bool IsDeafened { get; set; }
    bool IsPushToTalkEnabled { get; set; }
    string PushToTalkKey { get; set; }
    double VadSensitivityThreshold { get; set; }
    double CurrentMicLevel { get; }
    bool IsSpeaking { get; }
    bool IsNoiseSuppressionEnabled { get; set; }
    bool IsEchoCancellationEnabled { get; set; }
    bool Is3DPositionalAudioEnabled { get; set; }

    event Action<double>? MicLevelChanged;
    event Action<bool>? SpeakingStateChanged;

    void PlaySoundCue(SoundCueType cue);
    void StartMicMonitoring();
    void StopMicMonitoring();
}

public enum SoundCueType
{
    UserJoin,
    UserLeave,
    Mute,
    Unmute,
    Deafen,
    Undeafen,
    MessageReceived,
    CallStart,
    CallEnd,
    PushToTalkOn,
    PushToTalkOff
}
