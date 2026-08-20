using StormFellowship.Models;

namespace StormFellowship.Services;

public interface ICallService
{
    CallSession? ActiveCall { get; }
    bool IsInCall { get; }
    
    event Action<CallSession?>? CallStateChanged;
    event Action<double[]>? WaveformUpdated;

    void StartDirectCall(User remoteUser, bool isVideo = false);
    void JoinVoiceChannel(Channel voiceChannel);
    void EndCall();
    void ToggleMute();
    void ToggleDeafen();
    void ToggleVideo();
    void ToggleScreenShare();
}
