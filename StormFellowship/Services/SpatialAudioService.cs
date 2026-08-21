using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Services;

public partial class SpatialParticipantNode : ObservableObject
{
    [ObservableProperty]
    private string _userId = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _avatarGlyph = "👤";

    [ObservableProperty]
    private double _posX = 0.0; // -100 to +100

    [ObservableProperty]
    private double _posY = 0.0; // -100 to +100

    [ObservableProperty]
    private double _pan = 0.0; // -1.0 (Left) to +1.0 (Right)

    [ObservableProperty]
    private double _volumeFactor = 1.0; // 0.2 to 1.5

    [ObservableProperty]
    private bool _isSpeaking = false;
}

/// <summary>
/// 3D Spatial Proximity Audio positioning engine for voice channels.
/// Computes binaural/stereo panning and distance attenuation for virtual room placement.
/// </summary>
public class SpatialAudioService
{
    private static SpatialAudioService? _instance;
    public static SpatialAudioService Instance => _instance ??= new SpatialAudioService();

    public bool IsSpatialAudioEnabled { get; set; } = true;
    public ObservableCollection<SpatialParticipantNode> Nodes { get; } = new();

    public SpatialAudioService()
    {
        SeedNodes();
    }

    private void SeedNodes()
    {
        Nodes.Add(new SpatialParticipantNode { UserId = "user_alex", DisplayName = "Алексей", AvatarGlyph = "🛡️", PosX = -60.0, PosY = 30.0, Pan = -0.6, VolumeFactor = 1.0 });
        Nodes.Add(new SpatialParticipantNode { UserId = "user_kate", DisplayName = "Екатерина", AvatarGlyph = "🎵", PosX = 60.0, PosY = 30.0, Pan = 0.6, VolumeFactor = 1.0 });
        Nodes.Add(new SpatialParticipantNode { UserId = "user_bot", DisplayName = "STORM Bot", AvatarGlyph = "🤖", PosX = 0.0, PosY = -70.0, Pan = 0.0, VolumeFactor = 1.1 });
    }

    public void UpdateParticipantPosition(SpatialParticipantNode node, double x, double y)
    {
        node.PosX = Math.Clamp(x, -100.0, 100.0);
        node.PosY = Math.Clamp(y, -100.0, 100.0);

        // Compute Pan: -1.0 (far left) to +1.0 (far right)
        node.Pan = node.PosX / 100.0;

        // Compute Distance falloff
        double dist = Math.Sqrt(node.PosX * node.PosX + node.PosY * node.PosY);
        node.VolumeFactor = Math.Clamp(1.2 - (dist / 200.0), 0.3, 1.4);
    }
}
